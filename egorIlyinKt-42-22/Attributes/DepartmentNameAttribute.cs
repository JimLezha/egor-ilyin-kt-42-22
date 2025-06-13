using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class DepartmentNameAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is string strValue)
        {
            if (Regex.IsMatch(strValue, @"^(Department |Кафедра ).*"))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult($"Поле {validationContext.DisplayName} должно начинаться с 'Department ' или 'Кафедра '.");
        }
        return new ValidationResult("Invalid value.");
    }
}