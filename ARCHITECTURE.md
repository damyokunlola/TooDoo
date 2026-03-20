# TooDooList - Architecture & Implementation Guide

## Project Architecture

### Layered Architecture

```
┌─────────────────────────────────────┐
│          Presentation Layer          │
│   (Views - Razor Views & Layout)    │
└─────────────────┬───────────────────┘
                  │
┌─────────────────▼───────────────────┐
│        Application Layer             │
│   (Controllers - Request Handlers)   │
└─────────────────┬───────────────────┘
                  │
┌─────────────────▼───────────────────┐
│      Business Logic Layer            │
│   (Services - Core Functionality)    │
└─────────────────┬───────────────────┘
                  │
┌─────────────────▼───────────────────┐
│      Data Access Layer               │
│  (DbContext & Entity Framework)      │
└─────────────────┬───────────────────┘
                  │
┌─────────────────▼───────────────────┐
│        Database Layer                │
│     (SQL Server Database)            │
└──────────────────────────────────────┘
```

## Components Overview

### 1. Models (Domain Entities)

#### User Model
```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }  // Unique
    public int Age { get; set; }
    public string Profession { get; set; }
    public string EncryptedPassword { get; set; }  // AES-256 encrypted
    public DateTime CreatedDate { get; set; }
    public virtual ICollection<TaskItem> Tasks { get; set; }  // Navigation
}
```

#### TaskItem Model
```csharp
public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public TaskStatus Status { get; set; }  // Enum: Ongoing, Completed, Cancelled
    public int UserId { get; set; }  // Foreign Key
    public DateTime CreatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public virtual User User { get; set; }  // Navigation
}
```

### 2. Services Layer

#### EncryptionService
```
Purpose: Handle AES encryption and decryption
Responsibilities:
- Encrypt plain text passwords to AES-256-CBC format
- Decrypt encrypted passwords for verification
- Use IV (Initialization Vector) for security
- Combine IV + Encrypted data for storage

Key Features:
- AES algorithm with 256-bit key
- CBC mode for security
- PKCS7 padding
- Base64 encoding for storage
```

#### AuthenticationService
```
Purpose: Handle user registration and login
Responsibilities:
- Validate new user registration
- Check email uniqueness
- Encrypt and store password
- Authenticate login credentials
- Decrypt and verify passwords

Key Features:
- Email validation and uniqueness check
- Password hashing using EncryptionService
- User session management
- Error handling
```

#### TaskService
```
Purpose: Manage task operations and statistics
Responsibilities:
- Calculate task statistics
- Retrieve user tasks
- Create new tasks
- Update task status
- Count completed/ongoing/cancelled tasks

Key Features:
- Task statistics calculation
- Filtered queries by user
- Status tracking
- Completion date tracking
```

### 3. Controllers

#### AccountController
```
Routes:
- GET /Account/Register          → Show registration form
- POST /Account/Register         → Process registration
- GET /Account/Login             → Show login form
- POST /Account/Login            → Process login
- GET /Account/Logout            → Clear session and logout

Session Management:
- Store UserId, UserName, UserEmail in session
- Session timeout: 30 minutes
- Clear all data on logout
```

#### DashboardController
```
Routes:
- GET /Dashboard                 → Show task statistics

Features:
- Check if user is logged in
- Retrieve task statistics
- Display on dashboard
- Require login (redirect if not authenticated)
```

#### HomeController
```
Routes:
- GET /Home/Index                → Show home page
- GET /                          → Default route to home

Features:
- Display welcome page
- Show login/register links if not authenticated
- Show dashboard link if authenticated
```

### 4. Views

#### Register View (/Account/Register)
```
Form Fields:
- Name (text, required, 2-100 chars)
- Email (email, required, unique)
- Age (number, required, 18-120)
- Profession (text, required)
- Password (password, required, min 6 chars)
- Confirm Password (password, required, must match)

Validation:
- Client-side: jQuery Validation
- Server-side: Data Annotations in ViewModel
- Error messages displayed inline
```

#### Login View (/Account/Login)
```
Form Fields:
- Email (email, required)
- Password (password, required)
- Remember Me (checkbox, optional)

Validation:
- Email format validation
- Required field validation
- Error message on invalid credentials
```

#### Dashboard View (/Dashboard)
```
Display Elements:
- Total Tasks card (Primary color)
- Completed Tasks card (Success color)
- Ongoing Tasks card (Warning color)
- Cancelled Tasks card (Danger color)
- Statistics table with counts and percentages

Data Source:
- TaskStatistics object from service
- Per-user filtered data
```

#### Layout View (_Layout.cshtml)
```
Components:
- Navigation Bar
  - Logo/Brand
  - Dashboard link (if logged in)
  - User greeting (if logged in)
  - Login/Register links (if not logged in)
  - Logout link (if logged in)
- Alert messages (Success/Error)
- Body content area
- Footer
- JavaScript includes

Bootstrap Integration:
- Navbar with responsive hamburger
- Bootstrap 4.5.2 CSS
- Bootstrap JS for dropdown/modal
- jQuery & jQuery Validation
```

## Data Flow

### Registration Flow
```
1. User visits /Account/Register
2. Fills registration form
3. Submits form to POST /Account/Register
4. Controller validates input (ModelState)
5. Calls AuthenticationService.RegisterUserAsync()
6. Service checks email uniqueness
7. Service encrypts password using EncryptionService
8. Service creates User entity
9. Service saves to database via DbContext
10. Controller redirects to login with success message
```

