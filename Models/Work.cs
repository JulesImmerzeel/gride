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
		public long WorkID { get; set; }
		[Key, ForeignKey("Employee")]
		public long EmployeeID { get; set; }
		[Key, ForeignKey("Shift")]
		public long ShiftID { get; set; }
		[Key, ForeignKey("Function")]
		public int FunctionID { get; set; }
		public int Difference { get; set; } = 0;

		public virtual Employee Employee { get; set; }
		public virtual Shift Shift { get; set; }
		public virtual Function Function { get; set; }
	}
}