using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
	public class Shift
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public ulong ID { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
		[Key, ForeignKey("Location")]
		public uint LocationID { get; set; }
		[Key, ForeignKey("Function")]
		public uint FunctionID { get; set; }
		public byte MaxEmployees { get; set; }
		public virtual ICollection<Skill> Skills { get; set; }
	}
}
