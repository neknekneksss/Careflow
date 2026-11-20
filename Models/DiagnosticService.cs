using System.Collections.Generic;

namespace CareFlow.Models;

public class DiagnosticService
{
    public int ServiceId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<BookingItem> BookingItems { get; set; }
        = new List<BookingItem>();

    public ICollection<ServiceAvailability> Availabilities { get; set; }
        = new List<ServiceAvailability>();
}