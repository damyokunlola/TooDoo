# Email Service Implementation Guide

## Overview

The TooDooList application now includes a complete SMTP email service that automatically sends emails to users:

1. **Welcome Email** - Sent when a user registers
2. **Task Created Email** - Sent when a user creates a new task

## Email Service Architecture

### Components

1. **IEmailService Interface**
   - `SendWelcomeEmailAsync(email, name)` - Send registration welcome email
   - `SendTaskCreatedEmailAsync(email, userName, taskTitle)` - Send task creation notification
   - `SendEmailAsync(to, subject, body, isHtml)` - Generic email sending method

2. **EmailService Implementation**
   - Uses SMTP for email delivery
   - Supports HTML emails with professional templates
   - Graceful error handling with logging
   - Configuration-based SMTP settings

## Configuration

### SMTP Settings in appsettings.json

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

### Supported Email Providers

#### Gmail (Recommended for Development)

1. **Create a Gmail App Password:**
   - Go to: https://myaccount.google.com/apppasswords
   - Select "Mail" and "Windows Computer"
   - Copy the generated 16-character password

2. **Update appsettings.json:**
   ```json
   "SmtpSettings": {
       "Server": "smtp.gmail.com",
       "Port": "587",
       "Username": "your-email@gmail.com",
       "Password": "your-16-character-app-password",
       "FromEmail": "your-email@gmail.com",
       "EnableSSL": "true"
   }
   ```

#### Outlook.com

```json
"SmtpSettings": {
    "Server": "smtp-mail.outlook.com",
    "Port": "587",
    "Username": "your-email@outlook.com",
    "Password": "your-password",
    "FromEmail": "your-email@outlook.com",
    "EnableSSL": "true"
}
```

#### SendGrid

```json
"SmtpSettings": {
    "Server": "smtp.sendgrid.net",
    "Port": "587",
    "Username": "apikey",
    "Password": "SG.your-sendgrid-api-key",
    "FromEmail": "noreply@yourdomain.com",
    "EnableSSL": "true"
}
```

#### Mailgun

```json
"SmtpSettings": {
    "Server": "smtp.mailgun.org",
    "Port": "587",
    "Username": "postmaster@your-domain.mailgun.org",
    "Password": "your-mailgun-password",
    "FromEmail": "noreply@your-domain.mailgun.org",
    "EnableSSL": "true"
}
```

#### Local SMTP Server (Development)

```json
"SmtpSettings": {
    "Server": "localhost",
    "Port": "25",
    "Username": "",
    "Password": "",
    "FromEmail": "noreply@toodolist.local",
    "EnableSSL": "false"
}
```

## Features

### Welcome Email (On Registration)

**Triggered:** When user completes registration
**Recipient:** New user's email address
**Content:** 
- Welcome greeting with user's name
- Introduction to TooDooList features
- Direct login link
- Professional HTML template with branding

**Example:**
```
To: user@example.com
Subject: Welcome to TooDooList!

Dear John,

Thank you for registering with us. We're excited to have you on board!

With TooDooList, you can:
- Create and manage your tasks efficiently
- Track your progress with detailed statistics
- Stay organized and productive

[Log In Now Button]

Best regards,
The TooDooList Team
```

### Task Created Email

**Triggered:** When user creates a new task
**Recipient:** User's registered email
**Content:**
- Task creation confirmation
- Task title and details
- Creation timestamp
- Direct link to dashboard
- Professional HTML template

**Example:**
```
To: user@example.com
Subject: New Task Created - TooDooList

Hi John,

A new task has been created in your TooDooList:

Task Title: Complete Project Report
Created Date: March 13, 2024 10:30 AM
Status: Ongoing

[View Your Tasks Button]

Keep up the good work!

Best regards,
The TooDooList Team
```

## Integration Points

### 1. User Registration Flow

**File:** `Controllers/AccountController.cs`

```csharp
[HttpPost]
public async Task<IActionResult> Register(RegisterViewModel model)
{
    // ... validation ...
    bool isRegistered = await _authService.RegisterUserAsync(...);
    // Email sent automatically in AuthenticationService
}
```

**File:** `Services/AuthenticationService.cs`

```csharp
public async Task<bool> RegisterUserAsync(...)
{
    // ... create user ...
    _ = _emailService.SendWelcomeEmailAsync(email, name);
    return true;
}
```

### 2. Task Creation Flow

**File:** `Controllers/TaskController.cs`

```csharp
[HttpPost]
public async Task<IActionResult> Create(CreateTaskViewModel model)
{
    // Get user ID from session
    var userId = HttpContext.Session.GetInt32("UserId");
    
    // Create task (includes email sending in service)
    bool isCreated = await _taskService.CreateTaskAsync(userId.Value, ...);
}
```

**File:** `Services/TaskService.cs`

```csharp
public async Task<bool> CreateTaskAsync(int userId, string title, ...)
{
    // ... create task ...
    _ = _emailService.SendTaskCreatedEmailAsync(user.Email, user.Name, title);
    return true;
}
```

## Error Handling

The email service includes graceful error handling:

```csharp
try
{
    // Send email
    await client.SendMailAsync(message);
    _logger.LogInformation("Email sent successfully to {Email}", to);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error sending email to {Email}", to);
    // Don't throw - email is non-critical
}
```

**Features:**
- Errors are logged but don't crash the application
- Non-blocking (fire-and-forget pattern)
- Validates SMTP configuration before sending
- Handles network failures gracefully

## Logging

Email operations are logged to the application logs:

```
Information: Email sent successfully to user@example.com
Warning: SMTP settings are not configured. Email not sent to user@example.com
Error: Error sending email to user@example.com - Exception details
```

## Testing

### Test Registration Email

