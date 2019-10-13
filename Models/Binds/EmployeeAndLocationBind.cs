using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models.Binds
{
	public class EmployeeAndLocationBind
	{
		[Key, ForeignKey("Employee")]
		public long EmployeeID { get; set; }

		[Key, ForeignKey("Location")]
		public int LocationID { get; set; }
	}
}
