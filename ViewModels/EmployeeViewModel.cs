using Gride.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// EmployeeViewModel is a view model based on the Employee Model
/// The only difference between the viewModel and the model is that
/// the type of profile image is changed from "String" to "IFormFile".
/// 
/// This because, this viewmodel is used with changing the employee profile image.
/// 
/// Examples of this being implemented can be found in the edit task of both UserController and EmployeeCOntroller
/// </summary>
namespace Gride.ViewModels
{
    /// <summary>
    /// Representes the view model of the employee model
    /// </summary>
    public class EmployeeViewModel
    {
        public int ID { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DoB { get; set; }
        public Gender Gender { get; set; } = Gender.Not_Specified;
        [Required]
        [StringLength(100)]
        [EmailAddress]
        [Display(Name = "E-mail address")]
        public string EMail { get; set; }
        [Required]
        [StringLength(12)]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public int? SupervisorID { get; set; }
        public bool Admin { get; set; } = false;
        public float Experience { get; set; }
        public IFormFile ProfileImage { get; set; } = null; // string is IFormFile.

        public ICollection<EmployeeSkill> EmployeeSkills { get; set; }
        public ICollection<EmployeeFunction> EmployeeFunctions { get; set; }
        public ICollection<EmployeeLocations> EmployeeLocations { get; set; }
        public ICollection<EmployeeAvailability> EmployeeAvailabilities { get; set; }
    }
    public enum Gender
    {
        Male, Female, Not_Specified
    
    }
}
