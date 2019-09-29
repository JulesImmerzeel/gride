using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
	public class EmployeeModel
	{
		public uint ID { get; set; }
		public string Name{ get; set; }
        [Display(Name="Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DoB { get; set; }
		public Gender Gender { get; set; }
        [Display(Name = "E-mail address")]
        public string EMail { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
		public bool Admin { get; set; }
		public ulong Skills { get; set; }
		public int Function { get; set; }
		public ulong LoginID { get; set; }
		public float Experience { get; set; }
		public uint Locations { get; set; }
        [Display(Name = "Profile Image")]
        public string ProfileImage { get; set; }
    }

	public enum Gender
	{
		Male, Female, Not_Specified
	}
}
