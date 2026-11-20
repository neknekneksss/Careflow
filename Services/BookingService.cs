
using CareFlow.Data;
using CareFlow.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareFlow.Services;

public record ServiceAvailabilityInfo(
    int ServiceId,
    string ServiceName,
    int Capacity,
    int BookedCount,
    int RemainingCapacity
);

public record BookingResult(
    bool Success,
    string Message,
    int? BookingId = null
);

public record BookingSummary(
    int BookingId,
    string PatientName,
    DateTime AppointmentDate,
    string Services
);

public class BookingService
{
    private readonly CareFlowDbContext _db;

    public BookingService(CareFlowDbContext db)
    {
        _db = db;
    }

    public async Task<List<Patient>> GetPatientsAsync()
    {
        return await _db.Patients
            .OrderBy(p => p.FullName)
            .ToListAsync();
    }

    public async Task<List<DiagnosticService>> GetActiveServicesAsync()
    {
        return await _db.DiagnosticServices
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    // Retrieve all confirmed bookings with their patients and services.
    public async Task<List<BookingSummary>> GetConfirmedBookingsAsync()
    {
        var bookings = await _db.Bookings
            .AsNoTracking()
            .Where(b => b.Status == BookingStatus.Confirmed)
            .Include(b => b.Patient)
            .Include(b => b.BookingItems)
                .ThenInclude(item => item.Service)
            .OrderByDescending(b => b.AppointmentDate)
            .ThenByDescending(b => b.BookingId)
            .ToListAsync();

        return bookings.Select(b => new BookingSummary(
            b.BookingId,
            b.Patient.FullName,
            b.AppointmentDate,
            string.Join(", ", b.BookingItems.Select(item => item.Service.Name))
        )).ToList();
    }

    // Retrieve remaining capacity for every active service on a date.
    public async Task<List<ServiceAvailabilityInfo>> GetAvailabilityAsync(
        DateTime date)
    {
        var services = await _db.DiagnosticServices
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();

        var results = new List<ServiceAvailabilityInfo>();

        foreach (var service in services)
        {
            var availability = await _db.ServiceAvailabilities
                .FirstOrDefaultAsync(a =>
                    a.ServiceId == service.ServiceId &&
                    a.AvailableDate == date.Date);

            int capacity = availability?.Capacity ?? 0;

            int bookedCount = await _db.BookingItems
                .CountAsync(item =>
                    item.ServiceId == service.ServiceId &&
                    item.Booking.AppointmentDate == date.Date &&
                    item.Booking.Status == BookingStatus.Confirmed);

            results.Add(new ServiceAvailabilityInfo(
                service.ServiceId,
                service.Name,
                capacity,
                bookedCount,
                Math.Max(0, capacity - bookedCount)
            ));
        }

        return results;
    }

    // Confirm a booking as one atomic database operation.
    public async Task<BookingResult> CreateBookingAsync(
        int patientId,
        DateTime appointmentDate,
        List<int> serviceIds)
    {
        if (appointmentDate.Date < DateTime.Today)
        {
            return new BookingResult(false,
                "Appointment date cannot be in the past.");
        }

        if (serviceIds == null || serviceIds.Count < 1 || serviceIds.Count > 3)
        {
            return new BookingResult(false,
                "Select between one and three diagnostic services.");
        }

        if (serviceIds.Distinct().Count() != serviceIds.Count)
        {
            return new BookingResult(false,
                "The same diagnostic service cannot be selected twice.");
        }

        await using var transaction =
            await _db.Database.BeginTransactionAsync();

        try
        {
            bool patientExists = await _db.Patients
                .AnyAsync(p => p.PatientId == patientId);

            if (!patientExists)
            {
                return new BookingResult(false,
                    "The selected patient does not exist.");
            }

            int validServiceCount = await _db.DiagnosticServices
                .CountAsync(s =>
                    serviceIds.Contains(s.ServiceId) && s.IsActive);

            if (validServiceCount != serviceIds.Count)
            {
                return new BookingResult(false,
                    "One or more selected services are invalid or inactive.");
            }

            // Recheck every selected service's capacity.
            foreach (int serviceId in serviceIds)
            {
                var availability = await _db.ServiceAvailabilities
                    .FirstOrDefaultAsync(a =>
                        a.ServiceId == serviceId &&
                        a.AvailableDate == appointmentDate.Date);

                if (availability == null)
                {
                    return new BookingResult(false,
                        "No availability schedule exists for one of the selected services.");
                }

                int bookedCount = await _db.BookingItems
                    .CountAsync(item =>
                        item.ServiceId == serviceId &&
                        item.Booking.AppointmentDate == appointmentDate.Date &&
                        item.Booking.Status == BookingStatus.Confirmed);

                if (bookedCount >= availability.Capacity)
                {
                    return new BookingResult(false,
                        "One or more selected services have reached their daily capacity.");
                }
            }

            var booking = new Booking
            {
                PatientId = patientId,
                AppointmentDate = appointmentDate.Date,
                Status = BookingStatus.Confirmed
            };

            foreach (int serviceId in serviceIds)
            {
                booking.BookingItems.Add(new BookingItem
                {
                    ServiceId = serviceId
                });
            }

            _db.Bookings.Add(booking);

            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            return new BookingResult(
                true,
                "Booking confirmed successfully.",
                booking.BookingId
            );
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
