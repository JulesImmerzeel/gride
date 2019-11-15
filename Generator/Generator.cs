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
        public enum filter
        {
            all,
            func,
            skill,
            location
        }
        public static List<EmployeeModel> Generate(Shift shift, ApplicationDbContext _context, filter filter = filter.location)
        {

            //GIJS ZIJN CODE

            /*List<EmployeeModel> Employees =
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
            return Employees;*/

            //Iedereen die kan werken.
            List<EmployeeModel> available = (from row in _context.Availabilities
                                             join ea in _context.EmployeeAvailabilities on row.AvailabilityID equals ea.AvailabilityID
                                             join employee in _context.EmployeeModel on ea.EmployeeID equals employee.ID
                                             where shift.Start >= row.Start && shift.End >= row.End
                                             select employee
                                               ).ToList();

            //Iedereen die kan werken en juiste functie heeft.
            List<EmployeeModel> function = (from employee in available
                         join ef in _context.EmployeeFunctions on employee.ID equals ef.EmployeeID
                         join func in _context.Function on ef.FunctionID equals func.FunctionID
                         join sf in _context.ShiftFunctions on func.FunctionID equals sf.FunctionID
                         where shift.ShiftFunctions.Contains(sf)
                         select employee).ToList();

            //Iedereen die kan werken, juiste functie heeft en skill.
            List<EmployeeModel> skill = (from employee in function
                                         join es in _context.EmployeeSkills on employee.ID equals es.EmployeeModelID
                                         join sk in _context.Skill on es.SkillID equals sk.SkillID
                                         join ss in _context.ShiftSkills on sk.SkillID equals ss.SkillID
                                         where shift.ShiftSkills.Contains(ss)
                                         select employee).ToList();

            //Iedereen die kan werken, juiste functie heeft en skill en locatie.
            List<EmployeeModel> location = (from employee in skill
                                            join el in _context.EmployeeLocations on employee.ID equals el.EmployeeModelID
                                            join loc in _context.Locations on el.LocationID equals loc.LocationID
                                            where el.LocationID == shift.LocationID
                                            select employee).ToList();

            //TODO experience filter toevoegen.

            if (filter == filter.all)
            {
                return available;
            }
            if (filter == filter.func)
            {
                return function;
            }
            if (filter == filter.skill)
            {
                return skill;
            }
            else
            {
                return location;
            }
        }
    }
}
