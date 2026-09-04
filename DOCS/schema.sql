
-- RACEDAY DATABASE SCRIPT

USE [master];
GO
IF DB_ID('RaceDayDB') IS NOT NULL
    DROP DATABASE RaceDayDB;
GO
CREATE DATABASE RaceDayDB;
GO
USE RaceDayDB;
GO


-- DROP TABLES 

DROP TABLE IF EXISTS EventImages;
DROP TABLE IF EXISTS Results;
DROP TABLE IF EXISTS Enrolments;
DROP TABLE IF EXISTS Payments;
DROP TABLE IF EXISTS Categories;
DROP TABLE IF EXISTS Events;
DROP TABLE IF EXISTS Routes;
DROP TABLE IF EXISTS Weather;
DROP TABLE IF EXISTS EmergencyContacts;
DROP TABLE IF EXISTS ParticipantProfiles;
DROP TABLE IF EXISTS OrganiserProfiles;
DROP TABLE IF EXISTS Users;
GO


-- CREATE TABLES


-- 1. Users
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    [Role] NVARCHAR(20) NOT NULL CHECK ([Role] IN ('Organiser', 'Participant')),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- 2. OrganiserProfiles
CREATE TABLE OrganiserProfiles (
    ProfileId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL UNIQUE FOREIGN KEY REFERENCES Users(UserId) ON DELETE CASCADE,
    OrganisationName NVARCHAR(200) NOT NULL,
    ContactPhone NVARCHAR(20) NOT NULL,
    Website NVARCHAR(200) NULL
);

-- 3. ParticipantProfiles
CREATE TABLE ParticipantProfiles (
    ProfileId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL UNIQUE FOREIGN KEY REFERENCES Users(UserId) ON DELETE CASCADE,
    DateOfBirth DATE NOT NULL,
    Gender NVARCHAR(10) NOT NULL
);

-- 4. EmergencyContacts
CREATE TABLE EmergencyContacts (
    ContactId INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantId INT NOT NULL FOREIGN KEY REFERENCES ParticipantProfiles(ProfileId) ON DELETE CASCADE,
    ContactName NVARCHAR(100) NOT NULL,
    Relationship NVARCHAR(50) NOT NULL,
    PhoneNumber NVARCHAR(20) NOT NULL
);

-- 5. Routes
CREATE TABLE Routes (
    RouteId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    GpxData NVARCHAR(MAX) NULL,
    MapUrl NVARCHAR(200) NULL,
    ElevationGain INT NULL
);

-- 6. Weather
CREATE TABLE Weather (
    WeatherId INT IDENTITY(1,1) PRIMARY KEY,
    Temperature DECIMAL(5,2) NOT NULL,
    Conditions NVARCHAR(100) NOT NULL,
    ForecastDate DATETIME NOT NULL
);

-- 7. Events
CREATE TABLE Events (
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserId INT NOT NULL FOREIGN KEY REFERENCES OrganiserProfiles(ProfileId),
    RouteId INT NULL FOREIGN KEY REFERENCES Routes(RouteId),
    WeatherId INT NULL FOREIGN KEY REFERENCES Weather(WeatherId),
    Name NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    EventDate DATETIME NOT NULL,
    [Location] NVARCHAR(200) NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- 8. Categories
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL FOREIGN KEY REFERENCES Events(EventId) ON DELETE CASCADE,
    Name NVARCHAR(100) NOT NULL,
    DistanceKm DECIMAL(5,2) NOT NULL,
    EntryFee DECIMAL(10,2) NOT NULL,
    StartTime TIME NOT NULL,
    MaxParticipants INT NOT NULL
);

-- 9. Payments
CREATE TABLE Payments (
    PaymentId INT IDENTITY(1,1) PRIMARY KEY,
    Amount DECIMAL(10,2) NOT NULL,
    PaymentMethod NVARCHAR(50) NOT NULL,
    [Status] NVARCHAR(20) NOT NULL CHECK ([Status] IN ('Pending', 'Paid', 'Failed')),
    TransactionDate DATETIME DEFAULT GETDATE()
);

-- 10. Enrolments
CREATE TABLE Enrolments (
    EnrolmentId INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantId INT NOT NULL FOREIGN KEY REFERENCES ParticipantProfiles(ProfileId),
    CategoryId INT NOT NULL FOREIGN KEY REFERENCES Categories(CategoryId),
    PaymentId INT NULL FOREIGN KEY REFERENCES Payments(PaymentId),
    EnrolmentDate DATETIME DEFAULT GETDATE(),
    BibNumber INT NOT NULL,
    [Status] NVARCHAR(20) NOT NULL CHECK ([Status] IN ('Pending', 'Confirmed', 'Cancelled'))
);

-- 11. Results
CREATE TABLE Results (
    ResultId INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentId INT NOT NULL UNIQUE FOREIGN KEY REFERENCES Enrolments(EnrolmentId) ON DELETE CASCADE,
    FinishTime INT NULL,
    [Position] INT NULL,
    [Status] NVARCHAR(20) NOT NULL CHECK ([Status] IN ('Finished', 'DNF', 'DNS'))
);

