using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class CapitalizedAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is string strValue)
        {
            if (Regex.IsMatch(strValue, @"^[A-Z].*"))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult($"Поле {validationContext.DisplayName} должно начинаться с заглавной буквы.");
        }
        return new ValidationResult("Invalid value.");
    }
}