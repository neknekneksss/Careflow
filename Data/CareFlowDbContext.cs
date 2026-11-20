
using CareFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace CareFlow.Data;

public class CareFlowDbContext : DbContext
{
    public CareFlowDbContext(DbContextOptions<CareFlowDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients => Set<Patient>();

    public DbSet<DiagnosticService> DiagnosticServices
        => Set<DiagnosticService>();

    public DbSet<Booking> Bookings => Set<Booking>();

    public DbSet<BookingItem> BookingItems => Set<BookingItem>();

    public DbSet<ServiceAvailability> ServiceAvailabilities
        => Set<ServiceAvailability>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DiagnosticService>()
            .HasKey(s => s.ServiceId);

        modelBuilder.Entity<ServiceAvailability>()
            .HasKey(sa => sa.AvailabilityId);

        // Patient → Bookings
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Patient)
            .WithMany(p => p.Bookings)
            .HasForeignKey(b => b.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Booking → BookingItems
        modelBuilder.Entity<BookingItem>()
            .HasOne(bi => bi.Booking)
            .WithMany(b => b.BookingItems)
            .HasForeignKey(bi => bi.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        // DiagnosticService → BookingItems
        modelBuilder.Entity<BookingItem>()
            .HasOne(bi => bi.Service)
            .WithMany(s => s.BookingItems)
            .HasForeignKey(bi => bi.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        // DiagnosticService → ServiceAvailabilities
        modelBuilder.Entity<ServiceAvailability>()
            .HasOne(sa => sa.Service)
            .WithMany(s => s.Availabilities)
            .HasForeignKey(sa => sa.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevent duplicate services in one booking.
        modelBuilder.Entity<BookingItem>()
            .HasIndex(bi => new { bi.BookingId, bi.ServiceId })
            .IsUnique();

        // Only one availability record per service per date.
        modelBuilder.Entity<ServiceAvailability>()
            .HasIndex(sa => new { sa.ServiceId, sa.AvailableDate })
            .IsUnique();

        // Capacity must be positive.
        modelBuilder.Entity<ServiceAvailability>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_ServiceAvailability_Capacity",
                "\"Capacity\" > 0"));

        // Store booking status as readable text.
        modelBuilder.Entity<Booking>()
            .Property(b => b.Status)
            .HasConversion<string>();
    }
}
