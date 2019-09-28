using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
	public class EmployeeModel
	{
		public uint ID { get; set; }
		public string Name{ get; set; }
		public string LastName { get; set; }
		public DateTime DoB { get; set; }
		public Gender Gender { get; set; }
		public string EMail { get; set; }
		public string PhoneNumber { get; set; }
		public bool Admin { get; set; }
		public ulong Skills { get; set; }
		public int Function { get; set; }
		public ulong LoginID { get; set; }
		public float Experience { get; set; }
		public uint Locations { get; set; }
		public string ProfileImage { get; set; }
	}

	public enum Gender
	{
		Male, Female, Not_Specified
	}
}
