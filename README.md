# CarSharing — Car Rental Desktop App

[![CI](https://github.com/maxmetelitsa/CarSharing/actions/workflows/ci.yml/badge.svg)](https://github.com/maxmetelitsa/CarSharing/actions/workflows/ci.yml)

A WPF desktop application for a car-sharing company: customers browse the car park, filter cars and book them for a period; administrators manage the fleet and track rentals. A rental contract is generated as a Word (.docx) file for every booking.

C# · .NET Framework 4.7.2 · WPF · MVVM · ADO.NET · SQLite · xUnit

> 🇷🇺 Русское описание — [ниже](#русская-версия).

## Features

**Customer**
- Register and sign in.
- Browse the car park (15 cars in four classes) and search by model or brand.
- Filter cars by brand and class and sort them by price.
- Book a car: choose dates, see the price instantly; the app checks that the car is free for those dates and generates the rental contract (.docx).
- Profile: personal data and documents, list of own contracts.

**Administrator**
- Manage the fleet: add, edit and delete cars with photos.
- Track bookings: add, edit and delete them, with the price calculated from the dates.
- Rent a car through the same booking form as customers.

## Architecture

The sign-in, catalog and booking windows follow **MVVM**: the window only binds to a view model and handles navigation; the logic lives in plain C# classes covered by unit tests.

| Layer | Folder | What is there |
| --- | --- | --- |
| View models | `ViewModels/` | `SignInViewModel`, `CarCatalogViewModel`, `RentCarViewModel` |
| MVVM helpers | `Mvvm/` | `ViewModelBase` (INotifyPropertyChanged), `RelayCommand` |
| Business rules | `Domain/` | Rental period and price, booking form validation |
| Data access | `Data/` | Repositories with parameterized SQL; a booking is saved in one transaction |
| Services | `Services/` | Sign-in, contract generation, message boxes behind an interface |
| Security | `Security/` | PBKDF2 (HMAC-SHA256) password hashing with a random salt |

## Getting started

Requirements: Windows, Visual Studio 2022 with the **.NET desktop development** workload.

1. Clone the repository and open `CarSharing.sln`.
2. Restore NuGet packages (right-click the solution → **Restore NuGet Packages**).
3. Press **F5**. The app opens on the sign-in window.

The SQLite database ships with the repository (`CarSharing/CarHouse.db`), so no database server is needed.

### Demo accounts

All people in the database are fictional.

| Role | Email | Password |
| --- | --- | --- |
| Administrator | `admin@carhouse.by` | `Admin1234` |
| Customer | `ivan.petrov@example.com` | `Demo1234` |
| Customer without documents | `sergey.novik@example.com` | `Demo1234` |

Only password hashes are stored. To reset the demo data, run `CarSharing/seed-demo-data.sql` against the database (for example in DB Browser for SQLite).

## Tests

`CarSharing.Tests` is an xUnit project with 56 tests for the business rules, password hashing and the three view models (repositories and dialogs are replaced with fakes). Run them from **Test → Run All Tests** in Visual Studio.

## Project structure

```
CarSharing/
├── ViewModels/, Mvvm/       MVVM layer
├── Domain/                  Business rules
├── Data/                    Repositories (SQL)
├── Services/                Sign-in, contracts, paths, messages
├── Security/                Password hashing
├── Authorization/           Sign-in and registration windows
├── CarPark/                 Car park overview
├── Rent/                    Car catalog with filters and booking
├── Profile/                 Customer profile and contracts
├── Admin/                   Fleet management and rental tracking
├── CompanyInfo/             .docx contract generation (Open XML SDK)
├── CarHouse.db              SQLite database
└── Images/                  Car photos
CarSharing.Tests/            xUnit tests
```

## Project history

**Original version (2023).** I designed and wrote CarSharing myself as a college course project: the idea and feature set, the SQLite database schema, all windows and their design, the booking flow, .docx contract generation, the profile and the admin panel.

**Refactoring (2026).** I later reviewed and improved the code with the help of [Claude](https://claude.ai), an AI assistant. The application itself stayed the same — its features, user interface, window layout and database structure (apart from a new `Administrators` table) are the original ones. What changed:

- the sign-in, catalog and booking windows were moved to MVVM, with SQL moved into repositories and business rules into `Domain/`;
- passwords are now stored as hashes, and credentials were removed from the code;
- a unit test project was added (56 xUnit tests).

The rest of the windows (admin panel, car park management, profile) and the contract generation are the original code.

---

## Русская версия

**CarSharing** — десктопное приложение на WPF для компании каршеринга.

**Возможности клиента:** регистрация и вход; автопарк из 15 машин четырёх классов с поиском; фильтры по марке и классу и сортировка по цене; бронирование на выбранные даты с проверкой, что машина свободна, мгновенным расчётом стоимости и автоматической генерацией договора аренды в Word (.docx); профиль с документами и договорами.

**Возможности администратора:** управление автопарком и фотографиями; учёт бронирований: добавление, изменение и удаление с расчётом стоимости по датам; аренда через ту же форму, что у клиентов.

**Технологии:** C#, .NET Framework 4.7.2, WPF, MVVM, ADO.NET, SQLite, LINQ, Open XML SDK, xUnit.

**История проекта.** Исходную версию (2023) я спроектировал и написал сам как курсовой проект: идею и набор функций, схему базы данных, все окна и их дизайн, бронирование, генерацию договоров, профиль и панель администратора. В 2026 году я доработал код с помощью ИИ-ассистента [Claude](https://claude.ai). Само приложение осталось прежним: функции, интерфейс, окна и структура базы данных (кроме новой таблицы администраторов) — исходные. Изменилось следующее: окна входа, каталога и бронирования переведены на MVVM, SQL вынесен в репозитории; пароли хранятся в виде хешей; добавлены 56 тестов на xUnit. Остальные окна (администратор, управление автопарком, профиль) и генерация договоров — исходный код.

**Запуск:** откройте `CarSharing.sln` в Visual Studio 2022, восстановите NuGet-пакеты и нажмите F5.
Вход администратора: `admin@carhouse.by` / `Admin1234`, клиента: `ivan.petrov@example.com` / `Demo1234`.
