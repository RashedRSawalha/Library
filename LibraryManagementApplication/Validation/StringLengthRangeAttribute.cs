using System;
using System.ComponentModel.DataAnnotations;



namespace LibraryManagementApplication.Validation
{

    public class StringLengthRangeAttribute : ValidationAttribute
    {
        private readonly int _minLength;
        private readonly int _maxLength;

        public StringLengthRangeAttribute(int minLength, int maxLength)
        {
            _minLength = minLength;
            _maxLength = maxLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var strValue = value as string;

            if (string.IsNullOrEmpty(strValue))
            {
                return new ValidationResult("The field is required.");
            }

            if (strValue.Length < _minLength || strValue.Length > _maxLength)
            {
                return new ValidationResult($"The field must be between {_minLength} and {_maxLength} characters.");
            }

            return ValidationResult.Success;
        }
    }
}