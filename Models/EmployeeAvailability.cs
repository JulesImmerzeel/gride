using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
    public class EmployeeAvailability
    {
        public int EmployeeAvailabilityID { get; set; }
        public int EmployeeID { get; set; }
        public int AvailabilityID { get; set; }
        public EmployeeModel Employee { get; set; }
        public Availability Availability { get; set; }
    }
}
