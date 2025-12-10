# Hospital Management System (ASP.NET Core 8)

A complete **Hospital Management System** built using **ASP.NET Core 8 Web API**, following **Clean Architecture**. The system includes authentication, authorization, doctors/patients management, appointments, schedules, medical records, payments, OTP reset password flow, and more.

---

## 🚀 Overview

* **Technologies:** ASP.NET Core 8, C#, EF Core, SQL Server
* **Architecture:** Clean Architecture (API / Application / Infrastructure / Domain)
* **Users:** Admin, Doctor, Patient

---

## 🔐 Authentication & Authorization

* Login using **JWT Tokens**.
* **Refresh Token** mechanism.
* **Forget Password + OTP Verification**:

  * User enters email → system sends **6-digit OTP**.
  * OTP is valid for **10 minutes**.
  * After verifying OTP, user can reset password.
* Secure password hashing using **ASP.NET Identity**.
* Role-based authorization: Admin / Doctor / Patient.

---

## 🧑‍⚕️ Admin Features

* Add a single doctor OR upload multiple doctors via **Excel (EPPlus)**.
* System sends an **email automatically** to each doctor with their login credentials.
* Manage:

  * **Specializations** (can also be uploaded via Excel)
  * **Services** (can also be uploaded via Excel)
  * **Branches**
  * **Banners**
  * **Events**
  * **News**
  * **Schedules (Shifts)**
  * **Doctors** (can also be uploaded via Excel)
* Assign doctors to schedules.
* Full CRUD on all entities.

---

## 👨‍⚕️ Doctor Features

* View all patients booked with them in any day.
* View assigned **shifts**.
* Write, update, and view **Medical Records**.
* Mark patient visit as completed.
* Cancel appointments on a future day → system sends cancellation email to affected patients.
* View all canceled appointments.
* Print Medical Record as a **prescription**.
* Each doctor has a specific **visit cost**.

---

## 🧑‍💼 Patient Features

* Register & Login.
* Search doctors by **name / specialization / branch** / **Date** / **Shift**.
* View **events, News, services, branches**, **Specalizations**.
* Book appointments in available doctor **shifts**.
* Online payment via **Paymob** + **cash payment**.
* View all their **medical records**.
* View full **appointment history**.
* Supports OTP forget password flow as well.

---

## ⚙️ Technical Features

* **Serilog** for logging:

  * Logs are written **daily to a file**.
  * Logs are also printed to the **console** in real-time.
* **Transactions** for safe operations.
* ** MiddelWare** => **Global Exception Handler** for unified error responses.
* **Automapper** 
* Clean Architecture layering:

```
Hospital/
├── Hospital.API/             # Controllers, Middleware, Program.cs
├── Hospital.Application/     # DTOs, Interfaces, Service Contracts
├── Hospital.Domain/          # Entities, Enums
├── Hospital.Infrastructure/  # Repos, DbContext, Migrations
└── Hospital.sln
```

---

## 🔁 Reset Password Flow (OTP)

1. User enters email in `/auth/forgot-password`.
2. System sends a **6-digit OTP** valid for **10 minutes**.
3. User enters OTP → `/auth/verify-otp`.
4. If correct, user sets a new password at `/auth/reset-password`.

---

## 📁 Doctors, Specializations, and Services Bulk Upload via Excel

* Supported via **EPPlus (<=4.5.3.3)**.
* Excel includes columns such as:

  * Name, Email, Phone (for doctors)
  * Specializations (for doctors)
  * Branches (for doctors)
  * Services and Events can also be uploaded via Excel for batch creation.
* System auto-creates accounts + sends credentials via email (for doctors).

---

## 💳 Payment

* **Paymob integration** for online card payments.
* Support **cash** payment inside the medical visit.

---

## 🤖 Chatbot Integration

* **Integrated a chatbot using Firework with model:**

* accounts/fireworks/models/**gpt-oss-20b**

- **The chatbot can answer queries about hospital services, Specailization, doctors, Branches,News, Events and more.**

## Flow Example:

- User asks: "How Many Branches in this hospital?"

- Chatbot searches the data and responds.

- Designed for RAG-style knowledge retrieval without storing chat history.

  ---

## 🔧 Requirements

* .NET 8 SDK
* SQL Server
* SMTP settings for email
* Paymob API keys.

---

## 🚀 How to Run

1. Clone repo:

```bash
git clone <repo-url>
```

2. Update `appsettings.json` with:

   * Connection string
   * JWT settings
   * SMTP settings
   * Paymob keys
3. Run migrations:

```bash
dotnet ef database update --startup-project Hospital.API
```

4. Run the API:

```bash
dotnet run --project Hospital.API
```

---

## 📄 License

This project is licensed under the **MIT License**.
