using Gride.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Validators
{
    public class DoBValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance is EmployeeModel)
            {
                EmployeeModel employee = (EmployeeModel)validationContext.ObjectInstance;
                var dob = ((DateTime)value);

                if (dob <= DateTime.Now.AddYears(-100))
                {
                    return new ValidationResult(GetErrorMessage1());
                }

                if (dob > DateTime.Now.AddYears(-12))
                {
                    return new ValidationResult(GetErrorMessage2());
                }
            }

            return ValidationResult.Success;
        }
        public string GetErrorMessage1()
        {
            return "You can't be born more than 100 years ago";
        }
        public string GetErrorMessage2()
        {
            return "You have to be older than 12 years";
        }
    }
}
