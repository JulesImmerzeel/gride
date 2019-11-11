using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
    public class WorkData
    {
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public int Overtime { get; set; }
        public int Delay { get; set; }
        public bool Assigned { get; set; }
    }
}
