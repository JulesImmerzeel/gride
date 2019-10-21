using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
    public class ShiftFunction
    {
        public int ShiftFunctionID { get; set; }
        public int ShiftID { get; set; }
        public int FunctionID {get;set;}
        public int MaxEmployees { get; set; } = 1;
        public Function Function { get; set; }
        public Shift Shift { get; set; }
    }
}
