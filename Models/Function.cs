using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
	public class Function
	{
		public int FunctionID { get; set; }
        [StringLength(50)]
        [Display(Name = "Function")]
        public string Name { get; set; }
        public uint EmployeeModelID { get; set; }
	}
}
