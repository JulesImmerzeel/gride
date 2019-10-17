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
		public int MessageID { get; set; }
		[Required]
		[StringLength(2000)]
		public string Text { get; set; }
		public DateTime Time { get; set; } = DateTime.Now;
        public ICollection<Comment> Comments { get; set; } 
        public EmployeeModel Employee { get; set; }
    }
}
