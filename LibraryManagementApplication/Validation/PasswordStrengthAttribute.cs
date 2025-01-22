using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


namespace LibraryManagementApplication.Validation
{
    public class PasswordStrengthAttribute : ValidationAttribute
    {
        private readonly int _minLength;

        public PasswordStrengthAttribute(int minLength = 6)
        {
            _minLength = minLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;

            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("Password is required.");
            }

            if (password.Length < _minLength)
            {
                return new ValidationResult($"Password must be at least {_minLength} characters long.");
            }

            // Check if the password contains at least one special character
            if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?\:{ }|<>]"))
            {
                return new ValidationResult("Password must contain at least one special character.");
            }

            return ValidationResult.Success;
        }
    }
}
