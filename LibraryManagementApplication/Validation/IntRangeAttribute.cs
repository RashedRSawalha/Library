
using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementApplication.Validation
{
    public class IntRangeAttribute : ValidationAttribute
    {
        private readonly int _minValue;
        private readonly int _maxValue;

        public IntRangeAttribute(int minValue, int maxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || !int.TryParse(value.ToString(), out int intValue))
            {
                return new ValidationResult("The field must be a valid number.");
            }

            if (intValue < _minValue || intValue > _maxValue)
            {
                return new ValidationResult($"The value must be between {_minValue} and {_maxValue}.");
            }

            return ValidationResult.Success;
        }
    }
}
