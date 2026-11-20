\# CareFlow – Database Design



\## 1. Database Overview



CareFlow uses a relational SQLite database managed through Entity Framework Core.



The database contains five main tables:



1\. Patients

2\. DiagnosticServices

3\. ServiceAvailabilities

4\. Bookings

5\. BookingItems



These tables represent patient information, diagnostic services, daily service capacity, confirmed bookings, and the services included in each booking.



\## 2. Entity Relationship Diagram



```mermaid

erDiagram

&#x20;   Patients ||--o{ Bookings : has

&#x20;   Bookings ||--|{ BookingItems : contains

&#x20;   DiagnosticServices ||--o{ BookingItems : included\_in

&#x20;   DiagnosticServices ||--o{ ServiceAvailabilities : has



&#x20;   Patients {

&#x20;       int PatientId PK

&#x20;       string FullName

&#x20;       string ContactNumber

&#x20;       string Email

&#x20;       datetime CreatedAt

&#x20;   }



&#x20;   DiagnosticServices {

&#x20;       int ServiceId PK

&#x20;       string Name

&#x20;       string Description

&#x20;       bool IsActive

&#x20;   }



&#x20;   ServiceAvailabilities {

&#x20;       int AvailabilityId PK

&#x20;       int ServiceId FK

&#x20;       datetime AvailableDate

&#x20;       int Capacity

&#x20;   }



&#x20;   Bookings {

&#x20;       int BookingId PK

&#x20;       int PatientId FK

&#x20;       datetime AppointmentDate

&#x20;       datetime CreatedAt

&#x20;       string Status

&#x20;   }



&#x20;   BookingItems {

&#x20;       int BookingItemId PK

&#x20;       int BookingId FK

&#x20;       int ServiceId FK

&#x20;   }

```



\## 3. Entity Descriptions



\### Patients



Stores existing fictional patient records.



| Column | Description |

|---|---|

| PatientId | Primary key |

| FullName | Patient's full name |

| ContactNumber | Contact number |

| Email | Email address |

| CreatedAt | Record creation date |



\### DiagnosticServices



Stores the diagnostic services offered by the hospital.



| Column | Description |

|---|---|

| ServiceId | Primary key |

| Name | Diagnostic service name |

| Description | Service description |

| IsActive | Indicates whether the service is available for booking |



\### ServiceAvailabilities



Stores the daily capacity assigned to each diagnostic service.



| Column | Description |

|---|---|

| AvailabilityId | Primary key |

| ServiceId | Foreign key referencing DiagnosticServices |

| AvailableDate | Date of availability |

| Capacity | Maximum number of bookings for the service on that date |



A unique constraint on ServiceId and AvailableDate prevents duplicate daily availability records for the same service.



A check constraint ensures that capacity is greater than zero.



\### Bookings



Stores confirmed diagnostic appointments.



| Column | Description |

|---|---|

| BookingId | Primary key |

| PatientId | Foreign key referencing Patients |

| AppointmentDate | Scheduled appointment date |

| CreatedAt | Booking creation date |

| Status | Booking status |



\### BookingItems



Connects bookings to their selected diagnostic services.



| Column | Description |

|---|---|

| BookingItemId | Primary key |

| BookingId | Foreign key referencing Bookings |

| ServiceId | Foreign key referencing DiagnosticServices |



A unique constraint on BookingId and ServiceId prevents the same service from being included twice in one booking.



The application also validates that each booking request contains between one and three distinct diagnostic services.



\## 4. Relationships



\- One Patient can have many Bookings.

\- One Booking belongs to one Patient.

\- One Booking can contain multiple BookingItems.

\- Each BookingItem references one DiagnosticService.

\- One DiagnosticService can appear in many BookingItems.

\- One DiagnosticService can have multiple ServiceAvailabilities, each for a different date.



\### Relationship Summary



\- Patients (1) → (Many) Bookings

\- Bookings (1) → (Many) BookingItems

\- DiagnosticServices (1) → (Many) BookingItems

\- DiagnosticServices (1) → (Many) ServiceAvailabilities



\## 5. Capacity Management



Daily capacity is stored in ServiceAvailabilities.



The number of confirmed bookings for a service on a particular date is counted through BookingItems and their associated Bookings.



Remaining capacity is calculated as:



\*\*Remaining Capacity = Daily Capacity − Confirmed Bookings\*\*



Each selected diagnostic service consumes one unit of its daily capacity.



The application checks capacity again during booking confirmation to avoid relying solely on previously displayed availability.



\## 6. Transaction Management



Booking confirmation uses an Entity Framework Core database transaction.



The application:



1\. Validates the patient and selected services.

2\. Checks the availability schedule.

3\. Rechecks the number of confirmed bookings against capacity.

4\. Creates one Booking record.

5\. Creates the associated BookingItem records.

6\. Saves and commits the transaction.



If an exception occurs during the database operation, the transaction is rolled back so that incomplete booking records are not committed.



\## 7. Referential Integrity



Foreign keys maintain relationships between patients, bookings, diagnostic services, and booking items.



Delete restrictions protect patient and service records referenced by existing bookings. BookingItems are configured to cascade when their parent Booking is deleted.



These relationships and constraints help maintain consistent and valid database records.

