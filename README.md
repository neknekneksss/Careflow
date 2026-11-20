\# CareFlow – Diagnostic Booking Management

Submitted by:
Brent Marcus C. Ocaya
Darryl Macarandan

BSIT 3C

\## Overview



CareFlow is a desktop-based diagnostic booking management application for hospital outpatient services. It allows users to create diagnostic appointments for existing patients, check service availability, and view confirmed bookings.



The application enforces daily service capacity limits and ensures that all services included in a booking are available before confirmation.



\## Technologies Used



\- C#

\- .NET 10

\- Avalonia UI

\- XAML

\- MVVM architecture

\- CommunityToolkit.Mvvm

\- Entity Framework Core

\- SQLite

\- Microsoft.Extensions.DependencyInjection



\## Features



\- Select from existing patients.

\- Select an appointment date.

\- View daily diagnostic service availability.

\- Select between one and three diagnostic services.

\- Prevent duplicate service selections.

\- Validate patient and service records.

\- Prevent appointments from being booked in the past.

\- Recheck service capacity during booking confirmation.

\- Save bookings and their selected services atomically.

\- View previously confirmed bookings.

\- Persist data using SQLite.



\## Architecture



CareFlow follows a layered architecture:



1\. \*\*Views\*\* – Avalonia XAML interface.

2\. \*\*ViewModels\*\* – UI state and user interaction through MVVM.

3\. \*\*Services\*\* – Booking operations, validation, availability checks, and transaction coordination.

4\. \*\*Data\*\* – Entity Framework Core database context, database initialization, and seeding.

5\. \*\*Models\*\* – Entities representing patients, services, bookings, booking items, and availability.



The ViewModels communicate with the BookingService rather than accessing the database directly.



\## Database



CareFlow uses SQLite for persistent storage.



The database is automatically created and initialized when the application starts. Entity Framework Core migrations are applied automatically, and fictional patients, diagnostic services, and daily availability records are seeded when necessary.



Default database location:



`%LOCALAPPDATA%\\CareFlow\\careflow.db`



\## Running the Application



1\. Open `CareFlow.slnx` in Visual Studio.

2\. Restore the NuGet packages if necessary.

3\. Build the solution.

4\. Run the application using F5.



The application initializes its database automatically on startup.



\## Resetting the Database



To restore a clean demonstration database:



1\. Close the application.

2\. Navigate to `%LOCALAPPDATA%\\CareFlow`.

3\. Delete `careflow.db`.

4\. Launch the application again.



The database schema and fictional seed data will be recreated automatically.



\*\*Do not delete the Migrations folder.\*\*



\## Project Scope



The application focuses on diagnostic appointment booking and capacity management. Patient registration, diagnostic service administration, authentication, billing, cancellation, and rescheduling are outside the scope of this project.



\## AI Contribution Acknowledgement



AI assistance was used to support implementation, explain programming concepts, troubleshoot errors, and assist with documentation. The project was reviewed, integrated, and tested by the developer.

