Hur man skapar databasen till det här. 
Så man behöver öppna docker appen, sen gå till dbeaver, klicka shift, ctrl och N för att skapa en ny databas, sen gör man localhost och tar rätt port oftast 3006.
Sen behöver man gå till lösenordet och skriva in lösenordet man ska ha.
Sen går man till driver proprietes tror jag och klickar in allow public key retrival den är satt på false ändra det till true. 
Sen klickar du done eller liknande och sen klickar du på databasen och sen kör du skript 1 aka schemat.

Schema
CREATE DATABASE IF NOT EXISTS holiday_maker;
   USE holiday_maker;
CREATE TABLE Accommodations (
    AccommodationID INT PRIMARY KEY AUTO_INCREMENT,
    Name VARCHAR(100) NOT NULL,
    Location VARCHAR(100) NOT NULL,
    HasPool BOOLEAN NOT NULL,
    HasRestaurant BOOLEAN NOT NULL,
    BeachDistance INT NOT NULL,
    CenterDistance INT NOT NULL,
    Rating DECIMAL(2,1) NOT NULL,
    CONSTRAINT chk_accommodation_beach CHECK (BeachDistance >= 0),
    CONSTRAINT chk_accommodation_center CHECK (CenterDistance >= 0),
    CONSTRAINT chk_accommodation_rating CHECK (Rating BETWEEN 1 AND 5)
);
CREATE TABLE Customers (
    CustomerID INT PRIMARY KEY AUTO_INCREMENT,
    First_name VARCHAR(100) NOT NULL,
    Last_name VARCHAR(100) NOT NULL,
    Email VARCHAR(100),
    Phone VARCHAR(20)
);
CREATE TABLE Rooms (
    RoomID INT PRIMARY KEY AUTO_INCREMENT,
    AccommodationID INT NOT NULL,
    RoomNumber VARCHAR(20) NOT NULL,
    MaxGuests INT NOT NULL,
    PricePerNight DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (AccommodationID)
    REFERENCES Accommodations(AccommodationID),
    CHECK (MaxGuests > 0),
    CHECK (PricePerNight > 0),
    UNIQUE (AccommodationID, RoomNumber)
);
CREATE TABLE Bookings (
BookingID INT PRIMARY KEY AUTO_INCREMENT,
CustomerID INT NOT NULL,
RoomID INT NOT NULL,
CheckInDate DATE NOT NULL,
CheckOutDate DATE NOT NULL,
NumberOfGuests INT NOT NULL,
FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID),
CONSTRAINT chk_booking_dates CHECK (CheckOutDate > CheckInDate),
CONSTRAINT chk_booking_guests CHECK (NumberOfGuests > 0)
);

när du har kört detta skriptet bör du ha 4 tables. 
kör då testdata/test skript

test skript/testdata

USE holiday_maker;
SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE Bookings;
TRUNCATE TABLE Rooms;
TRUNCATE TABLE Accommodations;
TRUNCATE TABLE Customers;
SET FOREIGN_KEY_CHECKS = 1;
INSERT INTO Customers (First_name, Last_name, Email, Phone) VALUES
('Anna', 'Lindqvist', 'anna.lindqvist@example.com', '070-1112233'),
('Erik', 'Johansson', 'erik.johansson@example.com', '070-2223344'),
('Sara', 'Nilsson', 'sara.nilsson@example.com', '070-3334455'),
('Omar', 'Hassan', 'omar.hassan@example.com', '070-4445566'),
('Maja', 'Persson', 'maja.persson@example.com', '070-5556677');
INSERT INTO Accommodations (Name, Location, HasPool, HasRestaurant, BeachDistance, CenterDistance, Rating) VALUES
('Hotel Kantarell', 'Osby', FALSE, TRUE, 45000, 2500, 3.2),
('Hotel Manet', 'Landskrona', FALSE, TRUE, 300, 3500, 3.8),
('Hotel SuperDuperVIP', 'Landskrona', TRUE, TRUE, 1800, 200, 4.8),
('Hotel Marcelo', 'Malmö', TRUE, TRUE, 2500, 300, 4.6),
('Hotel Benga', 'Malmö', TRUE, TRUE, 400, 1500, 4.1);
INSERT INTO Rooms (AccommodationID, RoomNumber, MaxGuests, PricePerNight) VALUES
(1, '101', 1, 590.00),
(1, '102', 2, 790.00),
(1, '103', 2, 790.00),
(1, '104', 4, 1190.00),
(2, '1', 1, 650.00),
(2, '2', 2, 890.00),
(2, '3', 2, 950.00),
(2, '4', 4, 1390.00),
(3, '201', 1, 1800.00),
(3, '202', 2, 2400.00),
(3, '203', 4, 3600.00),
(3, '204', 4, 3900.00),
(4, '301', 1, 1700.00),
(4, '302', 2, 2300.00),
(4, '303', 3, 2900.00),
(4, '304', 4, 3500.00),
(5, '11', 1, 1100.00),
(5, '12', 2, 1500.00),
(5, '13', 2, 1600.00),
(5, '14', 4, 2200.00);
INSERT INTO Bookings (CustomerID, RoomID, CheckInDate, CheckOutDate, NumberOfGuests) VALUES
(1, 1, '2026-12-01', '2026-12-04', 1),
(2, 2, '2026-12-05', '2026-12-08', 2),
(3, 4, '2026-12-10', '2026-12-14', 4),
(4, 5, '2026-12-03', '2026-12-06', 1),
(5, 6, '2026-12-10', '2026-12-13', 2),
(1, 6, '2026-12-13', '2026-12-17', 2),
(2, 8, '2026-12-20', '2026-12-27', 4),
(3, 9, '2026-12-07', '2026-12-09', 1),
(4, 10, '2026-12-10', '2026-12-13', 2),
(5, 10, '2026-12-13', '2026-12-15', 2),
(1, 11, '2026-12-18', '2026-12-23', 3),
(2, 12, '2026-12-24', '2026-12-28', 4),
(3, 13, '2026-12-02', '2026-12-05', 1),
(4, 14, '2026-12-10', '2026-12-13', 2),
(5, 14, '2026-12-13', '2026-12-16', 2),
(1, 14, '2026-12-20', '2026-12-24', 2),
(2, 15, '2026-12-15', '2026-12-18', 3),
(3, 16, '2026-12-23', '2026-12-30', 4),
(4, 17, '2026-12-08', '2026-12-10', 1),
(5, 18, '2026-12-01', '2026-12-05', 2),
(1, 18, '2026-12-12', '2026-12-19', 2),
(2, 19, '2026-12-24', '2026-12-27', 2),
(3, 20, '2026-12-05', '2026-12-09', 4),
(4, 20, '2026-12-09', '2026-12-12', 3),
(5, 20, '2026-12-26', '2026-12-31', 4);

nu bör allt fungera och du bör ha 5 personer i systemet. 
Nu när databasen är uppe så kör du dotnet.run i terminalen

p.s glöm inte ändra lösenordet i db.cs filen
