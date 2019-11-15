using Gride.Data;
using Gride.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using static System.MathF;

namespace Gride.Generator
{
	public static class Generator
	{
		/// <summary>
		/// Creates a list people that are the best for the job.
		/// </summary>
		/// <param name="shift">The shift where the employees should be assigned too</param>
		/// <param name="_context">The database context</param>
		/// <param name="result">The result of the generation</param>
		/// <param name="avgExp">The average exp of the team (ignored when <see cref="GeneratorSettings.PreferHigerExp"/> is enabled)</param>
		/// <param name="settings">The setting for the generation</param>
		/// <remarks>result is set whenever possible even if en exception where to occur</remarks>
		/// <exception cref="NotEnoughStaffException">Thrown when not enough staff is available</exception>
		// TODO: Make it so you can see who is assigned to what function
		// TODO: Make it so you can get every exception that would have been thrown
#if DEBUG 
		public static void Generate(Shift shift, ApplicationDbContext _context, out List<EmployeeModel> result, float avgExp = 2, GeneratorSettings settings = GeneratorSettings.StopOnError)
#else
		public static void Generate(Shift shift, ApplicationDbContext _context, out List<EmployeeModel> result, float avgExp = 2, GeneratorSettings settings = GeneratorSettings.Default)
#endif
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

			// Checks if settings is viable
			if ((settings & (GeneratorSettings.PreferLowerExp | GeneratorSettings.PreferHigerExp)) == (GeneratorSettings.PreferLowerExp | GeneratorSettings.PreferHigerExp))
				throw new ArgumentException("cannot have both PreferLowerEp and PreferHigherExp selected please chose one or none", "settings");


			// A temporary list for operations
			List<EmployeeModel> cresult = new List<EmployeeModel>();

			foreach (ShiftFunction func in _context.ShiftFunctions.ToList().FindAll(x => x.ShiftID == shift.ShiftID))
			{

				//Iedereen die kan werken.
				List<EmployeeModel> available = (from row in _context.Availabilities
												 join ea in _context.EmployeeAvailabilities on row.AvailabilityID equals ea.AvailabilityID
												 join employee in _context.EmployeeModel on ea.EmployeeID equals employee.ID
												 where shift.Start >= row.Start && shift.End >= row.End
												 select employee
												   ).ToList();

				// Removes everyone that is selected for other functions
				// TODO: Also check other shifts
				if ((settings & GeneratorSettings.AllowDoubleBooking) == 0)
					available = (from possible in available
								 where !cresult.Contains(possible)
								 select possible).ToList();

				//Iedereen die kan werken en juiste functie heeft.
				List<EmployeeModel> function = (from employee in available
												join ef in _context.EmployeeFunctions on employee.ID equals ef.EmployeeID
												join funct in _context.Function on ef.FunctionID equals funct.FunctionID
												join sf in _context.ShiftFunctions on funct.FunctionID equals sf.FunctionID
												where shift.ShiftFunctions.Contains(func)
												select employee).ToList();

				//Iedereen die kan werken, juiste functie heeft en locatie.
				List<EmployeeModel> location = (from employee in function
												join el in _context.EmployeeLocations on employee.ID equals el.EmployeeModelID
												join loc in _context.Locations on el.LocationID equals loc.LocationID
												where el.LocationID == shift.LocationID
												select employee).ToList();

				// Checks if enough people are available for that time and location
				if (func.MaxEmployees > location.Count)
				{
					// if not enough people are available the result will be set and a NotEnoughStaffExption is thrown
					cresult.AddRange(location);
					// Checks if the generation should stop
					if((settings & GeneratorSettings.StopOnError) != 0)
					{
						result = cresult;
						throw new NotEnoughStaffException();
					}
					continue;
				}

				// check if we have the perfect amount of people
				if (func.MaxEmployees == location.Count && ((settings & GeneratorSettings.ForceSkills) == 0))
				{
					cresult.AddRange(location);
					continue;
				}

				//Iedereen die kan werken, juiste functie en locatie heeft en skill.
				List<EmployeeModel> skill = (from employee in location
											 join es in _context.EmployeeSkills on employee.ID equals es.EmployeeModelID
											 join sk in _context.Skill on es.SkillID equals sk.SkillID
											 join ss in _context.ShiftSkills on sk.SkillID equals ss.SkillID
											 where shift.ShiftSkills.Contains(ss)
											 select employee).ToList();

				// Checking if we have enough people
				// and if not trying to fill up the empty spots
				while (skill.Count < func.MaxEmployees)
				{
					// If not enough people have the required skill set
					// we go with the most experienced person available
					EmployeeModel HighestXPWorker = new EmployeeModel();
					foreach (EmployeeModel emp in location)
						if (!skill.Contains(emp) && HighestXPWorker.Experience < emp.Experience)
							HighestXPWorker = emp;
					skill.Add(HighestXPWorker);
				}

				// check if we have the required amount of people
				if(skill.Count == func.MaxEmployees)
				{
					cresult.AddRange(skill);
					continue;
				}

				// sorts skill with least experience first
				skill.Sort(new ExperienceComparer());
				
				// check if should take the most experienced no matter what
				if((settings & GeneratorSettings.PreferHigerExp) != 0)
				{
					// takes the most experienced people and saves them
					cresult.AddRange(skill.TakeLast(func.MaxEmployees));
					continue;
				}

				// the required experience that the team should have
				float requiredExp = func.MaxEmployees * avgExp;

				if((settings & GeneratorSettings.PreferLowerExp) != 0)
				{
					List<EmployeeModel> current = skill.Take(func.MaxEmployees).ToList();
					List<EmployeeModel> closest = new List<EmployeeModel>();
					float closestExp = 0;
					do
					{
						// calculates the hp the current list of people has
						float currentExp = 0;
						foreach (EmployeeModel employee in current)
							currentExp += employee.Experience; 
					}
					// pls don't hate me for doing this
					while (true);
				}

				//TODO experience filter toevoegen.

			}
			result = cresult;
		}
	}

	[Flags]
	public enum GeneratorSettings: short
	{
		// Default values
		Default = 0,
		// Can a person be working at 2 different functions
		AllowDoubleBooking = 0x1,
		// Should People with lower experience be preferred (can not be used with PreferHigherExp)
		PreferLowerExp = 0x2,
		// Should People with Higher experience be preferred (can not be used with PreferLowerExp)
		PreferHigerExp = 0x4,
		// Should everybody have the skills specified
		ForceSkills = 0x8,
		// Should the Generator stop if an error occurred
		StopOnError = 0x10,

	}

	class ExperienceComparer : Comparer<EmployeeModel>
	{
		public override int Compare(EmployeeModel x, EmployeeModel y) => x.Experience.CompareTo(y.Experience);
	}
}
