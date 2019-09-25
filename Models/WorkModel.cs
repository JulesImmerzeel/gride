using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
	public class WorkModel
	{
		public ulong WorkID { get; set; }
		public uint EmployeeID { get; set; }
		public ulong ShiftID { get; set; }
		public int Difference { get; set; }
	}
}
