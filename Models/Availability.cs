using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Gride.Models
{
	public class Availability
	{
		[Key, ForeignKey("Employee")]
		public uint EmployeeID { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
		[Key, ForeignKey("Location")]
		public uint LocationID { get; set; }
		public bool Prefered { get; set; } = true;
	}
}
