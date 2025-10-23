# SustainaCore – Full Solution Documentation

The **SustainaCore Solution** is a cross-platform project that integrates:

- A secure **.NET Web API** (ASP.NET Core) for backend logic, authentication, and data storage.
- A **Web Application** (ASP.NET MVC/Razor Pages) for browser-based management and dashboards.
- A **Mobile Application** (MAUI) for cross-platform client access.
- A shared **Core** domain model and infrastructure to ensure a single, maintainable code base.

---

## 📁 Solution Structure

Sustainacore/
│
├── src/
│ ├── Core/
│ │ ├── Sustainacore.Domain/ # Entity models, enums, aggregates
│ │ ├── Sustainacore.Application/ # Business/application logic
│ │ ├── Sustainacore.SharedKernel/ # Shared helpers and interfaces
│ │ └── Sustainacore.Contracts/ # DTOs and API contracts
│ │
│ ├── Infra/
│ │ ├── Sustainacore.Infrastructure/ # Database and external adapters
│ │ ├── Sustainacore.Firebase/ # Firebase admin integration
│ │ └── Sustainacore.Payments/ # Payment processor stubs
│ │
│ ├── Web/
│ │ ├── Sustainacore.Api/ # RESTful backend API (JWT + Firestore)
│ │ └── Sustainacore.Web/ # Frontend web app (MVC + Razor)
│ │
│ ├── Mobile/
│ │ └── Sustainacore.Mobile/ # MAUI mobile client
│ │ ├── Views/ # LoginPage.xaml, DashboardPage.xaml, etc.
│ │ └── Services/ApiBase.cs # Platform-aware API URL
│ │
│ └── Observability/ # Logging and diagnostics
│
├── scripts/
│ ├── run-all.ps1 # Run API + Web
│ └── run-all-with-mobile.ps1 # Run API + Web + Mobile (Windows)
│
└── Directory.Packages.props # Centralized NuGet package versions

yaml
Copy code

---

## ⚙️ Prerequisites

- **.NET 9 SDK**
- **Visual Studio 2022** (with ASP.NET + MAUI workloads)
- **PowerShell 7+**
- **Firebase project**
  - Web API key
  - Service Account JSON (for Admin SDK)
- **Android Emulator / Windows platform tools** (for MAUI)

---

## 🔑 Firebase Setup

Your Firebase configuration is stored in:

`src/Web/Sustainacore.Api/appsettings.Development.json`

