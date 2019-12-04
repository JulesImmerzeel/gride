using Gride.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Data
{
    public class InitialData
    {
        public List<EmployeeModel> employees = new List<EmployeeModel>();
        public List<Availability> availabilities = new List<Availability>();
        public List<EmployeeAvailability> employeeAvailabilities = new List<EmployeeAvailability>();
        public List<Skill> skills = new List<Skill>();
        public List<EmployeeSkill> employeeSkills = new List<EmployeeSkill>();
        public List<Function> functions = new List<Function>();
        public List<EmployeeFunction> employeeFunctions = new List<EmployeeFunction>();
        public List<Location> locations = new List<Location>();
        public List<EmployeeLocations> employeeLocations = new List<EmployeeLocations>();
        public List<Shift> shifts = new List<Shift>();
        public List<ShiftSkills> shiftSkills = new List<ShiftSkills>();
        public List<ShiftFunction> shiftFunctions = new List<ShiftFunction>();

        public List<EmployeeAvailability> SetEmployeeAvailabilities(ApplicationDbContext context)
        {
            employees = context.EmployeeModel.ToList();
            availabilities = context.Availabilities.ToList();

            foreach(EmployeeModel employee in employees)
            {
                int t = 20;
                int range = availabilities.Count() - 1;
                for (int i = 0; i < t; i++)
                {
                    int id = RandomID(range)+1;
                    EmployeeAvailability ea = new EmployeeAvailability { AvailabilityID = id, EmployeeID = employee.ID };
                    employeeAvailabilities.Add(ea);
                }
                counts.Clear();
            }
            return employeeAvailabilities;
        }

        public List<EmployeeSkill> SetEmployeeSkills(ApplicationDbContext context)
        {
            employees = context.EmployeeModel.ToList();
            skills = context.Skill.ToList();

            foreach (EmployeeModel employee in employees)
            {
                int t = 3;
                int range = skills.Count() - 1;
                for (int i = 0; i < t; i++)
                {
                    int id = RandomID(range) + 1;
                    EmployeeSkill es = new EmployeeSkill { SkillID = id, EmployeeModelID = employee.ID };
                    employeeSkills.Add(es);
                }
                counts.Clear();
            }
            return employeeSkills;
        }

        public List<EmployeeFunction> SetEmployeeFunctions(ApplicationDbContext context)
        {
            employees = context.EmployeeModel.ToList();
            functions = context.Function.ToList();

            foreach (EmployeeModel employee in employees)
            {
                int t = 1;
                int range = functions.Count() - 1;
                for (int i = 0; i < t; i++)
                {
                    int id = RandomID(range) + 1;
                    EmployeeFunction ef = new EmployeeFunction { FunctionID = id, EmployeeID = employee.ID };
                    employeeFunctions.Add(ef);
                }
                counts.Clear();
            }
            return employeeFunctions;
        }
        public List<EmployeeLocations> SetEmployeeLocations(ApplicationDbContext context)
        {
            employees = context.EmployeeModel.ToList();
            locations = context.Locations.ToList();

            foreach (EmployeeModel employee in employees)
            {
                int t = 3;
                int range = locations.Count() - 1;
                for (int i = 0; i < t; i++)
                {
                    int id = RandomID(range) + 1;
                    EmployeeLocations el = new EmployeeLocations { LocationID = id, EmployeeModelID = employee.ID };
                    employeeLocations.Add(el);
                }
                counts.Clear();
            }
            return employeeLocations;
        }

        public List<Shift> SetShifts(ApplicationDbContext context)
        {
            availabilities = context.Availabilities.ToList();
            locations = context.Locations.ToList();
            List<Shift> tmpShifts = new List<Shift>();
            List<Shift> tmpLocationShifts = new List<Shift>();

            foreach (Availability a in availabilities)
            {
                DateTime start = a.Start;
                DateTime end = a.End;
                tmpLocationShifts.Add(new Shift { Start = start, End = end });
                shifts.Add(new Shift { Start = start, End = end });
            }
            foreach (Shift s in shifts)
            {
                for (int j = 1; j < locations.Count(); j++)
                {
                    tmpShifts.Add(new Shift { Start = s.Start, End = s.End , LocationID = j });
                }
                s.LocationID = 4;
            }
            for (int i = 1; i <= 6; i++)
            {
                foreach (Shift s in tmpShifts)
                {
                    shifts.Add(new Shift { Start = s.Start.AddDays(i * 7), End = s.End.AddDays(i * 7), LocationID = s.LocationID});
                }
            }
            return shifts;
        }

        public List<ShiftSkills> SetShiftSkills(ApplicationDbContext context)
        {
            shifts = context.Shift.ToList();
            skills = context.Skill.ToList();

            foreach (Shift shift in shifts)
            {
                int t = 2;
                int range = skills.Count() - 1;
                for (int i = 0; i < t; i++)
                {
                    int id = RandomID(range) + 1;
                    ShiftSkills ss = new ShiftSkills { SkillID = id, ShiftID = shift.ShiftID };
                    shiftSkills.Add(ss);
                }
                counts.Clear();
            }
            return shiftSkills;
        }

        public List<ShiftFunction> SetShiftFunctions(ApplicationDbContext context)
        {
            shifts = context.Shift.ToList();
            functions = context.Function.ToList();

            foreach (Shift shift in shifts)
            {
                int t = 3;
                Random r = new Random();
                int range = functions.Count() - 1;
                for (int i = 0; i < t; i++)
                {
                    int id = RandomID(range) + 1;
                    int maxEmployees = r.Next(1, 3);
                    ShiftFunction sf = new ShiftFunction { FunctionID = id, ShiftID = shift.ShiftID, MaxEmployees = maxEmployees };
                    shiftFunctions.Add(sf);
                }
                counts.Clear();
            }
            return shiftFunctions;
        }


        List<int> counts = new List<int>();
        Random rnd = new Random();
        public int RandomID(int range)
        {
            if (range <= 0)
                return 1;

            int cnt = rnd.Next(range);
            foreach (int c in counts)
            {
                if (cnt == c)
                {
                    return RandomID(range);
                }
            }
            counts.Add(cnt);
            return cnt;
        }
    }
}
