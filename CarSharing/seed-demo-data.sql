-- Demo data for CarSharing: fictional people only.
-- Customers sign in with password 'Demo1234', the administrator (admin@carhouse.by) with 'Admin1234'.
-- Passwords are stored as PBKDF2-SHA256 hashes (see Security/PasswordHasher.cs).
BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS Administrators (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL, Surname TEXT NOT NULL, Email TEXT NOT NULL UNIQUE, PasswordHash TEXT NOT NULL);
DELETE FROM Administrators;
DELETE FROM Users;
DELETE FROM [Order];
DELETE FROM UserOrders;
DELETE FROM sqlite_sequence WHERE name IN ('Administrators','Users');
INSERT INTO Administrators (Name, Surname, Email, PasswordHash) VALUES ('Admin', 'Manager', 'admin@carhouse.by', 'PBKDF2$100000$anNbZ8jhj6D2yW37rWLwqQ==$1xBdIuEeiPTesQ384s2RqiclSdIfq1mcFiICHhmJ0aQ=');
INSERT INTO Users (Name, Surname, Email, Password, PassportNumber, IdentificationNumber, LicenseSeries, LicenseNumber) VALUES ('Иван', 'Петров', 'ivan.petrov@example.com', 'PBKDF2$100000$WloAW5wjS0EJCbrYaDh/tA==$J7ojssldVJoqySQ2n64vyAaf0ZwvvZa9x8kjTul4k3Q=', 'MP1234567', '3010190A001PB1', '9AB', '123456');
INSERT INTO Users (Name, Surname, Email, Password, PassportNumber, IdentificationNumber, LicenseSeries, LicenseNumber) VALUES ('Анна', 'Ковалёва', 'anna.kovaleva@example.com', 'PBKDF2$100000$p6Y/atWgGgJ6hvF2GeWw8w==$bs80W2JbXx+3+rQulAqKyX4OaAKojpijWjppybfwEow=', 'MP2345678', '4150592B002PB2', '9AC', '234567');
INSERT INTO Users (Name, Surname, Email, Password, PassportNumber, IdentificationNumber, LicenseSeries, LicenseNumber) VALUES ('Сергей', 'Новик', 'sergey.novik@example.com', 'PBKDF2$100000$jSBl4MaXyoLQpSw7FjFtOw==$t0L3MWKZnxwpu3XSHOWLlAcp/PrCoxRKuCOopGKrkKg=', '', '', '', '');
-- Data fixes: Tesla was stored with the Mercedes-Benz brand, typos and trailing spaces in names.
UPDATE Cars SET Brand = 17 WHERE Name = 'Tesla Model S';
UPDATE EconomyClass SET Brand = 'Tesla' WHERE Number = 'D537OP';
UPDATE BusinessClass SET Brand = 'Tesla' WHERE Number = 'D537OP';
UPDATE Cars SET Name = 'Porsche Cayenne S Coupe' WHERE Name = 'Porsche Caynner S Coupe';
UPDATE Cars SET Name = TRIM(Name);
COMMIT;
