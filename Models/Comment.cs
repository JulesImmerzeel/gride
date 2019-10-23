using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
    public class Comment
    {
        public int CommentID { get; set; }
        [Required]
        [StringLength(2000)]
        public string Text { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public Employee Employee { get; set; }
    }
}
