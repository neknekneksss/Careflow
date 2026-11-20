
using CareFlow.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CareFlow.Data;

public static class DbSeeder
{
    public static void Seed(CareFlowDbContext db)
    {
        // Add fictional patients if the table is empty.
        if (!db.Patients.Any())
        {
            db.Patients.AddRange(
                new Patient
                {
                    FullName = "Maria Santos",
                    ContactNumber = "09171234567",
                    Email = "maria.santos@example.com"
                },
                new Patient
                {
                    FullName = "Juan Dela Cruz",
                    ContactNumber = "09181234567",
                    Email = "juan.delacruz@example.com"
                },
                new Patient
                {
                    FullName = "Angela Reyes",
                    ContactNumber = "09191234567",
                    Email = "angela.reyes@example.com"
                },
                new Patient
                {
                    FullName = "Mark Garcia",
                    ContactNumber = "09201234567",
                    Email = "mark.garcia@example.com"
                }
            );

            db.SaveChanges();
        }

        // Add diagnostic services if the table is empty.
        if (!db.DiagnosticServices.Any())
        {
            db.DiagnosticServices.AddRange(
                new DiagnosticService
                {
                    Name = "Complete Blood Count (CBC)",
                    Description = "Measures blood cell components."
                },
                new DiagnosticService
                {
                    Name = "Urinalysis",
                    Description = "Examines urine for health indicators."
                },
                new DiagnosticService
                {
                    Name = "Chest X-ray",
                    Description = "Produces images of the chest."
                },
                new DiagnosticService
                {
                    Name = "Electrocardiogram (ECG)",
                    Description = "Records the electrical activity of the heart."
                },
                new DiagnosticService
                {
                    Name = "Fasting Blood Sugar (FBS)",
                    Description = "Measures blood glucose after fasting."
                }
            );

            db.SaveChanges();
        }

        // Generate daily capacity records for the next 30 days.
        var capacities = new Dictionary<string, int>
        {
            ["Complete Blood Count (CBC)"] = 10,
            ["Urinalysis"] = 8,
            ["Chest X-ray"] = 5,
            ["Electrocardiogram (ECG)"] = 6,
            ["Fasting Blood Sugar (FBS)"] = 10
        };

        var services = db.DiagnosticServices.ToList();

        for (int day = 0; day < 30; day++)
        {
            DateTime date = DateTime.Today.AddDays(day);

            foreach (var service in services)
            {
                bool exists = db.ServiceAvailabilities.Any(a =>
                    a.ServiceId == service.ServiceId &&
                    a.AvailableDate == date);

                if (!exists && capacities.TryGetValue(service.Name, out int capacity))
                {
                    db.ServiceAvailabilities.Add(new ServiceAvailability
                    {
                        ServiceId = service.ServiceId,
                        AvailableDate = date,
                        Capacity = capacity
                    });
                }
            }
        }

        db.SaveChanges();
    }
}
