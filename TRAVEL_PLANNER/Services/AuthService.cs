using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TRAVEL_PLANNER.Models;

namespace TRAVEL_PLANNER.Services
{
    public static class AuthService
    {
        private static readonly string StorageDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TRAVEL_PLANNER");

        private static readonly string UsersPath = Path.Combine(StorageDirectory, "users.json");
        private static readonly string SessionPath = Path.Combine(StorageDirectory, "session.json");

        public static void EnsureStorage()
        {
            Directory.CreateDirectory(StorageDirectory);
        }

        public static bool TryRegister(string email, string password, out string errorMessage)
        {
            var normalizedEmail = NormalizeEmail(email);
            errorMessage = ValidateCredentials(normalizedEmail, password);
            if (errorMessage != null)
            {
                return false;
            }

            var users = LoadUsers();
            if (users.Any(item => string.Equals(item.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase)))
            {
                errorMessage = "Користувач із такою електронною поштою вже існує.";
                return false;
            }

            users.Add(new UserAccount
            {
                Email = normalizedEmail,
                PasswordHash = ComputeHash(password),
                CreatedAtUtc = DateTime.UtcNow
            });

            SaveUsers(users);
            RememberSession(normalizedEmail);
            errorMessage = null;
            return true;
        }

        public static bool TrySignIn(string email, string password, out string errorMessage)
        {
            var normalizedEmail = NormalizeEmail(email);
            errorMessage = ValidateCredentials(normalizedEmail, password);
            if (errorMessage != null)
            {
                return false;
            }

            var user = LoadUsers().FirstOrDefault(item =>
                string.Equals(item.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                errorMessage = "Користувача з такою електронною поштою не знайдено";
                return false;
            }

            if (!string.Equals(user.PasswordHash, ComputeHash(password), StringComparison.Ordinal))
            {
                errorMessage = "Неправильний пароль";
                return false;
            }

            RememberSession(normalizedEmail);
            errorMessage = null;
            return true;
        }

        public static string RestoreSession()
        {
            EnsureStorage();
            if (!File.Exists(SessionPath))
            {
                return null;
            }

            try
            {
                var json = File.ReadAllText(SessionPath, Encoding.UTF8);
                var session = JsonSerializer.Deserialize<AppSession>(json);
                var email = NormalizeEmail(session?.UserEmail);
                return LoadUsers().Any(item => string.Equals(item.Email, email, StringComparison.OrdinalIgnoreCase))
                    ? email
                    : null;
            }
            catch
            {
                return null;
            }
        }

        public static void SignOut()
        {
            EnsureStorage();
            if (File.Exists(SessionPath))
            {
                File.Delete(SessionPath);
            }
        }

        public static List<SavedTrip> LoadTrips(string email)
        {
            var path = BuildTripsPath(email);
            if (!File.Exists(path))
            {
                return new List<SavedTrip>();
            }

            try
            {
                var json = File.ReadAllText(path, Encoding.UTF8);
                return JsonSerializer.Deserialize<List<SavedTrip>>(json) ?? new List<SavedTrip>();
            }
            catch
            {
                return new List<SavedTrip>();
            }
        }

        public static void SaveTrips(string email, IEnumerable<SavedTrip> trips)
        {
            var path = BuildTripsPath(email);
            var json = JsonSerializer.Serialize(trips?.ToList() ?? new List<SavedTrip>(), new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(path, json, Encoding.UTF8);
        }

        private static string ValidateCredentials(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return "Введіть адресу електронної пошти";
            }

            if (!email.Contains("@") || !email.Contains("."))
            {
                return "Введіть коректну адресу електронної пошти";
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return "Введіть пароль";
            }

            if (password.Length < 6)
            {
                return "Пароль має містити щонайменше 6 символів";
            }

            return null;
        }

        private static List<UserAccount> LoadUsers()
        {
            EnsureStorage();
            if (!File.Exists(UsersPath))
            {
                return new List<UserAccount>();
            }

            try
            {
                var json = File.ReadAllText(UsersPath, Encoding.UTF8);
                return JsonSerializer.Deserialize<List<UserAccount>>(json) ?? new List<UserAccount>();
            }
            catch
            {
                return new List<UserAccount>();
            }
        }

        private static void SaveUsers(IEnumerable<UserAccount> users)
        {
            EnsureStorage();
            var json = JsonSerializer.Serialize(users?.ToList() ?? new List<UserAccount>(), new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(UsersPath, json, Encoding.UTF8);
        }

        private static void RememberSession(string email)
        {
            EnsureStorage();
            var json = JsonSerializer.Serialize(new AppSession
            {
                UserEmail = NormalizeEmail(email)
            }, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(SessionPath, json, Encoding.UTF8);
        }

        private static string BuildTripsPath(string email)
        {
            EnsureStorage();
            var normalizedEmail = NormalizeEmail(email);
            var hash = ComputeHash(normalizedEmail).Substring(0, 16);
            return Path.Combine(StorageDirectory, $"trips_{hash}.json");
        }

        private static string NormalizeEmail(string email)
        {
            return string.IsNullOrWhiteSpace(email) ? string.Empty : email.Trim().ToLowerInvariant();
        }

        private static string ComputeHash(string value)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value ?? string.Empty));
                var builder = new StringBuilder(bytes.Length * 2);
                foreach (var item in bytes)
                {
                    builder.Append(item.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}