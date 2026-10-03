# CourtTime — Data Model (ERD)

Entities live in `src/CourtTime/Models` and `src/CourtTime/Data`, and are mapped by `ApplicationDbContext`.
Enums are stored as strings so the database stays readable.

```mermaid
erDiagram
    ApplicationUser ||--o{ Facility : manages
    ApplicationUser ||--o{ Booking : books
    Facility ||--o{ Court : has
    Court ||--o{ Booking : "is reserved by"
    Court ||--o{ BlockedSlot : "is blocked by"

    ApplicationUser {
        string Id PK
        string Email
        string FullName
        string PhoneNumber
        string PreferredSport "BeachTennis | Padel | Futsal"
    }
    Facility {
        int Id PK
        string Name
        string Address
        string ContactPhone
        string ManagerId FK
    }
    Court {
        int Id PK
        int FacilityId FK
        string Name
        string Sport
        string Surface
        decimal HourlyPrice "decimal(10,2)"
        time OpeningTime
        time ClosingTime
        bool IsActive
    }
    Booking {
        int Id PK
        int CourtId FK
        string PlayerId FK
        datetime StartTime
        datetime EndTime
        string Status "Confirmed | Cancelled | Completed | NoShow"
        string PaymentStatus "Pending | Paid"
        datetime CreatedAt
    }
    BlockedSlot {
        int Id PK
        int CourtId FK
        datetime StartTime
        datetime EndTime
        string Reason "Class | Maintenance | Event"
        string Notes
    }
```

## Rules enforced by the database

| Rule | How |
|---|---|
| A booking ends after it starts | Check constraint `CK_Booking_EndAfterStart` |
| A blocked slot ends after it starts | Check constraint `CK_BlockedSlot_EndAfterStart` |
| Fast "bookings for court X on day Y" lookups | Index on `Booking (CourtId, StartTime)` and `BlockedSlot (CourtId, StartTime)` |
| Deleting a facility removes its courts, bookings and blocked slots | Cascade delete Facility → Court → Booking / BlockedSlot |
| Deleting a user never silently deletes facilities or booking history | Restrict delete on `Facility.ManagerId` and `Booking.PlayerId` |

Double bookings are prevented in the booking service (W05) by checking for overlapping
bookings and blocked slots inside the same transaction that saves the new booking.

## Roles

| Role | Can do |
|---|---|
| `Player` | Browse courts, book slots, manage own bookings and profile |
| `FacilityManager` | Manage own facility's courts, block slots, mark bookings as paid / no-show |

Roles are created on startup by `DbInitializer`; users pick their role on the Register page.
