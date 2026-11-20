using System;
using System.Collections.Generic;

namespace CareFlow.Models;

public class Booking
{
    public int BookingId { get; set; }

    public int PatientId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public BookingStatus Status { get; set; }
        = BookingStatus.Pending;

    public Patient Patient { get; set; } = null!;

    public ICollection<BookingItem> BookingItems { get; set; }
        = new List<BookingItem>();
}