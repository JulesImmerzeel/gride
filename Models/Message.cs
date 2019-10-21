using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
	public class Message
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long MessageID { get; set; }
		[Key, ForeignKey("Employee")]
		public long EmployeeID { get; set; }
		[Required, StringLength(2000)]
		public string Text { get; set; }
		public DateTime Time { get; set; } = DateTime.Now;

		public virtual Employee Employee { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } 
	}
}
