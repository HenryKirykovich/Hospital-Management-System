# Test Accounts

All accounts use the same password: **`Password123!`**

---

## Admin

| Username | Password | Full Name | Email |
|---|---|---|---|
| `admin` | `Password123!` | System Administrator | admin@hospital.com |

---

## Doctors

| Username | Password | Full Name | Email |
|---|---|---|---|
| `dr.smith` | `Password123!` | Dr. James Smith | j.smith@hospital.com |
| `dr.johnson` | `Password123!` | Dr. Sarah Johnson | s.johnson@hospital.com |
| `dr.williams` | `Password123!` | Dr. Robert Williams | r.williams@hospital.com |
| `dr.brown` | `Password123!` | Dr. Emily Brown | e.brown@hospital.com |

---

## Nurses

| Username | Password | Full Name | Email |
|---|---|---|---|
| `nurse.davis` | `Password123!` | Maria Davis | m.davis@hospital.com |
| `nurse.wilson` | `Password123!` | Thomas Wilson | t.wilson@hospital.com |
| `nurse.moore` | `Password123!` | Laura Moore | l.moore@hospital.com |

---

## Patients

| Username | Password | Full Name | Email |
|---|---|---|---|
| `patient.jones` | `Password123!` | David Jones | d.jones@email.com |
| `patient.taylor` | `Password123!` | Anna Taylor | a.taylor@email.com |
| `patient.anderson` | `Password123!` | Michael Anderson | m.anderson@email.com |
| `patient.thomas` | `Password123!` | Jennifer Thomas | j.thomas@email.com |
| `patient.jackson` | `Password123!` | Charles Jackson | c.jackson@email.com |

---

## Role Permissions

| Role | Patients | Appointments | Inventory | Chat |
|---|---|---|---|---|
| Admin | Full CRUD | Full CRUD | Full CRUD | Yes |
| Doctor | Full CRUD | Full CRUD | Add/Edit/Restock | Yes |
| Nurse | View + Add | View + Add | Add/Edit/Restock | Yes |
| Patient | Own record only | Own appointments | No access | Yes |

---

> To reseed the database with fresh data:
> ```
> Invoke-RestMethod -Method POST -Uri "http://localhost:5066/api/dev/reseed"
> ```