1. Navigate to: `https://localhost:5001/Account/Register`
2. Fill in form with test data
3. Click Register
4. Check your email inbox for welcome email
5. Verify email contains:
   - Your name
   - Welcome message
   - Login link

### Test Task Created Email

1. Login to application
2. Navigate to: `https://localhost:5001/Task/Create`
3. Fill in task form
4. Click "Create Task"
5. Check email inbox for task notification
6. Verify email contains:
   - Your name
   - Task title
   - Creation timestamp
   - Dashboard link

## Production Deployment

### Best Practices

1. **Use Environment Variables:**
   ```csharp
   "SmtpSettings": {
       "Server": "@(Environment.GetEnvironmentVariable("SMTP_SERVER"))",
       "Port": "@(Environment.GetEnvironmentVariable("SMTP_PORT"))",
       "Username": "@(Environment.GetEnvironmentVariable("SMTP_USERNAME"))",
       "Password": "@(Environment.GetEnvironmentVariable("SMTP_PASSWORD"))",
       "FromEmail": "@(Environment.GetEnvironmentVariable("SMTP_FROM_EMAIL"))",
       "EnableSSL": "@(Environment.GetEnvironmentVariable("SMTP_ENABLE_SSL"))"
   }
   ```

2. **Use Dedicated Email Service:**
   - SendGrid
   - Mailgun
   - Amazon SES
   - Azure Communication Services

3. **Security:**
   - Never commit credentials to version control
   - Use Azure Key Vault or similar
   - Rotate app passwords regularly
   - Use strong, unique emails for sending

4. **Performance:**
   - Consider async queue for high volume
   - Implement retry logic for failed emails
   - Monitor email delivery rates
   - Track bounce and complaint rates

5. **Compliance:**
   - Include unsubscribe links (if building mailing list)
   - Add terms and privacy links in emails
   - Comply with CAN-SPAM, GDPR, etc.

## Troubleshooting

### Email Not Sending

**Problem:** Emails not being sent
**Solutions:**
1. Check SMTP settings in appsettings.json
2. Verify email provider credentials
3. Check application logs for errors
4. Verify firewall allows SMTP port (usually 587)
5. Test SMTP connection manually

### Authentication Failed

**Problem:** SMTP authentication error
**Solutions:**
1. Verify username and password are correct
2. For Gmail, use app password (not regular password)
3. Enable "Less secure app access" if needed (not recommended)
4. Check if account is locked

### Port Issues

**Problem:** Connection timeout or refused
**Solutions:**
1. Verify correct SMTP port (typically 587 for TLS, 465 for SSL)
2. Check firewall/network ACLs
3. Test connectivity: `telnet smtp.gmail.com 587`

### SSL/TLS Issues

**Problem:** SSL certificate errors
**Solutions:**
1. Ensure EnableSSL matches port (587 needs TLS, 465 needs SSL)
2. Update .NET to latest version
3. Consider disabling certificate validation for local testing only

## Email Templates

### Customizing Welcome Email

**File:** `Services/EmailService.cs` - `SendWelcomeEmailAsync` method

Modify the HTML template:
```csharp
string body = $@"
<html>
    <body>
        <!-- Your custom HTML here -->
    </body>
</html>";
```

### Customizing Task Created Email

**File:** `Services/EmailService.cs` - `SendTaskCreatedEmailAsync` method

Modify the HTML template:
```csharp
string body = $@"
<html>
    <body>
        <!-- Your custom HTML here -->
    </body>
</html>";
```

## Future Enhancements

1. **Email Verification:**
   - Send verification link on registration
   - Confirm email before account activation

2. **Email Preferences:**
   - Allow users to opt-out of emails
   - Set notification preferences

3. **Email History:**
   - Store sent emails in database
   - Resend feature

4. **Advanced Notifications:**
   - Task reminders
   - Daily digest emails
   - Weekly reports

5. **Email Analytics:**
   - Track open rates
   - Track click-through rates
   - Delivery reports

## API Reference

### SendWelcomeEmailAsync

```csharp
public async Task SendWelcomeEmailAsync(string email, string name)
```

**Parameters:**
- `email` (string): Recipient email address
- `name` (string): User's full name

**Returns:** Task (void)

**Example:**
```csharp
await _emailService.SendWelcomeEmailAsync("user@example.com", "John Doe");
```

### SendTaskCreatedEmailAsync

```csharp
public async Task SendTaskCreatedEmailAsync(string email, string userName, string taskTitle)
```

**Parameters:**
- `email` (string): Recipient email address
- `userName` (string): User's full name
- `taskTitle` (string): Created task title

**Returns:** Task (void)

**Example:**
```csharp
await _emailService.SendTaskCreatedEmailAsync(
    "user@example.com", 
    "John Doe", 
    "Complete Project Report"
);
```

### SendEmailAsync

```csharp
public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
```

**Parameters:**
- `to` (string): Recipient email address
- `subject` (string): Email subject
- `body` (string): Email body content
- `isHtml` (bool): Whether body contains HTML

**Returns:** Task (void)

**Example:**
```csharp
await _emailService.SendEmailAsync(
    "user@example.com",
    "Custom Subject",
    "<h1>Custom HTML Email</h1>",
    isHtml: true
);
```

## Dependency Injection

The email service is registered in `Program.cs`:

```csharp
builder.Services.AddScoped<IEmailService, EmailService>();
```

**Usage in Controllers/Services:**

```csharp
public class MyController : Controller
{
    private readonly IEmailService _emailService;

    public MyController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task MyMethod()
    {
        await _emailService.SendEmailAsync(...);
    }
}
```

---

**Status:** ✅ Implementation Complete

The email service is fully integrated and ready for use. Configure your SMTP settings in appsettings.json to enable email notifications.
