
using CareFlow.Models;
using CareFlow.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CareFlow.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly BookingService _bookingService;

    public ObservableCollection<Patient> Patients { get; } = new();

    public ObservableCollection<ServiceOption> Services { get; } = new();

    public ObservableCollection<BookingSummary> ConfirmedBookings { get; } = new();

    [ObservableProperty]
    private Patient? selectedPatient;

    [ObservableProperty]
    private string appointmentDate = DateTime.Today.ToString("yyyy-MM-dd");

    [ObservableProperty]
    private string message = "Loading booking data...";

    public MainViewModel(BookingService bookingService)
    {
        _bookingService = bookingService;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            Patients.Clear();
            Services.Clear();

            var patients = await _bookingService.GetPatientsAsync();
            var services = await _bookingService.GetActiveServicesAsync();

            foreach (var patient in patients)
                Patients.Add(patient);

            foreach (var service in services)
            {
                Services.Add(new ServiceOption
                {
                    ServiceId = service.ServiceId,
                    Name = service.Name,
                    Description = service.Description
                });
            }

            await LoadConfirmedBookingsAsync();

            Message = $"Loaded {Patients.Count} patients and {Services.Count} diagnostic services.";
        }
        catch (Exception ex)
        {
            Message = $"Could not load booking data: {ex.Message}";
        }
    }

    private async Task LoadConfirmedBookingsAsync()
    {
        ConfirmedBookings.Clear();

        var bookings = await _bookingService.GetConfirmedBookingsAsync();

        foreach (var booking in bookings)
            ConfirmedBookings.Add(booking);
    }

    [RelayCommand]
    private async Task ReloadDataAsync()
    {
        await LoadDataAsync();
    }

    [RelayCommand]
    private async Task CheckAvailabilityAsync()
    {
        if (!DateTime.TryParseExact(
            AppointmentDate,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateTime selectedDate))
        {
            Message = "Enter a valid date using YYYY-MM-DD.";
            return;
        }

        if (selectedDate.Date < DateTime.Today)
        {
            Message = "Appointment date cannot be in the past.";
            return;
        }

        try
        {
            var availability = await _bookingService.GetAvailabilityAsync(selectedDate);

            foreach (var service in Services)
            {
                var result = availability.FirstOrDefault(
                    a => a.ServiceId == service.ServiceId);

                service.AvailabilityText = result == null || result.Capacity == 0
                    ? "Unavailable"
                    : $"{result.RemainingCapacity} of {result.Capacity} slots remaining";
            }

            Message = $"Availability updated for {selectedDate:MMMM dd, yyyy}.";
        }
        catch (Exception ex)
        {
            Message = $"Could not check availability: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task CreateBookingAsync()
    {
        if (SelectedPatient == null)
        {
            Message = "Please select a patient.";
            return;
        }

        if (!DateTime.TryParseExact(
            AppointmentDate,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateTime selectedDate))
        {
            Message = "Enter a valid date using YYYY-MM-DD.";
            return;
        }

        List<int> selectedServiceIds = Services
            .Where(s => s.IsSelected)
            .Select(s => s.ServiceId)
            .ToList();

        try
        {
            var result = await _bookingService.CreateBookingAsync(
                SelectedPatient.PatientId,
                selectedDate,
                selectedServiceIds);

            if (!result.Success)
            {
                Message = result.Message;
                return;
            }

            foreach (var service in Services)
                service.IsSelected = false;

            await CheckAvailabilityAsync();
            await LoadConfirmedBookingsAsync();

            Message = $"{result.Message} Booking reference: #{result.BookingId}.";
        }
        catch (Exception ex)
        {
            Message = $"Booking failed. No confirmation was made: {ex.Message}";
        }
    }
}
