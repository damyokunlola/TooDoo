# TooDooList - Testing & Deployment Guide

## Testing the Application

### Pre-Testing Checklist
- ✓ Database created and migrations applied
- ✓ appsettings.json configured correctly
- ✓ SQL Server running
- ✓ NuGet packages restored
- ✓ Project builds without errors

### Step-by-Step Testing Guide

#### Test 1: Registration Flow

**Objective**: Verify user registration works correctly

**Steps**:
1. Start application: `dotnet run`
2. Navigate to: `https://localhost:5001/Account/Register`
3. Fill in form:
   - Name: `John Doe`
   - Email: `john@example.com`
   - Age: `30`
   - Profession: `Software Engineer`
   - Password: `SecurePass123`
   - Confirm Password: `SecurePass123`
4. Click "Register"

**Expected Results**:
- ✓ Success message: "Registration successful! Please login."
- ✓ Redirected to login page
- ✓ User created in database

**Verify in Database**:
```sql
SELECT * FROM dbo.Users WHERE Email = 'john@example.com'
-- Assert: EncryptedPassword is not plaintext
-- Assert: Password length ~100+ characters (encrypted + Base64)
```

#### Test 2: Duplicate Email Registration

**Objective**: Verify unique email constraint

**Steps**:
1. Try to register with same email: `john@example.com`
2. Fill all fields correctly
3. Click "Register"

**Expected Results**:
- ✓ Error message: "Email already exists."
- ✓ Remain on registration page
- ✓ Form data preserved (except password)

#### Test 3: Form Validation

**Objective**: Verify client and server validation

**Test Cases**:
1. **Missing Name**: Leave empty → Error: "Please enter a valid name"
2. **Invalid Email**: Enter "notanemail" → Error: "Please enter a valid email"
3. **Age < 18**: Enter "15" → Error: "Age must be between 18 and 120"
4. **Short Password**: Enter "Pass" → Error: "Password must be at least 6 characters"
5. **Mismatched Passwords**: Confirm different → Error: "Passwords do not match"

**Expected Results**:
- ✓ Client validation prevents submission with red error messages
- ✓ Form not submitted to server

#### Test 4: Login Flow

**Objective**: Verify user authentication

**Steps**:
1. Navigate to: `https://localhost:5001/Account/Login`
2. Enter:
   - Email: `john@example.com`
   - Password: `SecurePass123`
3. Click "Login"

**Expected Results**:
- ✓ Redirected to Dashboard
- ✓ Navigation shows: "Welcome, John Doe"
- ✓ Session created with UserId, UserName, UserEmail

**Verify in Session**:
- Check browser DevTools → Application → Cookies → `.AspNetCore.Session`

#### Test 5: Invalid Login Credentials

**Objective**: Verify login error handling

**Test Cases**:
1. **Wrong Email**:
   - Email: `nouser@example.com`
   - Password: `SecurePass123`
   - Expected: "Invalid email or password."

2. **Wrong Password**:
   - Email: `john@example.com`
   - Password: `WrongPass`
   - Expected: "Invalid email or password."

3. **Empty Fields**:
   - Leave fields empty
   - Expected: Validation errors

#### Test 6: Dashboard Display

**Objective**: Verify statistics display

**Steps**:
1. Login successfully
2. Navigate to: `https://localhost:5001/Dashboard`

**Expected Results**:
- ✓ Four statistics cards display
- ✓ Numbers are correct (initially all 0 if no tasks)
- ✓ Statistics table shows breakdown
- ✓ Percentages calculate correctly

#### Test 7: Session Protection

**Objective**: Verify protected pages

**Steps**:
1. Logout (or open new incognito window)
2. Try to access: `https://localhost:5001/Dashboard`

**Expected Results**:
- ✓ Redirected to login page
- ✓ Cannot access dashboard without login

#### Test 8: Logout

**Objective**: Verify logout functionality

**Steps**:
1. Login successfully
2. Click "Logout" in navigation
3. Click "Dashboard" link

**Expected Results**:
- ✓ Navigation returns to login/register links
- ✓ Session cleared
- ✓ Cannot access dashboard (redirected to login)

#### Test 9: Password Encryption

**Objective**: Verify passwords are encrypted

**Steps**:
1. Register user with password: `MyTestPassword123`
2. Query database:
```sql
SELECT Email, EncryptedPassword FROM dbo.Users WHERE Email = 'test@example.com'
```

**Expected Results**:
- ✓ EncryptedPassword is NOT plain text
- ✓ EncryptedPassword is Base64 encoded (~100+ chars)
- ✓ Each registration produces different encrypted string (due to random IV)

#### Test 10: UI Responsiveness

**Objective**: Verify mobile responsiveness

**Steps**:
1. Open browser DevTools (F12)
2. Toggle device toolbar
3. Test on different screen sizes:
   - Mobile (375px)
   - Tablet (768px)
   - Desktop (1920px)

**Expected Results**:
- ✓ Navigation collapses to hamburger on mobile
- ✓ Forms stack properly
- ✓ Cards arrange responsively
- ✓ Text readable on all sizes

### Automated Testing (Optional)

#### Unit Test Example
```csharp
public class EncryptionServiceTests
{
    [Fact]
    public void Encrypt_WithValidText_ReturnsEncryptedString()
    {
        // Arrange
        var service = new EncryptionService(CreateConfig());
        string plaintext = "mypassword";

        // Act
        string encrypted = service.Encrypt(plaintext);

        // Assert
        Assert.NotNull(encrypted);
        Assert.NotEqual(plaintext, encrypted);
    }

    [Fact]
    public void Encrypt_Decrypt_ReturnsOriginalText()
    {
        // Arrange
        var service = new EncryptionService(CreateConfig());
        string original = "mypassword";

        // Act
        string encrypted = service.Encrypt(original);
        string decrypted = service.Decrypt(encrypted);

        // Assert
        Assert.Equal(original, decrypted);
    }
}
```

