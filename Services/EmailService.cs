using System.Net;
using System.Net.Mail;

namespace TooDooList.Services
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string email, string name);
        Task SendTaskCreatedEmailAsync(string email, string userName, string taskTitle);
        Task SendBookingCreatedEmailAsync(string email, string userName, string serviceName);
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = false);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendWelcomeEmailAsync(string email, string name)
        {
            string subject = "Welcome to Indepth Clean!";
            string body = $@"
            <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #2ecc71;'><i class='fas fa-spray-can'></i> Welcome to Indepth Clean, {name}!</h2>
                        
                        <p>Thank you for registering with us. We're excited to have you on board!</p>
                        
                        <p>With Indepth Clean, you can:</p>
                        <ul>
                            <li>Book professional cleaning services at your convenience</li>
                            <li>Track your service bookings and status in real-time</li>
                            <li>Enjoy a spotless home and spaces</li>
                            <li>Manage multiple service types: Laundry, Dry Cleaning, Deep Cleaning, and Sofa Cleaning</li>
                        </ul>
                        
                        <p>
                            <a href='https://localhost:5001/Account/Login' 
                               style='background-color: #2ecc71; color: white; padding: 10px 20px; 
                                      text-decoration: none; border-radius: 5px; display: inline-block;'>
                                Log In Now
                            </a>
                        </p>
                        
                        <p>If you have any questions, feel free to reach out to our support team.</p>
                        
                        <p>Best regards,<br/>The Indepth Clean Team</p>
                    </div>
                </body>
            </html>";

            await SendEmailAsync(email, subject, body, isHtml: true);
        }

        public async Task SendTaskCreatedEmailAsync(string email, string userName, string taskTitle)
        {
            string subject = "New Task Created - TooDooList";
            string body = $@"
            <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #667eea;'>Task Created Successfully</h2>
                        
                        <p>Hi {userName},</p>
                        
                        <p>A new task has been created in your TooDooList:</p>
                        
                        <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #667eea; margin: 20px 0;'>
                            <strong>Task Title:</strong> {taskTitle}<br/>
                            <strong>Created Date:</strong> {DateTime.Now:MMMM dd, yyyy HH:mm}<br/>
                            <strong>Status:</strong> Ongoing
                        </div>
                        
                        <p>
                            <a href='https://localhost:5001/Dashboard' 
                               style='background-color: #667eea; color: white; padding: 10px 20px; 
                                      text-decoration: none; border-radius: 5px; display: inline-block;'>
                                View Your Tasks
                            </a>
                        </p>
                        
                        <p>Keep up the good work!</p>
                        
                        <p>Best regards,<br/>The TooDooList Team</p>
                    </div>
                </body>
            </html>";

            await SendEmailAsync(email, subject, body, isHtml: true);
        }

        public async Task SendBookingCreatedEmailAsync(string email, string userName, string serviceName)
        {
            string subject = "New Service Booking Created - Indepth Clean";
            string body = $@"
            <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #2ecc71;'>Booking Confirmed!</h2>
                        
                        <p>Hi {userName},</p>
                        
                        <p>Your cleaning service booking has been successfully created:</p>
                        
                        <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #2ecc71; margin: 20px 0;'>
                            <strong>Service:</strong> {serviceName}<br/>
                            <strong>Booking Date:</strong> {DateTime.Now:MMMM dd, yyyy HH:mm}<br/>
                            <strong>Status:</strong> Pending
                        </div>
                        
                        <p>We'll notify you once your service is confirmed.</p>
                        
                        <p>
                            <a href='https://localhost:5001/Dashboard' 
                               style='background-color: #2ecc71; color: white; padding: 10px 20px; 
                                      text-decoration: none; border-radius: 5px; display: inline-block;'>
                                View Your Bookings
                            </a>
                        </p>
                        
                        <p>Thank you for choosing Indepth Clean!</p>
                        
                        <p>Best regards,<br/>The Indepth Clean Team</p>
                    </div>
                </body>
            </html>";

            await SendEmailAsync(email, subject, body, isHtml: true);
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                string? smtpServer = smtpSettings["Server"];
                int smtpPort = int.Parse(smtpSettings["Port"] ?? "587");
                string? smtpUsername = smtpSettings["Username"];
                string? smtpPassword = smtpSettings["Password"];
                string? fromEmail = smtpSettings["FromEmail"];
                bool enableSsl = bool.Parse(smtpSettings["EnableSSL"] ?? "true");

                // Validate SMTP settings
                if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(smtpUsername) || 
                    string.IsNullOrEmpty(smtpPassword) || string.IsNullOrEmpty(fromEmail))
                {
                    _logger.LogWarning("SMTP settings are not configured. Email not sent to {Email}", to);
                    return;
                }

                using (var client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.EnableSsl = enableSsl;
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                    using (var message = new MailMessage(fromEmail, to))
                    {
                        message.Subject = subject;
                        message.Body = body;
                        message.IsBodyHtml = isHtml;

                        await client.SendMailAsync(message);
                        _logger.LogInformation("Email sent successfully to {Email}", to);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}", to);
                // Don't throw - email is non-critical
            }
        }
    }
}