Example:
```json
{
  "Firebase": {
    "ProjectId": "sustainacore-69e3e",
    "ApiKey": "YOUR_WEB_API_KEY",
    "AuthBase": "https://identitytoolkit.googleapis.com/v1",
    "TokenIssuer": "https://securetoken.google.com/sustainacore-69e3e",
    "ServiceAccountPath": "../../../firebase-admin/serviceAccount.json"
  }
}
⚠️ Do not commit your serviceAccount.json to GitHub.
Store it locally at src/firebase-admin/serviceAccount.json.

🏗️ First-Time Setup
From repo root:

powershell
Copy code
dotnet restore
dotnet build -c Debug
Check that the solution compiles successfully (no errors).

🌐 Fixed Development Ports
Component	URL	Description
API	http://localhost:5180	Swagger, backend API
Web	http://localhost:5166	MVC web dashboard
Mobile (Windows)	localhost access to API	
Mobile (Android Emulator)	10.0.2.2 (maps to host localhost)	

Configured in each project’s launchSettings.json.

🚀 Running Applications
Option 1 – Run Individually
API

powershell
Copy code
dotnet watch run --project .\src\Web\Sustainacore.Api\Sustainacore.Api.csproj
# Swagger: http://localhost:5180/swagger
Web

powershell
Copy code
dotnet watch run --project .\src\Web\Sustainacore.Web\Sustainacore.Web.csproj
# Dashboard: http://localhost:5166
# Admin:     http://localhost:5166/Admin
Mobile
Choose a target:

Windows

powershell
Copy code
dotnet build -t:Run .\src\Mobile\Sustainacore.Mobile\Sustainacore.Mobile.csproj -f net9.0-windows10.0.19041.0
Android Emulator

powershell
Copy code
dotnet build -t:Run .\src\Mobile\Sustainacore.Mobile\Sustainacore.Mobile.csproj -f net9.0-android
Mac Catalyst/iOS

powershell
Copy code
dotnet build -t:Run .\src\Mobile\Sustainacore.Mobile\Sustainacore.Mobile.csproj -f net9.0-maccatalyst
Option 2 – Run Multiple Together (Recommended)
Run API + Web
powershell
Copy code
pwsh .\scripts\run-all.ps1
Run API + Web + Mobile (Windows)
powershell
Copy code
pwsh .\scripts\run-all-with-mobile.ps1
These scripts:

Start each service on a fixed port.

Open Swagger and the web dashboard automatically.

Run mobile app in a separate minimized window.

📱 Mobile → API Base URL
src/Mobile/Sustainacore.Mobile/Services/ApiBase.cs

csharp
Copy code
namespace Sustainacore.Mobile.Services;

public static class ApiBase
{
#if ANDROID
    public const string Url = "http://10.0.2.2:5180"; // Android emulator loopback
#else
    public const string Url = "http://localhost:5180";
#endif
}
Use ApiBase.Url whenever your MAUI app makes REST calls.

🌍 Cross-Origin Access (CORS)
Added to Program.cs in the API:

csharp
Copy code
var devOrigins = "_devOrigins";
builder.Services.AddCors(opts =>
{
    opts.AddPolicy(devOrigins, p => p
        .WithOrigins(
            "http://localhost:5166", // Web
            "http://localhost",      // Windows WebView
            "http://10.0.2.2:5166"   // Android emulator
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

app.UseCors(devOrigins);
This ensures the web frontend and mobile emulator can call the API without CORS issues.

🧭 Visual Studio Multi-Startup Configuration
To start API and Web together:

In Solution Explorer → right-click the solution → Set Startup Projects.

Choose Multiple startup projects.

Set both:

Sustainacore.Api → Start

Sustainacore.Web → Start

Save and press F5.

Mobile can be run separately (MAUI project launcher).

🧱 Database / Firebase Data Model
Firebase Authentication
Handles user sign-in via Email/Password.

Firestore Database
Stores users, projects, and role data.

Realtime Database (optional)
Used for streaming updates in future modules.

👥 Default Seed Users
Seeded users (can be created via script or Firebase CLI):

Email	Role	Password
admin1@sustainacore.test	Admin	Password123!
pm1@sustainacore.test	ProjectManager	Password123!
contractor1@sustainacore.test	Contractor	Password123!
client1@sustainacore.test	Client	Password123!

🧩 Authentication Flow
The mobile/web app calls Firebase REST endpoint:
POST https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={ApiKey}

Firebase returns an ID Token (JWT).

That token is sent in the Authorization: Bearer <token> header to the API.

API validates it using Firebase’s public keys (JwtBearer middleware).

Roles are extracted from token claims and used in [Authorize(Policy="AdminOnly")] endpoints.

⚙️ Key Technologies
Layer	Tech Stack
Frontend (Web)	ASP.NET MVC, Razor Pages, Bootstrap
Frontend (Mobile)	.NET MAUI, XAML, SecureStorage
Backend	ASP.NET Core 9, Firebase Admin SDK, Firestore
Security	Firebase Authentication + JWT Bearer
Data	Firestore + optional Realtime Database
CI/CD	GitHub Actions (planned)
Language	C# 12, .NET 9

🧠 Common Troubleshooting
Issue	Cause	Fix
CORS errors	Emulator/Web origins not whitelisted	Check CORS policy in API Program.cs
Android login fails	Using localhost	Use 10.0.2.2 instead
Swagger unreachable	API port changed	Confirm API still runs on 5180
“Dashboard not found”	Legacy route	Redirect from /Dashboard/Admin to /Admin (implemented)

🧩 Extending the System
You can add:

Role management endpoints under /api/users

Project tracking via Firestore collections (Projects, Tasks)

Notifications via Firebase Cloud Messaging (FCM)

Offline sync in MAUI using local SQLite

🧾 License
Copyright © 2025
Cooper & Cooper Group / Sustainacore Project
All rights reserved.

✅ Quick Start (Summary)
powershell
Copy code
# API + Web
pwsh .\scripts\run-all.ps1

# API + Web + Mobile (Windows)
pwsh .\scripts\run-all-with-mobile.ps1

# API only
dotnet watch run --project .\src\Web\Sustainacore.Api\Sustainacore.Api.csproj
Then visit:

Swagger: http://localhost:5180/swagger

Web App: http://localhost:5166

Mobile: runs via MAUI target build