using System.Security.Cryptography;
using System.Text;

namespace TooDooList.Services
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }

    public class EncryptionService : IEncryptionService
    {
        private readonly string _encryptionKey;

        public EncryptionService(IConfiguration configuration)
        {
            _encryptionKey = configuration["EncryptionSettings:Key"] ?? "MySecretKey12345MySecretKey12345";
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                    // Combine IV + encrypted data
                    byte[] result = new byte[aes.IV.Length + encryptedBytes.Length];
                    Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
                    Buffer.BlockCopy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);

                    return Convert.ToBase64String(result);
                }
            }
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            try
            {
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (var aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    // Extract IV from the beginning
                    byte[] iv = new byte[aes.IV.Length];
                    Buffer.BlockCopy(buffer, 0, iv, 0, iv.Length);
                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        byte[] encryptedBytes = new byte[buffer.Length - iv.Length];
                        Buffer.BlockCopy(buffer, iv.Length, encryptedBytes, 0, encryptedBytes.Length);

                        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
