# Indepth Clean - Quick Start Guide

## Getting Started

### Prerequisites
- .NET 8.0 or higher
- PostgreSQL database running
- Visual Studio Code or Visual Studio

### Database Connection
The application uses PostgreSQL. Ensure your connection string in `appsettings.json` is correct:

```json
"ConnectionStrings": {
    "DefaultConnection": "Host=127.0.0.1;Port=5432;Database=MyAppDb;Username=postgres;Password=o2020;"
}
```

### Running the Application

1. **Navigate to project directory:**
   ```bash
   cd "c:\Users\Okunlolaad\Desktop\TooDooList"
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Apply database migrations:**
   ```bash
   dotnet ef database update
   ```

4. **Run the application:**
   ```bash
   dotnet run
   ```

5. **Access the application:**
   - Open your browser and navigate to `https://localhost:5001`
   - Or `http://localhost:5000` for HTTP

## Using the Application

### Main Features

#### 1. Home Page
- Welcome screen with service information
- Shows all 4 service types: Laundry, Dry Cleaning, Deep Cleaning, Sofa Cleaning
- Quick links to register or login

#### 2. Registration & Login
- Same as before (unchanged)
- Create new account or login with existing credentials

#### 3. Dashboard
- View booking statistics (Total, Completed, In Progress, Pending, Cancelled)
- Visual progress bars for each status
- Quick links to create new booking or view all bookings

#### 4. Create Booking
- Fill in service details
- Select service type from dropdown
- Enter scheduled date and time
- Provide service address
- Set pricing
- Add service description

#### 5. My Bookings
- View all your bookings in card format
- See booking status at a glance
- Quick view details or edit options
- Filter by status

#### 6. Booking Details
- Complete booking information
- Current status with visual badge
- Edit or cancel options (if not completed/cancelled)
- Scheduled service date and time
- Service address and price

## Navigation Menu

When logged in, you'll see:
- **Dashboard** - Overview of all bookings
- **Bookings** - Dropdown with:
  - New Booking - Create new service booking
  - My Bookings - View all your bookings
- **Welcome message** with your name
- **Logout** - Sign out of your account

## Booking Status Lifecycle

1. **Start (Pending)** - Booking just created
2. **In Progress** - Service provider is working on it
3. **Completed** - Service finished successfully
4. **Cancelled** - Booking was cancelled

Progress shown with color-coded badges:
- Pending: Red (#e74c3c)
- In Progress: Orange (#f39c12)
- Completed: Blue (#3498db)
- Cancelled: Gray (#95a5a6)

## Email Configuration (Optional)

To enable email notifications:

1. Update `appsettings.json` SMTP settings:
   ```json
   "SmtpSettings": {
       "Server": "smtp.gmail.com",
       "Port": "587",
       "Username": "your-email@gmail.com",
       "Password": "your-app-password",
       "FromEmail": "your-email@gmail.com",
       "EnableSSL": "true"
   }
   ```

2. Emails are automatically sent when:
   - User registers (Welcome email)
   - New booking is created (Booking confirmation)

## Customization

### Change Color Theme
Edit `wwwroot/css/site.css` - Look for:
```css
:root {
    --primary-green: #2ecc71;
    --dark-green: #27ae60;
    /* ... other colors ... */
}
```

### Change Application Title
Update in `_Layout.cshtml`:
```csharp
<a class="navbar-brand" href="/">
    <i class="fas fa-spray-can"></i> Indepth Clean
</a>
```

### Add Service Types
Edit `Models/ServiceBooking.cs` - Update the `ServiceType` enum:
```csharp
public enum ServiceType
{
    Laundry,
    DryCleaning,
    DeepCleaning,
    SofaCleaning,
    // Add new types here
}
```

## Troubleshooting

### Issue: Database connection failed
- Verify PostgreSQL is running
- Check connection string in `appsettings.json`
- Ensure database exists

### Issue: Migrations failed
- Run `dotnet ef database update` again
- Check database connection
- Verify no other instances are running

### Issue: Pages not loading
- Clear browser cache
- Check browser console for errors
- Verify all static files are in `wwwroot` folder

### Issue: Services not working
- Check `Program.cs` for dependency injection
- Verify service interfaces are registered
- Check controller constructors

## Performance Tips

1. **Database Indexing** - ServiceBookings table is indexed on UserId
2. **Async/Await** - All database calls use async methods
3. **Email Sending** - Uses fire-and-forget pattern
4. **Caching** - Session cookies cache user ID

## Security Features

- **Password Encryption** - Passwords stored encrypted with AES
- **Session Management** - 30-minute session timeout
- **SQL Injection Protection** - Entity Framework prevents SQL injection
- **HTTPS** - Application uses HTTPS in production

## Deployment

### Deploy to Production

1. **Build Release:**
   ```bash
   dotnet build -c Release
   ```

2. **Publish:**
   ```bash
   dotnet publish -c Release -o ./publish
   ```

3. **Configure Production Settings:**
   - Update `appsettings.Production.json`
   - Set proper database connection string
   - Configure SMTP for emails
   - Set secure session cookies

4. **Run in Production:**
   ```bash
   dotnet TooDooList.dll --environment Production
   ```

## Support & Documentation

- API endpoints follow RESTful conventions
- Views use Razor syntax with Bootstrap 4
- Database uses PostgreSQL with Entity Framework Core
- Emails use HTML templates

For detailed information, refer to:
- `CLEANING_SERVICES_TRANSFORMATION.md` - Complete transformation summary
- `README.md` - Original project documentation
- Code comments in Controllers, Services, and Views

## What's Next?

Consider adding:
- 📱 Mobile app
- 💳 Online payment integration
- ⭐ User ratings and reviews
- 🔔 Push notifications
- 📊 Advanced analytics
- 🔐 Two-factor authentication
- 🌍 Multi-language support
