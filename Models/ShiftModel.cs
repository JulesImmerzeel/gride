using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
	public class ShiftModel
	{
		public ulong ShiftID { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
		public uint LocationID { get; set; }
		public ulong Skills { get; set; }
		public int Function { get; set; }
		public byte MaxEmployees { get; set; }
	}
}
