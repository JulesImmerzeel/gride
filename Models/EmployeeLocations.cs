using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
    public class EmployeeLocations
    {
        public int EmployeeLocationsID { get; set; }
        public int EmployeeModelID { get; set; }
        public int LocationID { get; set; }
        public EmployeeModel Employee { get; set; }
        public Location Location { get; set; }
    }
}
