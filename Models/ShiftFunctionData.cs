using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
    public class ShiftFunctionData
    {
        public int FunctionID { get; set; }
        public string Name { get; set; }
        public bool Assigned { get; set; }
        public int MaxEmployees { get; set; }
    }
}
