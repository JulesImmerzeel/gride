using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models.Binds
{
	public class EmployeeAndFunctionBind
	{
		[Key, ForeignKey("Employee")]
		public uint EmployeeID { get; set; }

		[Key, ForeignKey("Function")]
		public int FunctionID { get; set; }
	}
}
