# RaceDay API

A RESTful ASP.NET Core Web API for managing running, walking, and cycling events.

![.NET Build and Test](https://github.com/KamoMotale19/RaceDAY/actions/workflows/dotnet.yml/badge.svg)

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

name: .NET Build and Test

on:
  push:
    branches: [ "main" ]
  pull_request:
    branches: [ "main" ]
  workflow_dispatch:

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    steps:
    - name: Checkout code
      uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '10.0.x'

    - name: Restore dependencies
      run: dotnet restore Part2/RaceDayAPI.slnx

    - name: Build API
      run: dotnet build Part2/RaceDayAPI.slnx --no-restore --configuration Release

    - name: Build MVC Web App
      run: dotnet build Part3/RaceDayWeb/RaceDayWeb.csproj --configuration Release

    - name: Run API Tests
      run: dotnet test Part2/RaceDayAPI.Tests/RaceDayAPI.Tests.csproj --no-restore --configuration Release

## Part 3: MVC Web Application & Docker

The Part 3 front-end is located in the `Part3/RaceDayWeb` folder. It is an ASP.NET Core MVC application that consumes the Part 2 API.

### Docker Setup
The application is containerized using Docker. To run the application inside a Docker container:

1. Ensure **Docker Desktop** is running.
2. Ensure the **RaceDayAPI** is running locally on port `7295`.
3. Open a terminal in the `Part3/RaceDayWeb` directory.
4. Build the Docker image:
   ```bash
   docker build -t racedayweb .

## to test 
{
  "email": "michael.dlamini@raceday.co.za",
  "password": "RaceDayTest123!"
}

Michael is also organiser

Participant 
thabo.nkosi@example.com and RaceDayTest123!

