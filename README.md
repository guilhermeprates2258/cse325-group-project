# CSE 325 Group Project

A .NET Blazor web application built for CSE 325 at Brigham Young University-Idaho, Fall 2026.

## Team Members

- Guilherme Prates Batista

## Project Summary

**CourtTime** is a booking system for small sports facilities (beach tennis, padel, futsal). Players see real-time court availability and reserve a time slot themselves; facility managers register courts and prices, block slots for classes or maintenance, and track the daily schedule and payments in one place.

The project was selected at the W03 meeting. The full proposal (overview, scope, features, user stories, and technical considerations) is in [PROPOSAL.md](PROPOSAL.md).

## Technology Stack

- .NET 10 (LTS) / Blazor Web App (interactive server rendering)
- Entity Framework Core with SQL Server
- ASP.NET Core Identity for user authentication
- Deployed to Azure App Service

## Getting Started

Requirements: .NET 10 SDK and SQL Server LocalDB (installed with Visual Studio) or another SQL Server instance.

    git clone https://github.com/guilhermeprates2258/cse325-group-project.git
    cd cse325-group-project/src/CourtTime
    dotnet tool install --global dotnet-ef   # once per machine
    dotnet restore
    dotnet ef database update
    dotnet run

On startup the app applies pending migrations and creates the `Player` and `FacilityManager` roles.
The connection string is in `src/CourtTime/appsettings.json` (`DefaultConnection`).

## Project Structure

    src/CourtTime/
      Components/        Blazor pages, layout and Identity (Account) pages
      Data/              ApplicationDbContext, ApplicationUser, Roles, DbInitializer, Migrations
      Models/            Domain entities: Facility, Court, Booking, BlockedSlot
    docs/ERD.md          Data model diagram and database rules

## Branching Model

- `main` stays deployable at all times.
- Each member works on a feature branch named `feature/short-description`.
- Changes reach `main` through a pull request reviewed by at least one teammate.

## Project Management

Features and tasks are tracked on our public Trello board: https://trello.com/b/xDjtmoAJ/cse-325-group-project

## Course Requirements

This project must satisfy the following, per the CSE 325 project description:

- User authentication
- Full CRUD functionality
- Performance, validation, accessibility (WCAG 2.1 Level AA), and usability standards
- Consistent branding and clear navigation
- Code comments and user documentation
- Deployment to a cloud service
- A 5-7 minute group demonstration video

## License

MIT
