using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
	public class Work
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public ulong WorkID { get; set; }
		[Key, ForeignKey("Employee")]
		public uint EmployeeID { get; set; }
		[Key, ForeignKey("Shift")]
		public ulong ShiftID { get; set; }
		public int Difference { get; set; } = 0;
	}
}