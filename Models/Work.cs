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
		public int WorkID { get; set; }
		public int EmployeeID { get; set; }
		public int ShiftID { get; set; }
		public int FunctionID { get; set; }
		public int Overtime { get; set; } = 0;
        public int Delay { get; set; } = 0;

        public EmployeeModel Employee { get; set; }
        public Shift Shift { get; set; }
		public Function Function { get; set; }
    }
}
