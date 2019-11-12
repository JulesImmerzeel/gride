using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gride.Models;
using Gride.Data;

namespace Gride.Generator
{
    public static class Generator
    {
        public static List<EmployeeModel> Generate(Shift shift, ApplicationDbContext _context)
        {
            List<EmployeeModel> Employees =
                (from Employee in _context.EmployeeModel
                where (from EmployeeAvailability in _context.EmployeeAvailabilities
                       where EmployeeAvailability.EmployeeID == Employee.ID
                       select EmployeeAvailability.Availability).ToList().Exists(x => x.Start <= shift.Start && x.End >= shift.End)
                       select Employee).ToList();
            Employees = 
                (from Employee  in Employees
                where (from EmployeeFunction in _context.EmployeeFunctions
                       where EmployeeFunction.EmployeeID == Employee.ID
                       select EmployeeFunction.Function).ToList().Exists(x => shift.ShiftFunctions.ToList().Exists(y => y.FunctionID == x.FunctionID))
                select Employee).ToList();
            return Employees;
        }
    }
}
