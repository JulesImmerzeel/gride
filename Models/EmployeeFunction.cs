using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
    public class EmployeeFunction
    {
        public int EmployeeFunctionID { get; set; }
        public int EmployeeID { get; set; }
        public int FunctionID { get; set; }
        public EmployeeModel Employee { get; set; }
        public Function Function { get; set; }
    }
}
