using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Gride.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string FeedbackDescription { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime FeedbackPostDate { get; set; }
        public bool Fixed { get; set; }
    }
}
