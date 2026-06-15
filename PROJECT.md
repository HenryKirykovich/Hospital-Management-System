# Hospital Management System

A full-featured hospital management application built with **C# .NET 8**, **ASP.NET Core**, **MongoDB**, **SignalR**, and **Windows Forms**.

---

## Technology Stack

```
┌─────────────────────────────────────────────────────────────────────┐
│                        TECHNOLOGY STACK                             │
├──────────────────────┬──────────────────────────────────────────────┤
│  Client (WinForms)   │  .NET 8 Windows Forms                        │
│  Server (API)        │  ASP.NET Core (.NET 10)                      │
│  Shared (Models)     │  .NET 8 Class Library                        │
│  Database            │  MongoDB (localhost:27017)                   │
│  Real-time           │  SignalR                                     │
│  Authentication      │  JWT Bearer Tokens (HS256, 8h expiry)        │
│  Password Hashing    │  BCrypt (work factor 12)                     │
│  HTTP Client         │  System.Net.Http + System.Text.Json          │
└──────────────────────┴──────────────────────────────────────────────┘
```

---

## Solution Structure

```
HospitalManagementSystem/
│
├── HospitalManagement.Shared/          ← Models & DTOs shared between Client and Server
│   ├── Models/
│   │   ├── User.cs
│   │   ├── Patient.cs
│   │   ├── Appointment.cs
│   │   ├── InventoryItem.cs
│   │   ├── ChatMessage.cs
│   │   └── PatientVitals.cs
│   └── DTOs/
│       └── AuthDTOs.cs
│
├── HospitalManagement.Server/          ← ASP.NET Core Web API
│   ├── Controllers/
│   ├── Services/
│   ├── Hubs/
│   ├── Data/
│   ├── Settings/
│   ├── Seeds/
│   └── Program.cs
│
└── HospitalManagement.Client/          ← Windows Forms Application
    ├── Forms/
    ├── Services/
    ├── Session/
    ├── Api/
    └── Program.cs
```

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────┐
│                       SYSTEM ARCHITECTURE                           │
│                                                                     │
│  ┌─────────────────────────┐         ┌─────────────────────────┐   │
│  │   WinForms Client       │         │   ASP.NET Core Server   │   │
│  │   (.NET 8 Windows)      │         │   (.NET 10)             │   │
│  │                         │  HTTP   │                         │   │
│  │  ┌─────────────────┐   │◄───────►│  ┌──────────────────┐  │   │
│  │  │   LoginForm     │   │  REST   │  │  AuthController  │  │   │
│  │  │   RegisterForm  │   │  JSON   │  │  PatientCtrl     │  │   │
│  │  │   MainForm      │   │         │  │  AppointmentCtrl │  │   │
│  │  │   PatientList   │   │         │  │  InventoryCtrl   │  │   │
│  │  │   ApptList      │   │  WS     │  │  ChatController  │  │   │
│  │  │   InventoryList │   │◄───────►│  │                  │  │   │
│  │  │   ChatForm      │   │ SignalR │  │  HospitalHub     │  │   │
│  │  └─────────────────┘   │         │  └──────────────────┘  │   │
│  │                         │         │           │            │   │
│  │  ┌─────────────────┐   │         │  ┌────────▼──────────┐ │   │
│  │  │  ClientSession  │   │         │  │   MongoDbContext   │ │   │
│  │  │  ApiClient      │   │         │  │   (Singleton)     │ │   │
│  │  │  SignalRService │   │         │  └──────────┬────────┘ │   │
│  │  └─────────────────┘   │         │             │          │   │
│  └─────────────────────────┘         └─────────────┼─────────┘   │
│                                                     │             │
│                                       ┌─────────────▼─────────┐   │
│                                       │   MongoDB Database     │   │
│                                       │   Hospital-Management  │   │
│                                       │                       │   │
│                                       │   users               │   │
│                                       │   patients            │   │
│                                       │   appointments        │   │
│                                       │   inventory           │   │
│                                       │   chat_messages       │   │
│                                       │   vitals              │   │
│                                       └───────────────────────┘   │
└─────────────────────────────────────────────────────────────────────┘
```

---

## MongoDB Collections & Indexes

```
Hospital-Management-System (database)
│
├── users
│   ├── _id (ObjectId)
│   ├── username  ◄── UNIQUE INDEX
│   ├── email     ◄── UNIQUE INDEX
│   ├── passwordHash (BCrypt)
│   ├── role: Admin | Doctor | Nurse | Patient
│   ├── fullName, isActive, createdAt, lastLogin
│
├── patients
│   ├── _id, userId (ref → users)
│   ├── firstName, lastName, dateOfBirth, gender
│   ├── phone, email, address, bloodType
│   ├── allergies: [ string ]
│   └── medicalHistory: [ { date, diagnosis, treatment, doctorId, notes } ]
│
├── appointments
│   ├── _id, patientId, doctorId
│   ├── patientName, doctorName (denormalized)
│   ├── scheduledAt ◄── INDEX (doctorId + scheduledAt)
│   ├── durationMinutes, reason, notes
│   └── status: Scheduled | Confirmed | Cancelled | Completed | NoShow
│
├── inventory
│   ├── _id, name, category: Medication | Supply | Equipment
│   ├── quantity, unit, lowStockThreshold
│   ├── unitPrice, supplier, expiryDate
│   └── IsLowStock (computed: quantity <= lowStockThreshold)
│
├── chat_messages
│   ├── _id, senderId, senderName
│   ├── receiverId, channel ◄── INDEX (channel + sentAt)
│   ├── content, sentAt, isRead
│
└── vitals
    ├── _id, patientId ◄── INDEX (patientId + recordedAt)
    ├── patientName, roomNumber
    ├── heartRate, bloodPressureSystolic, bloodPressureDiastolic
    ├── temperature, oxygenSaturation
    └── isCritical, recordedAt
