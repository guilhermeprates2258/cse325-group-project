# CSE 325 Group Project

A .NET Blazor web application built for CSE 325 at Brigham Young University-Idaho, Fall 2026.

## Team Members

- Guilherme Prates Batista
- TEAM MEMBER 2
- TEAM MEMBER 3
- TEAM MEMBER 4

## Project Summary

The group is evaluating four candidate applications and will select one at the W03 meeting. Whichever is chosen, the application will be a Blazor Web App with user authentication and full CRUD functionality, deployed to a cloud service.

Candidates under consideration:

1. CourtTime - a booking system for small sports facilities (beach tennis, padel, futsal).
2. MentorLink - a peer mentoring session scheduler for online learning programs.
3. ServeHours - a volunteer opportunity and service hours tracker for small nonprofits.
4. JobDesk - a job and invoice tracker for one-to-five person service businesses.

## Technology Stack

- .NET 9 / Blazor Web App (interactive server rendering)
- Entity Framework Core with SQL Server
- ASP.NET Core Identity for user authentication
- Deployed to Azure App Service

## Getting Started

    git clone https://github.com/guilhermeprates2258/cse325-group-project.git
    cd cse325-group-project
    dotnet restore
    dotnet run

The application runs at https://localhost:5001 by default.

## Branching Model

- `main` stays deployable at all times.
- Each member works on a feature branch named `feature/short-description`.
- Changes reach `main` through a pull request reviewed by at least one teammate.

## Project Management

Tasks are tracked on our Trello board. The board URL is listed in the course submission document.

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
