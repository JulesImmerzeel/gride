using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
	public class AvailabilitieModel
	{
		public uint EmployeeID { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
		public uint LocationID { get; set; }
		public bool Prefered { get; set; }
	}
}
