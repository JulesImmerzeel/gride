using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models.Binds
{
	public class ShiftAndFunctionBind
	{
		[Key, ForeignKey("Shift")]
		public long ShiftID { get; set; }

		[Key, ForeignKey("Function")]
		public int FunctionID { get; set; }

		public byte maxEmployees { get; set; }

		public virtual Shift Shift { get; set; }
		public virtual Function Function { get; set; }
	}
}