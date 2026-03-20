# TooDooList - ASP.NET Core MVC Application

A complete ASP.NET Core MVC application for task management with user authentication, AES encryption, and task statistics dashboard.

## Features

- **User Registration**: Create new users with Name, Email, Age, Profession, and encrypted password
- **Secure Login**: AES-encrypted password authentication
- **Dashboard**: View task statistics (Total, Completed, Ongoing, Cancelled tasks)
- **Task Management**: Create and manage tasks with different statuses
- **Responsive UI**: Bootstrap-based responsive design
- **Form Validation**: jQuery-based client-side validation
- **Entity Framework Core**: Database ORM with SQL Server

## Project Structure

```
TooDooList/
├── Models/                 # Data models (User, TaskItem)
├── Controllers/            # MVC Controllers (Account, Dashboard, Home)
├── Services/              # Business logic layer
│   ├── EncryptionService.cs
│   ├── AuthenticationService.cs
│   └── TaskService.cs
├── Data/                  # DbContext and data access
├── Views/                 # Razor views
│   ├── Account/          # Register and Login views
│   ├── Dashboard/        # Dashboard view
│   ├── Home/            # Home page
│   └── Shared/          # Layout and shared views
├── wwwroot/             # Static files (CSS, JavaScript)
├── Program.cs           # Application entry point
└── appsettings.json     # Configuration
```

## Prerequisites

- .NET 8.0 SDK or later
- SQL Server 2019 or later (LocalDB or Express)
- Visual Studio Code or Visual Studio 2022

## Installation & Setup

### 1. Clone or Extract Project

```bash
cd TooDooList
```

### 2. Restore NuGet Packages

```bash
dotnet restore
```

### 3. Update Database Connection

Edit `appsettings.json` and update the connection string if needed:

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=TooDooListDb;Trusted_Connection=true;TrustServerCertificate=true;"
}
```

### 4. Create Database & Run Migrations

First, install Entity Framework Core tools globally:

```bash
dotnet tool install --global dotnet-ef
```

Create initial migrations:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Update Encryption Key (Optional)

Edit `appsettings.json` to change the encryption key:

```json
"EncryptionSettings": {
    "Key": "YourCustomKeyHere123YourCustomKey"
}
```

**Note**: The key must be exactly 32 characters for AES-256 encryption.

### 6. Run the Application

```bash
dotnet run
```

The application will be available at: `https://localhost:5001`

## Usage

### User Registration

1. Navigate to `/Account/Register`
2. Fill in all required fields
3. Password must be at least 6 characters
4. Passwords must match
5. Click "Register"

### User Login

1. Navigate to `/Account/Login`
2. Enter registered email and password
3. Click "Login"
4. You'll be redirected to the Dashboard

### Dashboard

After login, view your task statistics:
- **Total Tasks**: Sum of all your tasks
- **Completed Tasks**: Tasks marked as completed
- **Ongoing Tasks**: Tasks currently in progress
- **Cancelled Tasks**: Tasks that were cancelled

## Key Components

### Encryption Service

Uses AES encryption with CBC mode and PKCS7 padding:

```csharp
IEncryptionService encryptionService;
string encrypted = encryptionService.Encrypt("password");
string decrypted = encryptionService.Decrypt(encrypted);
```

### Authentication Service

Handles user registration and login with encrypted passwords:

```csharp
IAuthenticationService authService;
await authService.RegisterUserAsync(name, email, age, profession, password);
var user = await authService.LoginUserAsync(email, password);
```

### Task Service

Manages task statistics and operations:

```csharp
ITaskService taskService;
TaskStatistics stats = await taskService.GetTaskStatisticsAsync(userId);
```

## Database Schema

### Users Table

| Column | Type | Notes |
|--------|------|-------|
| Id | int | Primary Key |
| Name | nvarchar(100) | User's full name |
| Email | nvarchar(100) | Unique, required |
| Age | int | User's age |
| Profession | nvarchar(100) | User's profession |
| EncryptedPassword | nvarchar(max) | AES encrypted |
| CreatedDate | datetime | Registration date |

### Tasks Table

| Column | Type | Notes |
|--------|------|-------|
| Id | int | Primary Key |
| Title | nvarchar(200) | Task title |
| Description | nvarchar(1000) | Task description |
| Status | int | 0=Ongoing, 1=Completed, 2=Cancelled |
| UserId | int | Foreign Key to Users |
| CreatedDate | datetime | Task creation date |
| CompletedDate | datetime | When task was completed |

## Security Features

- **AES Encryption**: Passwords encrypted with AES-256-CBC
- **Unique Email**: Email constraint prevents duplicate registrations
- **Session Management**: User sessions expire after 30 minutes
- **HTTPS**: Enforced in production
- **CSRF Protection**: Tag helpers provide CSRF tokens

## Dependencies

- `Microsoft.EntityFrameworkCore` - ORM
- `Microsoft.EntityFrameworkCore.SqlServer` - SQL Server provider
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` - Identity support
- Bootstrap 4.5.2 - UI Framework
- jQuery 3.5.1 - JavaScript utilities
- jQuery Validation - Form validation

## Troubleshooting

### Connection String Issues

If you get a connection error:

1. Ensure SQL Server is running
2. Update the server name in `appsettings.json`
3. For SQL Server Express: Use `.\SQLEXPRESS` or `(localdb)\mssqllocaldb`

### Migration Issues

If migrations fail:

```bash
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Encryption Key Length

The encryption key must be exactly 32 characters (256 bits) for AES-256. Update it in `appsettings.json`.

## Development Tips

- Use `dotnet watch run` for hot reload during development
- Enable SQL Server Profiler to debug queries
- Check browser console for form validation errors
- Use Entity Framework Core logging to troubleshoot database issues

## Future Enhancements

- Add task deletion feature
- Implement password recovery via email
- Add activity logging
- Create API endpoints for mobile apps
- Implement two-factor authentication
- Add task categories and filtering
- Create task notification system

## License

MIT License - Free for personal and commercial use.

## Support

For issues or questions, please refer to the code comments or contact the development team.

---

**Version**: 1.0.0  
**Last Updated**: 2024
