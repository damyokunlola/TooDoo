# Database Migration Guide

## Overview

Entity Framework Core manages the database schema through migrations. Follow these steps to create and update your database.

## Prerequisites

Ensure you have the EF Core CLI tools installed:

```bash
dotnet tool install --global dotnet-ef
```

To verify installation:

```bash
dotnet ef --version
```

## Initial Setup

### 1. Create the First Migration

Run this command to create an initial migration:

```bash
dotnet ef migrations add InitialCreate
```

This creates migration files in the `Migrations/` folder that define the initial schema.

### 2. Update the Database

Apply the migration to create the database:

```bash
dotnet ef database update
```

This will:
- Create the database `TooDooListDb`
- Create the `Users` table
- Create the `Tasks` table
- Set up relationships and constraints

## Database Schema

### Users Table
```sql
CREATE TABLE [dbo].[Users] (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Age INT NOT NULL,
    Profession NVARCHAR(100) NOT NULL,
    EncryptedPassword NVARCHAR(MAX) NOT NULL,
    CreatedDate DATETIME2 NOT NULL
);
```

### Tasks Table
```sql
CREATE TABLE [dbo].[Tasks] (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000),
    Status INT NOT NULL,
    UserId INT NOT NULL,
    CreatedDate DATETIME2 NOT NULL,
    CompletedDate DATETIME2,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
```

## Common Migration Tasks

### Add a New Migration After Model Changes

1. Update your model in `Models/` folder
2. Create a new migration:

```bash
dotnet ef migrations add AddNewColumn
```

3. Update the database:

```bash
dotnet ef database update
```

### Remove Last Migration (Before Database Update)

```bash
dotnet ef migrations remove
```

### View Applied Migrations

```bash
dotnet ef migrations list
```

### Revert to Previous Migration

```bash
dotnet ef database update PreviousMigrationName
```

### Generate SQL Script from Migrations

```bash
dotnet ef migrations script InitialCreate LatestMigration -o migration.sql
```

## Troubleshooting

### "The connection string is not specified"
Solution: Ensure `appsettings.json` contains:
```json
"ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=TooDooListDb;Trusted_Connection=true;TrustServerCertificate=true;"
}
```

### "Cannot connect to database"
Solution:
1. Verify SQL Server is running
2. Check server name: `.` or `.\SQLEXPRESS` or `(localdb)\mssqllocaldb`
3. Test connection in SQL Server Management Studio

### Migration Conflicts
Solution:
```bash
dotnet ef migrations remove
dotnet ef migrations add FreshStart
dotnet ef database update
```

### Database Already Exists with Different Schema
Solution:
```bash
# Drop existing database (WARNING: Data will be lost)
dotnet ef database drop

# Recreate with migrations
dotnet ef database update
```

## Viewing the Database

### Using SQL Server Management Studio (SSMS)
1. Connect to: `.\SQLEXPRESS` or `(localdb)\mssqllocaldb`
2. Navigate to: Databases → TooDooListDb
3. View tables under: Tables

### Using Command Line
```bash
# List all tables
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo'

# Describe Users table
EXEC sp_columns 'Users'

# Count records
SELECT 'Users' AS TableName, COUNT(*) AS RecordCount FROM dbo.Users
UNION ALL
SELECT 'Tasks', COUNT(*) FROM dbo.Tasks
```

## Backup and Restore

### Backup Database
```bash
# Via SQL Server Management Studio
Right-click Database → Tasks → Back Up

# Via Command Line (T-SQL)
BACKUP DATABASE [TooDooListDb] TO DISK = 'C:\Backup\TooDooListDb.bak'
```

### Restore Database
```bash
# Via SQL Server Management Studio
Right-click Databases → Restore Database

# Via Command Line
RESTORE DATABASE [TooDooListDb] FROM DISK = 'C:\Backup\TooDooListDb.bak'
```

## Performance Tips

1. **Add Indexes** for frequently searched columns:
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>()
        .HasIndex(u => u.Email)
        .IsUnique();
}
```

2. **Use Lazy Loading** for related entities:
```csharp
var user = await _context.Users
    .Include(u => u.Tasks)
    .FirstOrDefaultAsync(u => u.Id == userId);
```

3. **Batch Operations** for better performance:
```csharp
_context.Tasks.AddRange(taskList);
await _context.SaveChangesAsync();
```

## Migration File Structure

Each migration file contains:
- `Up()` - Changes to apply
- `Down()` - Changes to revert

Example:
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.CreateTable(
        name: "Users",
        schema: "dbo",
        columns: table => new
        {
            Id = table.Column<int>()
                .Annotation("SqlServer:Identity", "1, 1"),
            Name = table.Column<string>(
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false),
            // ... more columns
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_Users", x => x.Id);
            table.UniqueConstraint("AK_Users_Email", x => x.Email);
        });
}
```

---

For more information, visit: [EF Core Migrations Documentation](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
