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
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public uint FunctionID { get; set; }
		public string Name { get; set; }
	}
}
