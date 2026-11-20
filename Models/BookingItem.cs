namespace CareFlow.Models;

public class BookingItem
{
    public int BookingItemId { get; set; }

    public int BookingId { get; set; }

    public int ServiceId { get; set; }

    public Booking Booking { get; set; } = null!;

    public DiagnosticService Service { get; set; } = null!;
}