using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
    public class EmployeeSkill
    {
        public int EmployeeSkillID { get; set; }
        public int EmployeeModelID { get; set; }
        public int SkillID { get; set; }
        public EmployeeModel Employee { get; set; }
        public Skill Skill { get; set; }
    }
}
