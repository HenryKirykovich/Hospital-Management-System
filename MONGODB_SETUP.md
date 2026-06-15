# MongoDB Setup Guide

## Step 1 — Download MongoDB

Go to: **https://www.mongodb.com/try/download/community**

- Version: `8.0` (latest)
- Platform: `Windows`
- Package: `MSI`

Click **Download** and run the installer.

---

## Step 2 — Install

During installation:
- ✅ Keep all default settings
- ✅ **"Install MongoDB as a Service"** — make sure this is checked
- ✅ **"Install MongoDB Compass"** — optional (visual UI tool, recommended)

Click **Install** → **Finish**.

---

## Step 3 — Verify MongoDB is Running

Open PowerShell and run:

```powershell
Get-Service -Name MongoDB
```

You should see:

```
Status   Name
------   ----
Running  MongoDB
```

If status is **Stopped**, start it:

```powershell
Start-Service MongoDB
```

---

## Step 4 — Done

MongoDB runs on `localhost:27017` by default.  
The application connects to it automatically on startup.

The database and all test data are created automatically on first launch.

---

## Default Connection (no changes needed)

```
Host:     localhost
Port:     27017
Database: Hospital-Management-System
```

---

## If MongoDB Compass is Installed (optional)

Connect with this string to browse the data visually:

```
mongodb://localhost:27017
```

Select database: `Hospital-Management-System`
