using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
	public class MessageModel
	{
		public ulong MessageID { get; set; }
		public uint EmployeeID { get; set; }
		public string Message { get; set; }
		public DateTime Time { get; set; } = DateTime.Now;
	}
}