-- 12. EventImages
CREATE TABLE EventImages (
    ImageId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL FOREIGN KEY REFERENCES Events(EventId) ON DELETE CASCADE,
    ImageUrl NVARCHAR(200) NOT NULL,
    Caption NVARCHAR(200) NULL
);
GO


-- SEED DATA


-- Insert Users
INSERT INTO Users (Email, PasswordHash, FirstName, LastName, [Role]) VALUES
    ('thabo.organiser@example.com', 'hashed_pw_1', 'Thabo', 'Mbeki', 'Organiser'),
    ('cyril.organiser@example.com', 'hashed_pw_2', 'Cyril', 'Ramaphosa', 'Organiser'),
    ('nelson.participant@example.com', 'hashed_pw_3', 'Nelson', 'Mandela', 'Participant'),
    ('desmond.participant@example.com', 'hashed_pw_4', 'Desmond', 'Tutu', 'Participant');

-- Insert OrganiserProfiles
INSERT INTO OrganiserProfiles (UserId, OrganisationName, ContactPhone, Website) VALUES
    (1, 'Cape Town Athletics', '021-555-1234', 'https://ctathletics.co.za'),
    (2, 'Durban Running Club', '031-555-5678', 'https://durbanrunning.co.za');

-- Insert ParticipantProfiles
INSERT INTO ParticipantProfiles (UserId, DateOfBirth, Gender) VALUES
    (3, '1980-07-18', 'Male'),
    (4, '1985-10-07', 'Male');

-- Insert EmergencyContacts
INSERT INTO EmergencyContacts (ParticipantId, ContactName, Relationship, PhoneNumber) VALUES
    (1, 'Winnie Mandela', 'Spouse', '021-555-9999'),
    (2, 'Leah Tutu', 'Spouse', '031-555-8888');

-- Insert Routes
INSERT INTO Routes (Name, GpxData, MapUrl, ElevationGain) VALUES
    ('Cape Peninsula Route', '<gpx>...</gpx>', 'https://maps.example.com/ctc', 200),
    ('Comrades Up Run', '<gpx>...</gpx>', 'https://maps.example.com/comrades', 500);

-- Insert Weather
INSERT INTO Weather (Temperature, Conditions, ForecastDate) VALUES
    (22.5, 'Sunny', '2026-09-15 08:00:00'),
    (18.0, 'Light Rain', '2026-09-20 06:00:00');

-- Insert Events 
INSERT INTO Events (OrganiserId, RouteId, WeatherId, Name, [Description], EventDate, [Location]) VALUES
    (1, 1, 1, 'Cape Town Cycle Tour 2026', 'Annual cycling event around the Cape Peninsula', '2026-09-15 07:00:00', 'Cape Town'),
    (2, 2, 2, 'Comrades Marathon 2026', 'Ultramarathon from Pietermaritzburg to Durban', '2026-09-20 05:30:00', 'Pietermaritzburg'),
    (1, NULL, NULL, 'Soweto Marathon 2026', 'Popular road race through Soweto', '2026-10-10 06:00:00', 'Soweto');

-- Insert Categories 
INSERT INTO Categories (EventId, Name, DistanceKm, EntryFee, StartTime, MaxParticipants) VALUES
    (1, '5km Fun Ride', 5.0, 50.00, '07:15:00', 500),
    (1, '10km Challenge', 10.0, 80.00, '07:00:00', 300),
    (2, 'Ultramarathon 90km', 90.0, 200.00, '05:30:00', 2000),
    (2, 'Marathon 42.2km', 42.2, 150.00, '05:45:00', 1500),
    (3, '10km Run', 10.0, 60.00, '06:30:00', 800),
    (3, '21km Half Marathon', 21.1, 100.00, '06:00:00', 600);

-- Insert Payments
INSERT INTO Payments (Amount, PaymentMethod, [Status]) VALUES
    (50.00, 'Card', 'Paid'),
    (200.00, 'EFT', 'Pending');

-- Insert Enrolments
INSERT INTO Enrolments (ParticipantId, CategoryId, PaymentId, BibNumber, [Status]) VALUES
    (1, 1, 1, 101, 'Confirmed'),
    (2, 3, 2, 202, 'Pending');

-- Insert Result
INSERT INTO Results (EnrolmentId, FinishTime, [Position], [Status]) VALUES
    (1, 1800, 42, 'Finished');

-- Insert EventImages
INSERT INTO EventImages (EventId, ImageUrl, Caption) VALUES
    (1, 'https://images.example.com/ctc1.jpg', 'Start line at Cape Town'),
    (1, 'https://images.example.com/ctc2.jpg', 'Scenic coastal view'),
    (2, 'https://images.example.com/comrades1.jpg', 'Runners at dawn');
GO
