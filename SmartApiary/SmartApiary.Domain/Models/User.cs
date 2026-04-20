using SmartApiary.Domain.Enums;
using System;

namespace SmartApiary.Domain.Models
{
    public class User
    {
        public Guid Id { get; private set; }
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public UserRole Role { get; private set; }
        public bool IsActive { get; private set; }
        public string? ActivationToken { get; private set; }
        public DateTime? ActivationTokenExpiresAtUtc { get; private set; }
        public string? ResetPasswordToken { get; private set; }
        public DateTime? ResetPasswordTokenExpiresAtUtc { get; private set; }
        public bool IsSuspended { get; private set; }
        public string? PhoneNumber { get; private set; } // Dodato polje za broj telefona
        private User() { }

        public static User Create(
            string firstName,
            string lastName,
            string email,
            UserRole role,
            string activationToken,
            DateTime expiresAtUtc,
            string? phoneNumber = null)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Role = role,
                IsActive = false,
                ActivationToken = activationToken,
                ActivationTokenExpiresAtUtc = expiresAtUtc,
                PhoneNumber = phoneNumber
            };
        }

        public void Activate(string passwordHash)
        {
            PasswordHash = passwordHash;
            IsActive = true;
            ActivationToken = null;
            ActivationTokenExpiresAtUtc = null;
        }

        public void GenerateResetPasswordToken()
        {
            ResetPasswordToken = Guid.NewGuid().ToString();
            ResetPasswordTokenExpiresAtUtc = DateTime.UtcNow.AddHours(1);
        }

        public void ResetPassword(string passwordHash)
        {
            PasswordHash = passwordHash;
            ResetPasswordToken = null;
            ResetPasswordTokenExpiresAtUtc = null;
        }

        public void Suspend()
        {
            IsSuspended = true;
        }
    }
}