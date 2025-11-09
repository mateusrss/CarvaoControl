using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CarvaoControl.Application.Services
{
    public class AuthService
    {
        private readonly string _configPath;
        private readonly string _basePath;

        private class Config
        {
            public string AdminPasswordHash { get; set; } = string.Empty;
        }

        public AuthService(string? basePath = null)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _basePath = basePath ?? Path.Combine(appData, "CarvaoControl");
            Directory.CreateDirectory(_basePath);
            _configPath = Path.Combine(_basePath, "config.json");
            EnsureConfig();
        }

        private void EnsureConfig()
        {
            if (!File.Exists(_configPath))
            {
                var cfg = new Config { AdminPasswordHash = HashPassword("chamacarvao") };
                File.WriteAllText(_configPath, JsonSerializer.Serialize(cfg));
            }
        }

        private static string HashPassword(string plain)
        {
            byte[] salt = new byte[16];
            RandomNumberGenerator.Fill(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(plain, salt, 10000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);

            byte[] hashBytes = new byte[48]; // 16 bytes salt + 32 bytes hash
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);

            return Convert.ToBase64String(hashBytes);
        }

        private static bool VerifyPassword(string plain, string hashedPassword)
        {
            try
            {
                byte[] hashBytes = Convert.FromBase64String(hashedPassword);
                
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);

                using var pbkdf2 = new Rfc2898DeriveBytes(plain, salt, 10000, HashAlgorithmName.SHA256);
                byte[] hash = pbkdf2.GetBytes(32);

                for (int i = 0; i < 32; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ValidateAdmin(string password)
        {
            try
            {
                var json = File.ReadAllText(_configPath);
                var cfg = JsonSerializer.Deserialize<Config>(json);
                if (cfg == null || string.IsNullOrEmpty(cfg.AdminPasswordHash)) return false;
                return VerifyPassword(password, cfg.AdminPasswordHash);
            }
            catch
            {
                return false;
            }
        }

        public void SetAdminPassword(string newPassword)
        {
            try
            {
                var cfg = new Config { AdminPasswordHash = HashPassword(newPassword) };
                File.WriteAllText(_configPath, JsonSerializer.Serialize(cfg));
            }
            catch
            {
                // ignore
            }
        }
    }
}
