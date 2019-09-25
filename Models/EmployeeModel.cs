using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gride.Models
{
	public class EmployeeModel
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public uint EmployeeID { get; set; }
		[Required]
		[StringLength(50)]
		public string Name{ get; set; }
		[Required]
		[StringLength(50)]
		public string LastName { get; set; }
		public DateTime DoB { get; set; }
		public Gender Gender { get; set; } = Gender.Not_Specified;
		[Required]
		[StringLength(100)]
		[EmailAddress]
		public string EMail { get; set; }
		[Required]
		[StringLength(12)]
		[Phone]
		public string PhoneNumber { get; set; }
		public bool Admin { get; set; } = false;
		public ulong Skills { get; set; }
		public int Function { get; set; }
		public ulong LoginID { get; set; }
		public float Experience { get; set; }
		public uint Locations { get; set; }
		[RegularExpression(@"(\\\\?([^\\/]*[\\/])*)([^\\/]+)$", ErrorMessage = "Path to ProfileImage is not a valid path")]
		public string ProfileImage { get; set; } = null;
	}

	public enum Gender
	{
		Male, Female, Not_Specified
	}
}
