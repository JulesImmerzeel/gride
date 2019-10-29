using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
    public class MaxEmployeesValidator : ValidationAttribute
    {
        private int maxEmployees = 0;
        private ICollection<ShiftFunction> functions;
        private ICollection<Work> works;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Shift shift = (Shift)validationContext.ObjectInstance;
            functions = shift.ShiftFunctions;

            foreach (ShiftFunction function in functions)
            {
                maxEmployees += function.MaxEmployees;
            }
            works = shift.Works;
            if (works.Count > maxEmployees)
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }

        private string GetErrorMessage()
        {
            return "You can't select more employees than you've assigned functions for.";
        }
    }
}
