# 🏦 Loan Application System (Take-Home Technical Assessment)

An end-to-end, enterprise-grade solution for automated loan application processing and decision-making. Built with **.NET 8 Web API**, **Next.js**, **Entity Framework Core**, **SQL Server**, and **xUnit**.

---

## 🏛️ Architecture & Design Patterns

The backend follows **Clean Architecture** and **SOLID** principles, emphasizing low coupling, high testability, and clear separation of concerns:

- **CQRS / Handler Pattern:** Commands (`SubmitApplicationCommand`) and Handlers (`SubmitApplicationCommandHandler`) decouple request handling from HTTP transport.
- **Extensible Rules Engine (`ILoanRule`):** Decision logic is decoupled into independent, isolated rule classes. New business conditions can be added without modifying existing handlers or breaking Open/Closed Principle (OCP).
- **Event-Driven Asynchronous Processing:** Upon application creation, domain events (`ApplicationSavedEvent`) are dispatched asynchronously via `BackgroundService` to notify downstream/mock external services without blocking the HTTP request thread.
- **Repository / DbContext Pattern:** Database operations are managed via Entity Framework Core with SQL Server persistence and migration support.

---

## 🛠️ Tech Stack

- **Backend:** .NET 8 Web API, C#, Entity Framework Core 8, xUnit, Moq, FluentValidation / Regex.
- **Frontend:** Next.js, React, Tailwind CSS, TypeScript.
- **Database:** Microsoft SQL Server / LocalDB.
- **Messaging/Async:** In-Process Event Bus (`System.Threading.Channels` + `BackgroundService`).

---

## ⚙️ Business Rules Matrix (`ILoanRule`)

The system evaluates loan applications against a chain of rules before approval:

| Rule Name | Scope | Condition | Outcome / Failure Message |
| :--- | :--- | :--- | :--- |
| `SsnFormatRule` | Format | SSN does not match `^\d{3}-\d{2}-\d{4}$` | `"Invalid SSN format. Required format: XXX-XX-XXXX"` |
| `BlacklistedSsnRule` | Fraud | SSN is `"000-00-0000"` | `"SSN is blacklisted"` |
| `NyStateRule` | Location | State code is `"NY"` | `"State NY is not allowed"` |
| `MinRequestedAmountRule` | Risk | Requested Amount < `$1,000` | `"Requested amount must be at least $1,000"` |
| `MaxRequestedAmountRule` | Risk | Requested Amount > `$50,000` | `"Requested amount exceeds the maximum limit of $50,000"` |

---

## 🚀 Quick Start Guide

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js (v18+)](https://nodejs.org/)
- [SQL Server / LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)

---

### 1. Database Setup & Migrations

Navigate to the `backend` directory and apply the database migrations:

```bash
cd backend
dotnet ef database update --project LoanApplication.Api.csproj