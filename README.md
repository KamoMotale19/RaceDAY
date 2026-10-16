
# RaceDay API

A RESTful ASP.NET Core Web API for managing running, walking, and cycling events.

![.NET Build and Test][(https://github.com/KamoMotale19/RaceDAY/actions/workflows/dotnet.yml/badge.svg)](https://github.com/KamoMotale19/RaceDAY/actions/runs/38958437817)

## Overview

- **Part 1** - Planning documents (ERD, API plan, SQL schema) in the `DOCS/` folder
- **Part 2** - ASP.NET Core Web API implementation in the `Part2/` folder

## The Two Roles

### Organiser
- Register and log in as an Organiser
- Create, update, and delete events
- Define categories (distance, entry fee, participant limit)
- Add routes, images, and weather records
- View enrolments for their events
- Capture finish times and positions

Organisers cannot enrol themselves or view participant-only data.

### Participant
- Register and log in as a Participant
- Browse public events and categories
- Enrol in an event by selecting a category
- Manage their own emergency contact and payments
- View their own race results

Participants cannot create events or view other participants' data.

## Tech Stack

- .NET 10 - ASP.NET Core Web API
- Entity Framework Core 10 - Code-First
- SQL Server - production database
- SQLite in-memory - test database
- JWT Bearer authentication
- ASP.NET Identity PasswordHasher
- Swagger / OpenAPI
- xUnit + WebApplicationFactory
- GitHub Actions CI

## Project Structure
