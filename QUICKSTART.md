# TooDooList - Quick Start Guide

## Project Overview

This is a complete ASP.NET Core MVC application with:
- User authentication with AES encryption
- Task management system
- Dashboard with statistics
- Responsive Bootstrap UI
- Entity Framework Core integration

## Quick Setup (5 minutes)

### Step 1: Install .NET EF Tools
```bash
dotnet tool install --global dotnet-ef
```

### Step 2: Restore Packages
```bash
dotnet restore
```

### Step 3: Create Initial Migration
```bash
dotnet ef migrations add InitialCreate --project . --startup-project .
```

### Step 4: Update Database
```bash
dotnet ef database update --project . --startup-project .
```

### Step 5: Run the Application
```bash
dotnet run
```

The application will start at: `https://localhost:5001`

## Default Configuration

- **Database**: SQL Server LocalDB on `.\SQLEXPRESS`
- **Database Name**: `TooDooListDb`
- **Encryption Key**: `MySecretKey12345MySecretKey12345` (change in production)
- **Session Timeout**: 30 minutes

## Application Flow

1. **Home Page** (`/`) - Landing page with login/register links
2. **Register** (`/Account/Register`) - Create new user account
3. **Login** (`/Account/Login`) - Authenticate user
4. **Dashboard** (`/Dashboard`) - View task statistics

## File Structure

```
Controllers/
├── AccountController.cs     - Registration and Login
├── DashboardController.cs   - Dashboard and stats
└── HomeController.cs        - Home page

Models/
├── User.cs                  - User entity
└── TaskItem.cs              - Task entity

Services/
├── EncryptionService.cs     - AES encryption/decryption
├── AuthenticationService.cs - User registration and login
└── TaskService.cs           - Task operations

Data/
└── ApplicationDbContext.cs  - EF Core DbContext

Views/
├── Account/
│   ├── Register.cshtml
│   └── Login.cshtml
├── Dashboard/
│   └── Index.cshtml
├── Home/
│   └── Index.cshtml
└── Shared/
    └── _Layout.cshtml
```

## Key Features Explained

### 1. AES Encryption
All passwords are encrypted using AES-256-CBC before storage:
- **Located in**: `Services/EncryptionService.cs`
- **Key Configuration**: `appsettings.json` → `EncryptionSettings.Key`

### 2. User Registration
- Validates email uniqueness
- Requires 6+ character password
- Checks password confirmation
- Encrypts password before storing

### 3. User Login
- Decrypts stored password
- Compares with login attempt
- Creates session on success
- Redirects to dashboard

### 4. Dashboard Statistics
- **Total Tasks**: Count of all tasks
- **Completed**: Tasks with Completed status
- **Ongoing**: Tasks with Ongoing status
- **Cancelled**: Tasks with Cancelled status

## Customization

### Change Encryption Key
**File**: `appsettings.json`
```json
"EncryptionSettings": {
    "Key": "YourNewKeyMustBe32CharactersLng"
}
```

### Change Database Connection
**File**: `appsettings.json`
```json
"ConnectionStrings": {
    "DefaultConnection": "Your connection string here"
}
```

### Modify Session Timeout
**File**: `Program.cs`
```csharp
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Change 30 to 60
});
```

## Troubleshooting

### Database Connection Failed
```
Error: Cannot connect to SQL Server
Solution: 
1. Ensure SQL Server is running
2. Check connection string in appsettings.json
3. For LocalDB: Use (localdb)\mssqllocaldb
```

### Migration Error
```
Solution: Run the following commands in order:
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Port Already in Use
```
Solution: Change port in launchSettings.json or run on different port:
dotnet run --urls "https://localhost:5002"
```

## Testing the Application

### Step 1: Register a User
1. Go to `https://localhost:5001/Account/Register`
2. Fill in all fields
3. Click Register
4. You'll see success message

### Step 2: Login
1. Go to `https://localhost:5001/Account/Login`
2. Use registered email and password
3. You'll be redirected to dashboard

### Step 3: View Dashboard
1. See task statistics
2. Check navigation bar shows "Welcome, [YourName]"
3. Click Logout to test logout functionality

## Development Tips

- Use `dotnet watch run` for auto-reload
- Check SQL Server via SQL Server Management Studio
- Use browser DevTools to debug JavaScript
- Check Console for ASP.NET Core logs

## Security Considerations

1. **Change Encryption Key** in production
2. **Use HTTPS** (already enforced)
3. **Update Session Timeout** based on security needs
4. **Enable CORS** only for trusted origins
5. **Implement Rate Limiting** for login attempts
6. **Use Strong Database Passwords**

## Next Steps

1. Add password recovery email feature
2. Implement task CRUD operations
3. Add user profile management
4. Create API endpoints
5. Implement two-factor authentication
6. Add admin panel
7. Create task notifications

## Support Files

- **README.md** - Full documentation
- **Program.cs** - Application configuration
- **appsettings.json** - Settings and connection string
- **Models** - Data entities
- **Services** - Business logic
- **Controllers** - Request handlers
- **Views** - UI pages

---

**Ready to run!** Execute `dotnet run` and visit `https://localhost:5001`
