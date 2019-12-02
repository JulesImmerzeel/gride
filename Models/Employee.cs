using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Gride.Models
{
	public class EmployeeModel
	{
		public int ID { get; set; }
		[Required]
		[StringLength(50)]
		public string Name{ get; set; }
		[Required]
		[StringLength(50)]
        [Display(Name="Last Name")]
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
		public string ProfileImage { get; set; } = null;

        public ICollection<EmployeeSkill> EmployeeSkills { get; set; }
		public ICollection<EmployeeFunction> EmployeeFunctions { get; set; }
        public ICollection<EmployeeLocations> EmployeeLocations { get; set; }
        public ICollection<EmployeeAvailability> EmployeeAvailabilities { get; set; }

        public WorkOverview Workoverview;

    }
	public enum Gender
	{
		Male, Female, Not_Specified
	}
}
