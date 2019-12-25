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
        public string Title { get; set; }
        public string FeedbackDescription { get; set; }
        [DataType(DataType.Date)]
        public DateTime FeedbackPostDate { get; set; }
    }
}
