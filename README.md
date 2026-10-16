
# RaceDay API

A RESTful ASP.NET Core Web API for managing running, walking, and cycling events — from event creation by organisers through to enrolments and results for participants.

![.NET Build and Test](https://github.com/KamoMotale19/RaceDAY/actions/workflows/dotnet.yml/badge.svg)

## Overview

RaceDay is split into two parts:

- **Part 1** — Planning documents: ERD, API plan, and SQL schema (`DOCS/` folder)
- **Part 2** — Working ASP.NET Core Web API implementation (`Part2/` folder)

## The Two Roles

The system has two distinct user roles, each with a different set of capabilities.

### Organiser

Organisers run events. They can:

- Register and log in with an Organiser role
- Create, update, and delete events they own
- Define categories for each event (distances, entry fees, participant limits)
- Add routes, event images, and weather records for events
- View all enrolments across events they own
- Capture finish times and finishing positions for participants

Organisers **cannot** enrol themselves in events or view participant-only data.

### Participant

Participants take part in events. They can:

- Register and log in with a Participant role
- Browse all public events and categories
- Enrol in an event by selecting a category
- View and manage their own emergency contact details
- View and manage their own payment records
- View their own race results (finish time, position, pace)

Participants **cannot** create events, categories, or routes, and cannot view other participants' data.

## Tech Stack

- **.NET 10** — ASP.NET Core Web API
- **Entity Framework Core 10** — Code-First approach
- **SQL Server** — Production database
- **SQLite (in-memory)** — Integration test database
- **JWT Bearer authentication** — Session management
- **ASP.NET Identity PasswordHasher** — Password hashing
- **Swagger / OpenAPI** — Interactive API documentation
- **xUnit + WebApplicationFactory** — Integration tests
- **GitHub Actions** — Continuous Integration

## Project Structure
