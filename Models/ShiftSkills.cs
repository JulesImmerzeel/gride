using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
    public class ShiftSkills
    {
        public int ShiftskillsID { get; set; }
        public int ShiftID { get; set; }
        public int SkillID { get; set; }
        public Shift Shift { get; set; }
        public Skill Skill { get; set; }
    }
}
