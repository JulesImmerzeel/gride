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
		public int ShiftID { get; set; }
		public DateTime Start { get; set; }
        [EndDateValidator]
        public DateTime End { get; set; }
        public int LocationID { get; set; }

        public Location Location { get; set; }
        public ICollection<ShiftFunction> ShiftFunctions { get; set; }
        public ICollection<ShiftSkills> ShiftSkills { get; set; }
        public ICollection<Work> Works { get; set; }
	}
}
