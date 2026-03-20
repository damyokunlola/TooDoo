using TooDooList.Data;
using TooDooList.Models;

namespace TooDooList.Services
{
    public interface IAuthenticationService
    {
        Task<bool> RegisterUserAsync(string name, string email, int age, string profession, string password);
        Task<User?> LoginUserAsync(string email, string password);
        Task<User?> GetUserByIdAsync(int userId);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEncryptionService _encryptionService;
        private readonly IEmailService _emailService;

        public AuthenticationService(ApplicationDbContext context, IEncryptionService encryptionService, IEmailService emailService)
        {
            _context = context;
            _encryptionService = encryptionService;
            _emailService = emailService;
        }

        public async Task<bool> RegisterUserAsync(string name, string email, int age, string profession, string password)
        {
            // Check if email already exists
            if (_context.Users.Any(u => u.Email == email))
                return false;

            // Encrypt password
            string encryptedPassword = _encryptionService.Encrypt(password);

            var user = new User
            {
                Name = name,
                Email = email,
                Age = age,
                Profession = profession,
                EncryptedPassword = encryptedPassword,
                CreatedDate = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Send welcome email (fire and forget)
            _ = _emailService.SendWelcomeEmailAsync(email, name);

            return true;
        }

        public async Task<User?> LoginUserAsync(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return null;

            // Decrypt and compare passwords
            string decryptedPassword = _encryptionService.Decrypt(user.EncryptedPassword);
            if (decryptedPassword == password)
                return user;

            return null;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await Task.FromResult(_context.Users.FirstOrDefault(u => u.Id == userId));
        }
    }
}