```

---

## User Roles & Permissions

```
┌──────────────────────────────────────────────────────────────────┐
│                     ROLE-BASED ACCESS                            │
├────────────┬──────────┬──────────┬──────────┬───────────────────┤
│ Feature    │  Admin   │  Doctor  │  Nurse   │  Patient          │
├────────────┼──────────┼──────────┼──────────┼───────────────────┤
│ Login      │   ✓      │   ✓      │   ✓      │   ✓               │
│ Register   │   ✓      │   ✓      │   ✓      │   ✓               │
│ View Pat.  │   ✓      │   ✓      │   ✓      │   own only        │
│ Add Pat.   │   ✓      │   ✓      │   ✓      │   ✗               │
│ Edit Pat.  │   ✓      │   ✓      │   ✓      │   ✗               │
│ Delete Pat.│   ✓      │   ✓      │   ✗      │   ✗               │
│ Med. Hist. │   ✓      │   ✓      │   view   │   own only        │
│ Add Appt.  │   ✓      │   ✓      │   ✓      │   ✗               │
│ Del Appt.  │   ✓      │   ✓      │   ✗      │   ✗               │
│ Inventory  │   ✓      │   ✓      │   view   │   ✗               │
│ Chat       │   ✓      │   ✓      │   ✓      │   ✓               │
└────────────┴──────────┴──────────┴──────────┴───────────────────┘
```

---

## SignalR Real-Time Events

```
Server → All Clients (broadcast):
  AppointmentCreated      → AppointmentListForm refreshes
  AppointmentUpdated      → AppointmentListForm updates row
  AppointmentStatusChanged→ Row color changes + notification bar
  AppointmentDeleted      → Row removed from grid
  InventoryUpdated        → InventoryListForm refreshes quantity
  LowStockAlert           → Alert popup for Admin/Doctor/Nurse
  ChatMessage             → ChatForm receives message
  VitalsUpdate            → Dashboard vitals panel updates
  Notification            → General notification bar