## Deployment Guide

### Production Checklist

#### 1. Security Hardening
- [ ] Change encryption key in appsettings.json
- [ ] Change database credentials
- [ ] Enable HTTPS with valid certificate
- [ ] Set session timeout appropriately
- [ ] Enable CORS only for trusted domains
- [ ] Remove debug information from error pages

#### 2. Configuration Updates
```json
// appsettings.Production.json
{
  "EncryptionSettings": {
    "Key": "ChangeThisToA32CharacterSecureKey!!"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-server;Database=TooDooListDb;User Id=sa;Password=SecurePassword123;Encrypt=true;TrustServerCertificate=false;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

#### 3. Database Backup
```bash
# Backup database before deployment
BACKUP DATABASE [TooDooListDb] 
TO DISK = 'C:\Backups\TooDooListDb_backup.bak'
WITH INIT, COMPRESSION, CHECKSUM;
```

#### 4. Enable HTTPS

**For IIS**:
1. Install SSL certificate
2. Bind to HTTPS port (443)
3. Redirect HTTP to HTTPS

**For Self-Hosted**:
```csharp
// In Program.cs
app.UseHttpsRedirection();
app.UseHsts();  // HSTS header
```

#### 5. Logging Configuration
```csharp
builder.Logging.AddFile("logs/app-{Date}.txt");
builder.Logging.AddConsole();

// Access logs
StreamWriter sw = new StreamWriter("app_log.txt", true);
sw.WriteLine($"{DateTime.Now}: Application started");
```

### Deployment Options

#### Option 1: Deploy to IIS
```bash
# 1. Build for release
dotnet publish -c Release -o ./publish

# 2. Copy publish folder to IIS server
# 3. Create Application Pool (.NET CLR-less)
# 4. Create Website pointing to publish folder
# 5. Configure HTTPS binding
```

#### Option 2: Deploy to Azure
```bash
# 1. Install Azure CLI
# 2. Login to Azure
az login

# 3. Create resource group
az group create --name TooDooListRG --location eastus

# 4. Create App Service Plan
az appservice plan create --name TooDooListPlan --resource-group TooDooListRG --sku B1 --is-linux

# 5. Create web app
az webapp create --resource-group TooDooListRG --plan TooDooListPlan --name toodolist --runtime "DOTNET:8.0"

# 6. Deploy application
az webapp up --resource-group TooDooListRG --name toodolist --plan TooDooListPlan
```

#### Option 3: Docker Deployment
```dockerfile
# Create Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "TooDooList.dll"]
```

```bash
# Build and run
docker build -t toodolist:latest .
docker run -p 8080:80 toodolist:latest
```

### Post-Deployment Verification

#### 1. Application Health Check
```bash
curl -X GET https://yourapp.com/
# Should return HTTP 200 with homepage
```

#### 2. Database Connection
```bash
# Verify database is accessible
Your app should load without database errors
```

#### 3. SSL Certificate
```bash
# Verify HTTPS
openssl s_client -connect yourapp.com:443
```

#### 4. Performance Monitoring
```csharp
// Add Application Insights
builder.Services.AddApplicationInsightsTelemetry(configuration);

// Monitor
- Response times
- Error rates
- Database performance
- User sessions
```

### Troubleshooting Deployment Issues

#### Issue: "Database connection failed"
**Solution**:
- Verify connection string
- Check firewall rules
- Ensure database server is running
- Test connectivity from deployment server

#### Issue: "HTTPS certificate not trusted"
**Solution**:
- Use valid certificate from trusted CA
- Check certificate expiration
- Verify certificate matches domain

#### Issue: "Application crashes on startup"
**Solution**:
- Check event viewer for errors
- Review application logs
- Verify all dependencies installed
- Test locally with production config

#### Issue: "Performance degradation"
**Solution**:
- Enable caching
- Optimize database queries
- Monitor resource usage
- Scale application

## Monitoring & Maintenance

### Regular Tasks
- [ ] Daily: Check error logs
- [ ] Weekly: Review performance metrics
- [ ] Monthly: Update dependencies (security patches)
- [ ] Quarterly: Test disaster recovery
- [ ] Annually: Security audit

### Key Metrics to Monitor
```
- Page load time (target: < 2 seconds)
- Error rate (target: < 0.1%)
- Database query time (target: < 100ms)
- Active user sessions
- Memory usage
- CPU usage
```

### Scaling Considerations

**Vertical Scaling**:
- Increase server RAM/CPU
- Better for gradual growth

**Horizontal Scaling**:
- Load balance across multiple servers
- Requires session state management (Redis)
- Better for high traffic

**Database Scaling**:
- Implement read replicas
- Add indexing
- Consider data archival

## Backup & Disaster Recovery

### Backup Strategy
```bash
# Daily backup
BACKUP DATABASE [TooDooListDb] 
TO DISK = 'Z:\Backups\Daily\TooDooListDb_$(date +%Y%m%d).bak'

# Weekly full backup to offsite
# Monthly archive backup
```

### Recovery Procedures
```bash
# Point-in-time recovery
RESTORE DATABASE [TooDooListDb] 
FROM DISK = 'Z:\Backups\Daily\TooDooListDb_20240101.bak'
WITH RECOVERY
```

---

**Remember**: Always test thoroughly before deploying to production!