### Login Flow
```
1. User visits /Account/Login
2. Enters email and password
3. Submits form to POST /Account/Login
4. Controller validates input (ModelState)
5. Calls AuthenticationService.LoginUserAsync()
6. Service finds user by email
7. Service decrypts stored password using EncryptionService
8. Service compares passwords
9. If match:
   - Store user info in session
   - Redirect to Dashboard
10. If no match:
    - Show error message
    - Reload login form
```

### Dashboard Flow
```
1. User clicks Dashboard link (requires login)
2. Request to GET /Dashboard
3. Controller checks if UserId in session
4. If not logged in: Redirect to login
5. If logged in:
   - Call TaskService.GetTaskStatisticsAsync(userId)
   - Service queries database for user's tasks
   - Service counts by status
   - Return TaskStatistics object
6. Pass statistics to view
7. View displays cards and table
```

## Database Relationships

### User-Task Relationship
```
One User → Many Tasks (One-to-Many)

- User.Tasks (collection of TaskItem)
- TaskItem.User (reference to User)
- Foreign Key: TaskItem.UserId → User.Id
- DeleteBehavior: Cascade (delete tasks when user deleted)
```

## Encryption Implementation

### AES Encryption Details
```
Algorithm: AES (Advanced Encryption Standard)
Mode: CBC (Cipher Block Chaining)
Key Size: 256 bits (32 bytes)
Padding: PKCS7
IV: Generated randomly for each encryption

Process:
1. Generate random IV (16 bytes)
2. Create AES cipher with key and IV
3. Encrypt plaintext
4. Combine IV + Encrypted Data
5. Base64 encode for storage

Decryption:
1. Base64 decode from storage
2. Extract IV (first 16 bytes)
3. Extract encrypted data
4. Create AES cipher with key and extracted IV
5. Decrypt to plaintext
```

## Session Management

### Session Configuration
```csharp
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;  // Secure cookie
    options.Cookie.IsEssential = true;  // Required by GDPR
});
```

### Session Data Stored
```
- UserId (int): User's unique ID
- UserName (string): User's full name
- UserEmail (string): User's email address
```

## Dependency Injection

### Service Registration in Program.cs
```csharp
// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
);

// Services
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ITaskService, TaskService>();

// Session
builder.Services.AddSession(options => /* ... */);
```

## Security Features

### 1. Password Encryption
- AES-256 encryption
- Random IV for each password
- Base64 encoding
- Stored in database as encrypted string

### 2. HTTPS Enforcement
- Enforced in middleware
- `app.UseHttpsRedirection()` in Program.cs

### 3. CSRF Protection
- Razor anti-forgery tokens
- ASP.NET Core automatic protection
- Hidden token in forms

### 4. Session Security
- HttpOnly cookie flag
- 30-minute timeout
- Destroys on logout

### 5. Input Validation
- Client-side: jQuery Validation
- Server-side: Data Annotations
- ModelState checking in controllers

## Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=TooDooListDb;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "EncryptionSettings": {
    "Key": "MySecretKey12345MySecretKey12345"
  }
}
```

### Key Configuration Points
1. **Database Connection**: Server and database name
2. **Encryption Key**: Must be exactly 32 characters
3. **Logging**: LogLevel for different categories
4. **Authentication**: Can be extended with OAuth, etc.

## Error Handling

### Global Error Handling
- Try-catch in services
- ModelState validation in controllers
- Custom error messages
- Logging (can be enhanced)

### User-Facing Errors
- Registration: Email already exists
- Login: Invalid credentials
- Database: Connection errors (logged, generic message shown)

## Testing Scenarios

### Registration Test
```
1. Navigate to /Account/Register
2. Enter: Name="John", Email="john@test.com", Age=25, Profession="Developer", Password="Pass123", ConfirmPassword="Pass123"
3. Click Register
4. Assert: Redirected to login with success message
5. Assert: User created in database
6. Assert: Password encrypted
```

### Login Test
```
1. Navigate to /Account/Login
2. Enter: Email="john@test.com", Password="Pass123"
3. Click Login
4. Assert: Redirected to Dashboard
5. Assert: Session contains UserId, UserName, UserEmail
6. Assert: Navigation shows logout link
```

### Dashboard Test
```
1. Login successfully
2. Navigate to /Dashboard
3. Assert: See task statistics cards
4. Assert: Numbers match database count
5. Logout
6. Try accessing /Dashboard directly
7. Assert: Redirected to login
```

## Performance Considerations

- **Queries**: Use Include() for related data
- **Session**: 30-minute timeout balances security and usability
- **Indexes**: Email is indexed (unique constraint)
- **Database**: Connection pooling in SQL Server
- **Caching**: Can be added for statistics

## Future Enhancements

1. **Two-Factor Authentication**: SMS or email OTP
2. **Social Login**: OAuth with Google, Facebook
3. **Task API**: RESTful endpoints for mobile apps
4. **Real-time Updates**: SignalR for live statistics
5. **Email Notifications**: Send task reminders
6. **Task Categories**: Organize tasks
7. **Export Reports**: PDF/Excel task reports
8. **Admin Dashboard**: User management
9. **Rate Limiting**: Prevent brute force attacks
10. **Audit Logging**: Track all user actions

## Code Quality Standards

- Clean code principles
- Single Responsibility Principle
- Dependency Injection
- Repository Pattern (can be added)
- Unit tests (can be added)
- XML documentation (can be added)

---

This architecture ensures scalability, maintainability, and security for the TooDooList application.
