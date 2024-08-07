using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.API.Models
{
    public class DateOfBirthValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateOfBirth)
            {
                
                if (dateOfBirth > DateTime.Now)
                {
                    return new ValidationResult(ErrorMessage ?? "Date of Birth cannot be in the future.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