```

---

## How to Run

### Prerequisites
- .NET 8 SDK (Client & Shared)
- .NET 10 SDK (Server)
- MongoDB running on `localhost:27017`

### Start the Server
```bash
cd HospitalManagement.Server
dotnet run
# Server starts at: http://localhost:5066
```

### Seed the Database (first time or reset)
```bash
# After server is running, call the reseed endpoint:
curl -X POST http://localhost:5066/api/dev/reseed
```

### Start the Client
```bash
cd HospitalManagement.Client
dotnet run
```

### Default Test Accounts (after seeding)

| Role    | Username          | Password      |
|---------|-------------------|---------------|
| Admin   | `admin`           | `Password123!`|
| Doctor  | `dr.smith`        | `Password123!`|
| Doctor  | `dr.johnson`      | `Password123!`|
| Doctor  | `dr.williams`     | `Password123!`|
| Doctor  | `dr.brown`        | `Password123!`|
| Nurse   | `nurse.davis`     | `Password123!`|
| Nurse   | `nurse.wilson`    | `Password123!`|
| Patient | `patient.jones`   | `Password123!`|
| Patient | `patient.taylor`  | `Password123!`|

---

## Implementation Stages

```
┌─────────────────────────────────────────────────────────────────┐
│                    DEVELOPMENT ROADMAP                          │
│                                                                 │
│  [✓] Stage 1  ──►  [✓] Stage 2  ──►  [✓] Stage 3              │
│  Foundation        Auth               Patients                  │
│       │                                    │                    │
│       └────────────────────────────────────┘                    │
│                          │                                      │
│                          ▼                                      │
│  [✓] Stage 4  ──►  [ ] Stage 5  ──►  [ ] Stage 6              │
│  Appointments       Inventory          Analytics +              │
│  + SignalR          + Alerts           Dashboard                │
│                          │                                      │
│                          ▼                                      │
│                    [ ] Stage 7                                  │
│                    Real-time Chat +                             │
│                    Vitals Monitor                               │
└─────────────────────────────────────────────────────────────────┘
```

---

### ✅ Stage 1 — Project Foundation
**Status: COMPLETE** | Commit: `Stage 1`

**What was done:**
- Upgraded `HospitalManagement.Client` from .NET Framework 4.7.2 → **net8.0-windows WinForms**
- Updated `HospitalManagement.Shared` to **net8.0** (compatible with both Client and Server)
- Added all project references (Client→Shared, Server→Shared)
- Installed NuGet packages: `MongoDB.Driver`, `BCrypt.Net-Next`, `JwtBearer`, `SignalR.Client`
- Configured MongoDB connection with `MongoDbContext` (auto-creates indexes on startup)
- Configured JWT authentication in `Program.cs`
- Added CORS policy for WinForms client

**Key files to review:**
```
HospitalManagement.Shared/
  Models/User.cs                  ← User model with roles
  Models/Patient.cs               ← Patient with embedded MedicalHistory[]
  Models/Appointment.cs           ← Appointment with status workflow
  Models/InventoryItem.cs         ← Inventory with IsLowStock computed flag
  Models/ChatMessage.cs           ← Chat message with channel routing
  Models/PatientVitals.cs         ← Real-time vitals model
  DTOs/AuthDTOs.cs                ← Login/Register/AuthResponse DTOs

HospitalManagement.Server/
  Data/MongoDbContext.cs          ← MongoDB singleton, all collections, auto-indexes
  Settings/MongoDbSettings.cs    ← Strongly-typed config from appsettings.json
  appsettings.json                ← MongoDB connection + JWT secret
  Program.cs                      ← All middleware registration
```

---

### ✅ Stage 2 — Authentication & JWT
**Status: COMPLETE** | Commit: `Stage 2`

**What was done:**
- `AuthService`: BCrypt password hashing (work factor 12), JWT token generation (8h, role claims)
- `AuthController`: `POST /api/auth/register`, `POST /api/auth/login`
- `ClientSession`: static class holding JWT token, userId, role in memory
- `ApiClient`: central HTTP wrapper that auto-attaches Bearer token to every request
- `LoginForm`: username/password, validation, error display
- `RegisterForm`: full registration with role selection
- `MainForm`: shell with header, sidebar, logout button

**Key files to review:**
```
HospitalManagement.Server/
  Services/IAuthService.cs        ← Contract
  Services/AuthService.cs         ← BCrypt + JWT implementation
  Controllers/AuthController.cs   ← POST /api/auth/register & /login

