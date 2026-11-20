\# CareFlow – Verification and Testing



\## Testing Overview



The following acceptance criteria from the ITSD81 Performance Task 4 specification were used to verify the CareFlow diagnostic booking management workflow.



\## Acceptance Test Results



| Test ID | Test Description | Expected Result | Result |

|---|---|---|---|

| AC-01 | View diagnostic services and remaining capacity for a selected date. | Remaining capacity is correctly calculated from persisted booking information. | PASS |

| AC-02 | Create a booking containing multiple available diagnostic services for an existing patient. | One confirmed booking is recorded with all selected services. | PASS |

| AC-03 | Submit a booking with a fully booked service or invalid booking information. | The request is rejected without creating incomplete or invalid booking records. | PASS |

| AC-04 | Attempt confirmation using previously displayed availability that has become unavailable. | Availability is revalidated, and the system prevents overbooking. | PASS |

| AC-05 | Verify transactional protection when a required database operation fails before a booking is committed. | No partial booking records or incorrect capacity changes are persisted. | PASS – Source-level evidence |

| AC-06 | Close and restart the application after confirming a booking. | Previously confirmed bookings and their associated services remain available. | PASS |



\## Additional Verification



\- Patient records load successfully from the SQLite database.

\- Active diagnostic services load successfully.

\- Past appointment dates are rejected.

\- Duplicate service selections are rejected.

\- Requests containing fewer than one or more than three services are rejected.

\- Confirmed bookings display the correct patient, appointment date, selected services, and booking reference.

\- Remaining capacity updates after successful bookings.

\- Confirmed booking records persist after application restarts.



\## Database Verification



The application uses SQLite persistence through Entity Framework Core.



Database initialization and migrations are performed during application startup. Fictional patient records, diagnostic services, and daily service availability schedules are seeded when necessary.



\## Conclusion



CareFlow implements the required diagnostic booking workflow, including patient validation, multi-service booking, daily capacity checking, transactional booking confirmation, and retrieval of confirmed bookings.



The application was evaluated against the acceptance criteria specified in the ITSD81 Performance Task 4 requirements.

