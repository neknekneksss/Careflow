using System;
using System.Collections.Generic;

namespace CareFlow.Models;

public class Patient
{
    public int PatientId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string ContactNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Booking> Bookings { get; set; }
        = new List<Booking>();
}