HospitalManagement.Client/
  Session/ClientSession.cs        ← Token storage + role helpers (IsAdmin, IsDoctor...)
  Api/ApiClient.cs                ← HTTP client, auto-attaches JWT
  Forms/LoginForm.cs              ← Login UI
  Forms/RegisterForm.cs           ← Registration UI
  Forms/MainForm.cs               ← App shell with sidebar navigation
```

**Authentication Flow:**
```
User enters credentials
        │
        ▼
POST /api/auth/login
        │
        ▼
AuthService.LoginAsync()
  ├── Find user by username
  ├── BCrypt.Verify(password, hash)
  └── Build JWT with claims:
        sub, username, email, role, fullName
        │
        ▼
JWT Token returned
        │
        ▼
ClientSession.SetSession(response)
  ├── Token stored in memory
  └── ApiClient auto-attaches on every request
```

---

### ✅ Stage 3 — Patient Management (CRUD)
**Status: COMPLETE** | Commit: `Stage 3`

**What was done:**
- `PatientService`: GetAll, GetById, Search (regex), Create, Update, SoftDelete, AddMedicalRecord
- `PatientController`: full CRUD with role-based access
- `PatientListForm`: DataGridView with debounced search (400ms), color rows
- `PatientEditForm`: all fields with validation (name, DOB, gender, blood type, allergies)
- `MedicalHistoryForm`: view history + add new records (Doctors/Admins only)
- Sidebar navigation wired in MainForm

**Key files to review:**
```
HospitalManagement.Server/
  Services/PatientService.cs      ← MongoDB CRUD, regex search, soft-delete
  Controllers/PatientController.cs← GET/POST/PUT/DELETE + /medical-record endpoint

HospitalManagement.Client/
  Forms/PatientListForm.cs        ← Main list with search + debounce timer
  Forms/PatientEditForm.cs        ← Add/Edit form with validation
  Forms/MedicalHistoryForm.cs     ← Patient history viewer + record entry
```

**API Endpoints:**
```
GET    /api/patient                 → All active patients
GET    /api/patient/{id}            → Single patient
GET    /api/patient/search?q=...    → Regex search
POST   /api/patient                 → Create (Admin/Doctor/Nurse)
PUT    /api/patient/{id}            → Update (Admin/Doctor/Nurse)
DELETE /api/patient/{id}            → Soft-delete (Admin/Doctor)
POST   /api/patient/{id}/medical-record → Add to history (Admin/Doctor)
```

---

### ✅ Stage 4 — Appointment Scheduling + SignalR
**Status: COMPLETE** | Commit: `Stage 4`

**What was done:**
- `HospitalHub`: SignalR hub with role-based groups, JoinGroup/LeaveGroup
- `AppointmentService` + `AppointmentController`: full CRUD + `PATCH /status`
- Every Create/Update/Delete/StatusChange **broadcasts to all connected clients**
- `SignalRService` (Client): manages HubConnection, auto-reconnect, fires C# events
- `AppServices`: singleton holder for SignalRService (created after login, disposed on logout)
- `AppointmentListForm`: real-time updates without manual refresh + color-coded status rows
- `AppointmentEditForm`: patient/doctor dropdowns with manual fallback
- `StatusPickerDialog`: change appointment status

**Key files to review:**
```
HospitalManagement.Server/
  Hubs/HospitalHub.cs             ← SignalR hub with role groups
  Services/AppointmentService.cs  ← MongoDB + UpdateStatusAsync
  Controllers/AppointmentController.cs ← All endpoints + SignalR broadcast

HospitalManagement.Client/
  Services/SignalRService.cs      ← HubConnection management + event wiring
  AppServices.cs                  ← Singleton holder, Init/Dispose lifecycle
  Forms/AppointmentListForm.cs    ← Real-time DataGridView with notification bar
  Forms/AppointmentEditForm.cs    ← Add/Edit form
  Forms/StatusPickerDialog.cs     ← Status change dialog
