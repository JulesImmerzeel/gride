using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
    public class MaxEmployeesValidator : ValidationAttribute
    {
        public MaxEmployeesValidator(string paramName)
        {
            this.ParamName = paramName;
        }

        public string ParamName { get; private set; }

        private int maxEmployees = 0;
        private ICollection<ShiftFunction> functions;
        private ICollection<Work> works;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(this.ParamName);
            functions = (ShiftFunction[])property.GetValue(validationContext.ObjectInstance, null);

            Shift shift = (Shift)validationContext.ObjectInstance;

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
