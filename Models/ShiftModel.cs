using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
	public class ShiftModel
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public ulong ShiftID { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
		[Key]
		[ForeignKey("LocationModel")]
		public uint LocationID { get; set; }
		[Required]
		public List<int> Skills { get; set; } = new List<int>();
		[Key]
		[ForeignKey("FunctionModel")]
		public int FunctionID { get; set; }
		public byte MaxEmployees { get; set; }
	}
}