```

**SignalR Flow:**
```
User changes appointment status
        │
        ▼
PATCH /api/appointment/{id}/status
        │
        ▼
AppointmentService.UpdateStatusAsync()
        │
        ▼
IHubContext<HospitalHub>.Clients.All
  .SendAsync("AppointmentStatusChanged", { id, status })
        │
        ├──► Client A: AppointmentListForm row color changes
        ├──► Client B: notification bar shows "Status changed to: Confirmed"
        └──► Client C: same update instantly
```

**Appointment Status Colors:**
```
Scheduled  → White
Confirmed  → Green  (#DCFFDC)
Cancelled  → Red    (#FFDCDC)
Completed  → Blue   (#DCDCFF)
NoShow     → Yellow (#FFF0C8)
```

---

### ⬜ Stage 5 — Medical Inventory Management
**Status: IN PROGRESS**

**Planned:**
- `InventoryService` + `InventoryController`: CRUD for medications/supplies/equipment
- `PATCH /api/inventory/{id}/restock`: add stock quantity
- SignalR `LowStockAlert` broadcast when quantity drops below threshold
- `InventoryListForm`: DataGridView with red highlighting for low-stock items
- `InventoryEditForm`: add/edit items with threshold and expiry date
- Real-time stock level updates for all connected staff

**Key files (to be created):**
```
HospitalManagement.Server/
  Services/InventoryService.cs
  Controllers/InventoryController.cs

HospitalManagement.Client/
  Forms/InventoryListForm.cs
  Forms/InventoryEditForm.cs
```

---

### ⬜ Stage 6 — Analytics, Reports & Dashboard
**Status: NOT STARTED**

**Planned:**
- `AnalyticsController`: aggregate MongoDB queries for statistics
- Report: patient visits per period
- Report: most common diagnoses
- Report: medication usage trends
- Report: doctor workload
- `DashboardForm`: live stats panel with SignalR updates
- Bed availability display
- Emergency status board

---

### ⬜ Stage 7 — Real-time Communication
**Status: NOT STARTED**

**Planned:**
- `ChatController`: save/load messages from `chat_messages` collection
- `ChatForm` (WinForms): real-time chat between staff and patients
- Channel-based messaging (general, emergency, private patient channels)
- Patient vitals monitoring: `VitalsForm` with live updating charts
- Push notifications for emergency events via SignalR

---

## Git History Summary

| Commit | Stage | Description |
|--------|-------|-------------|
| `9924075` | Stage 1 | Project foundation — .NET 8, MongoDB, models, JWT config |
| `f2b51ff` | Stage 2 | Authentication — BCrypt, JWT, LoginForm, RegisterForm |
| `9490a7c` | Stage 3 | Patient management — CRUD, DataGridView, MedicalHistory |
| `bbb0d85` | Fix     | ApiClient BaseUrl set to http://localhost:5066 |
| `fc8a24a` | Fix     | Add Patient button moved to toolbar, role-based visibility |
| `53e4178` | Stage 4 | Appointments + SignalR real-time notifications |
| `0de2b34` | Fix     | BsonObjectId removed from FK fields; DataSeeder added |

---

## Project Summary for Presentation

> **Hospital Management System** is a desktop application built for managing hospital operations.
> It uses a **client-server architecture** where a Windows Forms desktop app communicates
> with an ASP.NET Core REST API backed by MongoDB.
>
> Key technical highlights:
> - **Real-time updates** via SignalR WebSockets — appointment changes appear instantly on all screens
> - **Role-based security** — Admin, Doctor, Nurse, and Patient each see different features
> - **JWT authentication** — stateless, token-based auth with 8-hour expiry
> - **MongoDB** — document database storing nested medical history inside patient records
> - **BCrypt** — passwords are hashed with work factor 12 (industry standard)
> - **Soft delete** — patients are never permanently removed; data is retained for audit
