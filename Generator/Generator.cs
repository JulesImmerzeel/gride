using Gride.Data;
using Gride.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using static System.MathF;

namespace Gride.Gen
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
		/// <exception cref="ArgumentNullException">Thrown when either <paramref name="_context"/> or <paramref name="shift"/> is <see cref="null"/></exception>
		// TODO: Make it so you can see who is assigned to what function
		// TODO: Make it so you can get every exception that would have been thrown
#if DEBUG 
		public static void Generate(Shift shift, ApplicationDbContext _context, out Dictionary<int, List<EmployeeModel>> result, float avgExp = 2, GeneratorSettings settings = GeneratorSettings.StopOnError | GeneratorSettings.PreferTrios)
#else
		public static void Generate(Shift shift, ApplicationDbContext _context, out List<EmployeeModel> result, float avgExp = 2, GeneratorSettings settings = GeneratorSettings.Default | GeneratorSettings.PreferTrios)
#endif
		{
			// Checks if settings is viable
			if ((settings & (GeneratorSettings.PreferLowerExp | GeneratorSettings.PreferHigerExp)) == (GeneratorSettings.PreferLowerExp | GeneratorSettings.PreferHigerExp))
				throw new ArgumentException("can't have both PreferLowerEp and PreferHigherExp selected please chose one or none", "settings");
			if (_context == null)
				throw new ArgumentNullException("_context");
			if (shift == null)
				throw new ArgumentNullException("shift");

			// A temporary list for operations
			List<EmployeeModel> cresult = new List<EmployeeModel>();
			result = new Dictionary<int, List<EmployeeModel>>();

			foreach (ShiftFunction func in _context.ShiftFunctions.ToList().FindAll(x => x.ShiftID == shift.ShiftID))
			{

				//Iedereen die kan werken.
				List<EmployeeModel> available = (from row in _context.Availabilities
												 join ea in _context.EmployeeAvailabilities on row.AvailabilityID equals ea.AvailabilityID
												 join employee in _context.EmployeeModel on ea.EmployeeID equals employee.ID
												 where row.Weekly ? 
												 // if availability is weekly
												 row.Start.Day.Equals(shift.Start.Day) && row.End.Day.Equals(shift.End.Day) && 
												 (shift.Start.TimeOfDay >= row.Start.TimeOfDay && shift.End.TimeOfDay <= row.End.TimeOfDay) : 
												 // if not
												 (shift.Start >= row.Start && shift.End <= row.End)
												 select employee).ToList();


				// Removes everyone that is selected for other functions
				if ((settings & GeneratorSettings.AllowDoubleBooking) == 0)
					available = (from possible in RemoveUnavailable(available, shift, _context)
								 where !cresult.Contains(possible)
								 select possible).ToList();

				available = available.Distinct(new EmployeeComparer()).ToList();
				List<EmployeeModel> function = (from employee in available
											   join ef in _context.EmployeeFunctions on employee.ID equals ef.EmployeeID
											   where ef.FunctionID == func.FunctionID
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
					result.Add(func.FunctionID, location);
					// Checks if the generation should stop
					if((settings & GeneratorSettings.StopOnError) != 0)
						throw new NotEnoughStaffException();
					continue;
				}

				// check if we have the perfect amount of people
				if (func.MaxEmployees == location.Count && ((settings & GeneratorSettings.ForceSkills) == 0))
				{
					cresult.AddRange(location);
					result.Add(func.FunctionID, location);
					continue;
				}

				//Iedereen die kan werken, juiste functie en locatie heeft en skill.
				List<EmployeeModel> skill = (from employee in location
											 join es in _context.EmployeeSkills on employee.ID equals es.EmployeeModelID
											 join sk in _context.Skill on es.SkillID equals sk.SkillID
											 join ss in _context.ShiftSkills on sk.SkillID equals ss.SkillID
											 where shift.ShiftSkills.Contains(ss)
											 select employee).ToList();

				if(skill.Count < func.MaxEmployees && (settings & GeneratorSettings.ForceSkills) != 0)
				{
					// if not enough people are available the result will be set and a NotEnoughStaffExption is thrown (if possible)
					cresult.AddRange(skill);
					result.Add(func.FunctionID, skill);
					// Checks if the generation should stop
					if ((settings & GeneratorSettings.StopOnError) != 0)
						throw new NotEnoughStaffException();
					continue;
				}

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
					result.Add(func.FunctionID, skill);
					continue;
				}

				// sorts skill with least experience first
				skill.Sort(new ExperienceComparer());
				
				// check if should take the most experienced no matter what
				if((settings & GeneratorSettings.PreferHigerExp) != 0)
				{
					// takes the most experienced people and saves them
					skill = skill.TakeLast(func.MaxEmployees).ToList();
					cresult.AddRange(skill);
					result.Add(func.FunctionID, skill);
					continue;
				}

				// the required experience that the team should have
				float requiredExp = func.MaxEmployees * avgExp;

				if((settings & GeneratorSettings.PreferLowerExp) != 0)
				{
					List<EmployeeModel> current = skill.Take(func.MaxEmployees).ToList();
					List<EmployeeModel> closest = new List<EmployeeModel>();
					float closestExp = 0;
					bool done = false;
					int mostxpI = skill.Count - 1;
					int ToChange = current.Count - 1;
					// all hail brute force
					// probably needs to be multi threaded
					// TODO: not this
					do
					{
						// calculates the hp the current list of people has
						float currentExp = 0;
						foreach (EmployeeModel employee in current)
							currentExp += employee.Experience; 
						if(currentExp == requiredExp)
						{
							cresult.AddRange(current);
							result.Add(func.FunctionID, current);
							break;
						}

						if (Abs(closestExp - requiredExp) > Abs(currentExp - requiredExp))
						{
							// copying the data from current so that when we change current it doesn't affect closest
							closest = new List<EmployeeModel>(current);
							closestExp = currentExp;
						}

						current[ToChange] = skill[mostxpI];

						if (ToChange == 0 && mostxpI == current.Count)
							done = true;
						else if (mostxpI == current.Count)
						{
							mostxpI = skill.Count - 1;
							ToChange--;
						}
						else
							mostxpI--;
					}
					while (!done);

					if(done)
					{
						cresult.AddRange(closest);
						result.Add(func.FunctionID, closest);
					}
					continue;
				}
			}
		}

		/// <summary>
		/// Removes the unavailable employees in <paramref name="employees"/>.
		/// </summary>
		/// <param name="employees">The employees to filter.</param>
		/// <param name="shift">The shift to check who is available.</param>
		/// <param name="_context">The context to get the data from.</param>
		/// <returns>A filtered list</returns>
		static List<EmployeeModel> RemoveUnavailable(List<EmployeeModel> employees, Shift shift, ApplicationDbContext _context) =>
			(from employee in employees
			 where !(from uEmployee in employees
					 join w in _context.Works on uEmployee.ID equals w.EmployeeID
					 join s in _context.Shift on w.ShiftID equals s.ShiftID
					 where s.End > shift.Start || s.Start < shift.End
					 select uEmployee).ToList().Exists(x => x.ID == employee.ID)
			 select employee).ToList();
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
		// Should pairs of avg exp be made
		PreferPairs = 0x20,
		// Should trios of avg exp be made
		PreferTrios = 0x40,
	}

	class ExperienceComparer : Comparer<EmployeeModel>
	{
		public override int Compare(EmployeeModel x, EmployeeModel y) => x.Experience.CompareTo(y.Experience);
	}

	class EmployeeComparer : IEqualityComparer<EmployeeModel>
	{
		public bool Equals(EmployeeModel x, EmployeeModel y) => x.ID == y.ID;
		public int GetHashCode(EmployeeModel obj) => obj.GetHashCode();
	}
}
