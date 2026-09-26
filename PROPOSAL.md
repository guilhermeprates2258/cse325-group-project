# CourtTime — Project Proposal

CSE 325 .NET Software Development · Brigham Young University-Idaho · Fall 2026
W03 Team Activity: Project Checkpoint

## 1. Title

**CourtTime: Court Booking for Small Sports Facilities**

## 2. Project Overview

Small sports facilities such as beach tennis arenas, padel clubs, and futsal courts still manage most of their bookings through WhatsApp messages, phone calls, and paper or spreadsheet schedules. The owner or a staff member has to answer every message, check availability by hand, and remember who paid. This leads to double bookings, empty time slots that nobody knew were free, and hours spent every week on repetitive back-and-forth. CourtTime replaces that process with a simple web application where players see real-time court availability and reserve a slot themselves, while the facility keeps one reliable schedule.

The application has two kinds of users. Players (recreational athletes, students, and groups of friends) use CourtTime to find a free court at a time that works for them and to manage their own reservations. Facility managers (owners or front-desk staff of a small facility with one to ten courts) use it to register their courts, set prices and opening hours, block time for classes or maintenance, and see the day's schedule at a glance, including which bookings are paid.

The idea is valuable because it targets a real and very common gap: large booking platforms are built for big clubs and charge monthly fees, while small facilities are left with manual tools. CourtTime is focused, free of unnecessary complexity, and built around one core workflow (see availability, book, show up), which makes it a realistic semester project that still exercises every major part of the .NET stack: Blazor components, Entity Framework Core, ASP.NET Core Identity with roles, validation, and cloud deployment. One member of the group has direct contact with local beach tennis facilities, which gives us a real use case to design and test against.

## 3. Project Scope

### What's IN

- User registration and login with two roles: Player and Facility Manager
- Facility and court management (create, read, update, delete) by managers
- Availability calendar per court and per day, showing free and taken time slots
- Booking a time slot, with server-side prevention of double bookings
- "My Bookings" page where players view, reschedule, or cancel their reservations
- Manager daily schedule view, with the ability to block slots and mark bookings as paid or no-show
- Basic user profile (name, phone, preferred sport)
- Responsive, accessible (WCAG 2.1 AA) interface with consistent branding
- Deployment to Azure App Service with an Azure SQL database

### What's OUT

- Online payments (payment status is tracked manually by the manager)
- Native iOS or Android apps (the web app is responsive instead)
- SMS, WhatsApp, or push notifications
- Tournaments, leagues, rankings, or player matchmaking
- Recurring or subscription bookings (for example, "every Tuesday at 7 pm")
- Reviews and ratings of facilities
- Multi-language support
- A public marketplace across many cities (the focus is one facility's own booking page)

## 4. App Features

| # | Feature (user action) | Needs | User story |
|---|---|---|---|
| 1 | Users can create an account and log in as a Player or Facility Manager | Database, authentication | As a player, I want to create an account so that my bookings are saved under my name. |
| 2 | Managers can add, edit, and remove courts (name, sport, surface, hourly price, opening hours) | Database, authorization (Manager role) | As a facility manager, I want to register my courts and prices so that players see accurate information. |
| 3 | Players can view a court's availability calendar for any day | Database | As a player, I want to see which time slots are free today so that I can pick a time without messaging the facility. |
| 4 | Players can book an available time slot | Database, authentication, validation | As a player, I want to reserve a court in a few clicks so that the slot is guaranteed for my group. |
| 5 | Players can view, reschedule, and cancel their own bookings | Database, authentication, ownership check | As a player, I want to cancel a booking when plans change so that the court is freed for someone else. |
| 6 | Managers can view the daily schedule and block slots for classes or maintenance | Database, authorization (Manager role) | As a facility manager, I want to see all of today's bookings in one screen so that I know who is coming and when. |
| 7 | Managers can mark a booking as paid or no-show | Database, authorization (Manager role) | As a facility manager, I want to record who paid so that I no longer track payments in a notebook. |
| 8 | Users can edit their profile (name, phone, preferred sport) | Database, authentication | As a player, I want to keep my phone number up to date so that the facility can reach me if something changes. |

Each feature above is a card on the team Trello board, with its description and user story. Additional cards cover setup and delivery work (project scaffolding, database design, deployment, accessibility review, and the demonstration video).

## 5. Technical Considerations

**Data storage.** CourtTime stores its data in a relational database accessed through Entity Framework Core (SQL Server locally, Azure SQL in production). The main entities are:

- `ApplicationUser` — Identity user extended with full name, phone, and preferred sport
- `Facility` — name, address, contact phone, and the manager who owns it
- `Court` — facility, name, sport type, surface, hourly price, opening and closing time, active flag
- `Booking` — court, player, start and end time, status (Confirmed, Cancelled, Completed, NoShow), payment status, created date
- `BlockedSlot` — court, start and end time, reason (class, maintenance, event)

**User accounts.** Yes. ASP.NET Core Identity handles registration, login, logout, and password hashing. Two roles control access: Players can browse and manage their own bookings; Facility Managers can manage courts, block slots, and update booking status. Anonymous visitors can view courts and availability but must log in to book.

**External services.** The minimum viable product does not depend on any third-party API. Azure App Service hosts the application and Azure SQL hosts the database. If time allows, an email service (such as SendGrid) could send booking confirmations; this is a stretch goal, not part of the core scope.

**Device compatibility.** CourtTime is a responsive Blazor Web App built with Bootstrap, so it works on phones, tablets, and desktops. Phone use is the priority for players, since most bookings will be made on the go; the manager schedule view is optimized for tablet and desktop.

**Basic security.**

- Passwords are hashed and managed by ASP.NET Core Identity; no passwords are stored in plain text.
- Pages and actions are protected with `[Authorize]` and role checks; the server verifies that a player can only change their own bookings and a manager can only change their own facility's courts.
- All input is validated on the server with data annotations, not only in the browser.
- Entity Framework Core uses parameterized queries, which prevents SQL injection.
- The site is served over HTTPS only, with antiforgery protection on forms.
- Connection strings and secrets are kept out of source control (User Secrets locally, App Service configuration in production).
- Double bookings are prevented by checking for overlapping bookings and blocked slots inside the same database transaction that saves the new booking.

## 6. Project Links

- **GitHub Repository:** https://github.com/guilhermeprates2258/cse325-group-project
- **Trello Board:** https://trello.com/b/xDjtmoAJ/cse-325-group-project

Both links are public for instructor review, and every team member is a collaborator on the GitHub repository.
