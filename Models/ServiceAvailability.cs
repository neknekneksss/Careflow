using System;

namespace CareFlow.Models;

public class ServiceAvailability
{
    public int AvailabilityId { get; set; }

    public int ServiceId { get; set; }

    public DateTime AvailableDate { get; set; }

    public int Capacity { get; set; }

    public DiagnosticService Service { get; set; } = null!;
}