using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gride.Data;
using Gride.Models;
using Gride.Gen;
using System.Globalization;
using Newtonsoft.Json;

namespace Gride.Controllers
{
	public class ShiftsController : Controller
	{
		private readonly ApplicationDbContext _context;

		public ShiftsController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: Shifts
		public async Task<IActionResult> Index()
		{
			return View(await _context.Shift.OrderBy(s => s.Start).ToListAsync());
		}

		// GET: Shifts/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var shift = await _context.Shift
                .Include(s => s.ShiftFunctions).ThenInclude(s => s.Function)
                .Include(s => s.ShiftSkills).ThenInclude(s => s.Skill)
                .Include(s => s.Works).ThenInclude(s => s.Employee)
                .Include(s => s.Location)
                .FirstOrDefaultAsync(m => m.ShiftID == id);
			if (shift == null)
			{
				return NotFound();
			}

			return View(shift);
		}

		// GET: Shifts/Create
		public IActionResult Create()
		{
			var shift = new Shift();
			shift.ShiftSkills = new List<ShiftSkills>();
			shift.ShiftFunctions = new List<ShiftFunction>();
			PopulateAssignedFunction(shift);
			PopulateAssignedSkills(shift);
			PopulateLocationsDropDownList();
			return View();
		}

		private void PopulateSkills()
		{
			var allSkills = _context.Skill;
			var viewModel = new List<ShiftSkillsData>();
			foreach (var skill in allSkills)
			{
				viewModel.Add(new ShiftSkillsData
				{
					SkillID = skill.SkillID,
					Name = skill.Name,
					Assigned = false
				});
			}
			ViewData["Skills"] = viewModel;
		}

		private void PopulateFunctions()
		{
			var allFunctions = _context.Function;
			var viewModel = new List<ShiftFunctionData>();
			foreach (var function in allFunctions)
			{
				viewModel.Add(new ShiftFunctionData
				{
					FunctionID = function.FunctionID,
					Name = function.Name,
					Assigned = false,
					MaxEmployees = 1
				});
			}
			ViewData["Functions"] = viewModel;
		}

		private void PopulateLocationsDropDownList(object selectedLocation = null)
		{
			var locationQuery = from l in _context.Locations
								orderby l.Name
								select l;
			ViewBag.LocationID = new SelectList(locationQuery.AsNoTracking(), "LocationID", "Name", selectedLocation);
		}

		private void AssignStaff(Shift shift)
		{
			Dictionary<int, List<EmployeeModel>> results = new Dictionary<int, List<EmployeeModel>>();
			// look for each function this shift has
			foreach (int func in from shiftF in _context.ShiftFunctions
								  where shiftF.ShiftID == shift.ShiftID
								  select shiftF.FunctionID)
			{
				// Adds already assigned people to the list
				results.Add(func, (from work in _context.Works
								   where work.FunctionID == func && work.ShiftID == shift.ShiftID
								   select work.Employee).ToList());
			}
			Generator.Generate(shift, _context, ref results, settings: GeneratorSettings.PreferHigerExp);
			foreach (int funcID in results.Keys)
			{
				foreach (EmployeeModel employee in results[funcID])
				{
					Work w = new Work
					{
						FunctionID = funcID,
						Function = _context.Function.ToList().Find(x => x.FunctionID == funcID),
						EmployeeID = employee.ID,
						Employee = _context.EmployeeModel.ToList().Find(x => x.ID == employee.ID),
						ShiftID = shift.ShiftID,
						Shift = shift,
					};
					_context.Add(w);
				}
			}
			_context.SaveChanges();
		}

		// POST: Shifts/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Start,End,LocationID,Location")] Shift shift, string[] selectedSkills, string[] selectedFunctions, int[] selectedFunctionsMax)
		{
			if (selectedSkills != null)
			{
				shift.ShiftSkills = new List<ShiftSkills>();
				foreach (var skill in selectedSkills)
				{
					var skillToAdd = new ShiftSkills
					{
						ShiftID = shift.ShiftID,
						Shift = shift,
						SkillID = int.Parse(skill),
						Skill = _context.Skill.Single(s => s.SkillID == int.Parse(skill))
					};
					shift.ShiftSkills.Add(skillToAdd);
				}
			}
			if (selectedFunctions != null)
			{
                shift.ShiftFunctions = new List<ShiftFunction>();
                foreach (var function in selectedFunctions)
                {
                    int functionID = int.Parse(function);
                    int maxCnt = functionID - 1;
                    var functionToAdd = new ShiftFunction
                    {
                        ShiftID = shift.ShiftID,
                        Shift = shift,
                        FunctionID = functionID,
                        Function = _context.Function.Single(f => f.FunctionID == int.Parse(function)),
                        MaxEmployees = selectedFunctionsMax[maxCnt]
                    };
                    shift.ShiftFunctions.Add(functionToAdd);
                }
			}
			if (ModelState.IsValid)
			{
                _context.Add(shift);
                await _context.SaveChangesAsync();
                if (shift.Weekly)
                {
                    ICollection<Shift> children = CreateChildren(shift);
                    foreach (Shift child in children)
                    {
                        _context.Add(child);
                        child.ShiftFunctions = new List<ShiftFunction>();
                        foreach (ShiftFunction sf in shift.ShiftFunctions)
                        {
                            ShiftFunction shiftFunction = new ShiftFunction
                            {
                                Function = sf.Function,
                                FunctionID = sf.FunctionID,
                                MaxEmployees = sf.MaxEmployees,
                                ShiftID = child.ShiftID
                            };
                            child.ShiftFunctions.Add(shiftFunction);
                        }
                        child.ShiftSkills = new List<ShiftSkills>();
                        foreach (ShiftSkills ss in shift.ShiftSkills)
                        {
                            ShiftSkills shiftSkills = new ShiftSkills
                            {
                                Skill = ss.Skill,
                                SkillID = ss.SkillID,
                                ShiftID = child.ShiftID
                            };
                            child.ShiftSkills.Add(shiftSkills);
                        }
                    }
                    await _context.SaveChangesAsync();
                    shift.ShiftChildren = children;
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
			}
			PopulateLocationsDropDownList(shift);
			return View(shift);
		}

        private ICollection<Shift> CreateChildren(Shift shift)
        {
            Shift tmpShift = new Shift();
            tmpShift.Start = shift.Start;
            tmpShift.End = shift.End;
            tmpShift.Weekly = true;
            tmpShift.Location = shift.Location;
            tmpShift.LocationID = shift.LocationID;
            tmpShift.ParentShiftID = shift.ShiftID;
            ICollection<Shift> children = new List<Shift>();
            for (int i = 1; i < 52; i++)
            {
                Shift child = new Shift();
                child.Weekly = true;
                child.Location = tmpShift.Location;
                child.LocationID = tmpShift.LocationID;
                child.ParentShiftID = tmpShift.ParentShiftID;
                child.Start = shift.Start.AddDays(7 * i);
                child.End = shift.End.AddDays(7 * i);
                children.Add(child);
            }
            return children;
        }

		// GET: Shifts/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var shift = await _context.Shift
				.Include(s => s.ShiftFunctions).ThenInclude(s => s.Function)
				.Include(s => s.ShiftSkills).ThenInclude(s => s.Skill)
				.Include(s => s.Works).ThenInclude(s => s.Employee)
				.Include(s => s.Location)
				.AsNoTracking()
				.FirstOrDefaultAsync(m => m.ShiftID == id);
			if (shift == null)
			{
				return NotFound();
			}
			PopulateLocationsDropDownList(shift.LocationID);
			PopulateAssignedSkills(shift);
			PopulateAssignedFunction(shift);
			PopulateAssignedEmployees(shift);
			return View(shift);
		}

		private void PopulateAssignedEmployees(Shift shift)
		{
			var allEmployees = _context.EmployeeModel;
			var shiftEmployees = new HashSet<int>(shift.Works.Select(c => c.EmployeeID));
			var viewModel = new List<WorkData>();
			foreach (var employee in allEmployees)
			{
				int delay = 0, overtime = 0;
				if (_context.Works.FirstOrDefault(f => (f.EmployeeID == employee.ID) && (f.ShiftID == shift.ShiftID)) != null)
				{
					delay = _context.Works.FirstOrDefault(f => (f.EmployeeID == employee.ID) && (f.ShiftID == shift.ShiftID)).Delay;
					overtime = _context.Works.FirstOrDefault(f => (f.EmployeeID == employee.ID) && (f.ShiftID == shift.ShiftID)).Overtime;
				}
				viewModel.Add(new WorkData
				{
					EmployeeID = employee.ID,
					Name = employee.Name + " " + employee.LastName,
					Assigned = shiftEmployees.Contains(employee.ID),
					Delay = delay,
					Overtime = overtime
				});
			}
			ViewData["Employees"] = viewModel;
		}

		private void PopulateAssignedFunction(Shift shift)
		{
			var allFunctions = _context.Function;
			var shiftFunctions = new HashSet<int>(shift.ShiftFunctions.Select(c => c.FunctionID));
			var viewModel = new List<ShiftFunctionData>();
			foreach (var function in allFunctions)
			{
				int maxEmployees = 0;
				if (_context.ShiftFunctions.FirstOrDefault(f => (f.FunctionID == function.FunctionID) && (f.ShiftID == shift.ShiftID)) != null)
				{
					maxEmployees = _context.ShiftFunctions.FirstOrDefault(f => (f.FunctionID == function.FunctionID) && (f.ShiftID == shift.ShiftID)).MaxEmployees;
				}
				viewModel.Add(new ShiftFunctionData
				{
					FunctionID = function.FunctionID,
					Name = function.Name,
					Assigned = shiftFunctions.Contains(function.FunctionID),
					MaxEmployees = maxEmployees
				});
			}
			ViewData["Functions"] = viewModel;
		}

		private void PopulateAssignedSkills(Shift shift)
		{
			var allSkills = _context.Skill;
			var shiftSkills = new HashSet<int>(shift.ShiftSkills.Select(c => c.SkillID));
			var viewModel = new List<ShiftSkillsData>();
			foreach (var skill in allSkills)
			{
				viewModel.Add(new ShiftSkillsData
				{
					SkillID = skill.SkillID,
					Name = skill.Name,
					Assigned = shiftSkills.Contains(skill.SkillID)
				});
			}
			ViewData["Skills"] = viewModel;
		}

		// POST: Shifts/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int? id, string[] selectedSkills, string[] selectedFunctions, int[] selectedFunctionsMax, string[] selectedEmployees)

		{
			if (id == null)
			{
				return NotFound();
			}

			var shiftToUpdate = await _context.Shift
				.Include(s => s.ShiftSkills)
					.ThenInclude(s => s.Skill)
				.Include(s => s.ShiftFunctions)
					.ThenInclude(s => s.Function)
				.Include(s => s.Works)
					.ThenInclude(s => s.Employee)
				.Include(s => s.Location)
				.FirstOrDefaultAsync(s => s.ShiftID == id);

			if (await TryUpdateModelAsync(shiftToUpdate, "",
				s => s.Start, s => s.End, s => s.LocationID))
			{
				if (string.IsNullOrWhiteSpace(shiftToUpdate.Location.Name))
				{
					shiftToUpdate.Location = null;
				}
				UpdateShiftSkills(selectedSkills, shiftToUpdate);
				UpdateShiftFunctions(selectedFunctions, selectedFunctionsMax, shiftToUpdate);
				UpdateShiftEmployees(selectedEmployees, shiftToUpdate);
				try
				{
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateException /* ex */)
				{
					//Log the error (uncomment ex variable name and write a log.)
					ModelState.AddModelError("", "Unable to save changes. " +
						"Try again, and if the problem persists, " +
						"see your system administrator.");
				}
				return RedirectToAction(nameof(Index));
			}
			PopulateLocationsDropDownList(shiftToUpdate.LocationID);
			PopulateAssignedSkills(shiftToUpdate);
			PopulateAssignedFunction(shiftToUpdate);
			PopulateAssignedEmployees(shiftToUpdate);
			return View(shiftToUpdate);
		}

		private void UpdateShiftEmployees(string[] selectedEmployees, Shift shiftToUpdate)
		{
			if (selectedEmployees == null)
			{
				shiftToUpdate.Works = new List<Work>();
				return;
			}

			var selectedEmployeesHS = new HashSet<string>(selectedEmployees);
			if (shiftToUpdate.Works != null)
			{
				var shiftEmployees = new HashSet<int>
				(shiftToUpdate.Works.Select(c => c.Employee.ID));
				foreach (var employee in _context.EmployeeModel)
				{
					if (selectedEmployeesHS.Contains(employee.ID.ToString()))
					{
						if (!shiftEmployees.Contains(employee.ID))
						{
							shiftToUpdate.Works.Add(new Work
							{
								ShiftID = shiftToUpdate.ShiftID,
								EmployeeID = employee.ID,
								Employee = employee,
								Shift = shiftToUpdate,
								Overtime = 0,
								Delay = 0
							});
						}
					}
					else
					{

						if (shiftEmployees.Contains(employee.ID))
						{
							Work workToRemove = shiftToUpdate.Works.FirstOrDefault(i => i.EmployeeID == employee.ID);
							_context.Remove(workToRemove);
						}
					}
				}
			}
			else
			{
				shiftToUpdate.Works = new List<Work>();
				foreach (var employee in _context.EmployeeModel)
				{
					if (selectedEmployeesHS.Contains(employee.ID.ToString()))
					{
						shiftToUpdate.Works.Add(new Work
						{
							ShiftID = shiftToUpdate.ShiftID,
							Shift = shiftToUpdate,
							EmployeeID = employee.ID,
							Employee = employee,
							Delay = 0,
							Overtime = 0
						});
					}
				}
			}
		}

		private void UpdateShiftSkills(string[] selectedSkills, Shift shiftToUpdate)
		{
			if (selectedSkills == null)
			{
				shiftToUpdate.ShiftSkills = new List<ShiftSkills>();
				return;
			}

			var selectedSkillsHS = new HashSet<string>(selectedSkills);
			if (shiftToUpdate.ShiftSkills != null)
			{
				var shiftSkills = new HashSet<int>
				(shiftToUpdate.ShiftSkills.Select(c => c.Skill.SkillID));
				foreach (var skill in _context.Skill)
				{
					if (selectedSkillsHS.Contains(skill.SkillID.ToString()))
					{
						if (!shiftSkills.Contains(skill.SkillID))
						{
							shiftToUpdate.ShiftSkills.Add(new ShiftSkills { ShiftID = shiftToUpdate.ShiftID, SkillID = skill.SkillID });
						}
					}
					else
					{

						if (shiftSkills.Contains(skill.SkillID))
						{
							ShiftSkills skillToRemove = shiftToUpdate.ShiftSkills.FirstOrDefault(i => i.SkillID == skill.SkillID);
							_context.Remove(skillToRemove);
						}
					}
				}
			}
			else
			{
				shiftToUpdate.ShiftSkills = new List<ShiftSkills>();
				foreach (var skill in _context.Skill)
				{
					if (selectedSkillsHS.Contains(skill.SkillID.ToString()))
					{
						shiftToUpdate.ShiftSkills.Add(new ShiftSkills
						{
							ShiftID = shiftToUpdate.ShiftID,
							Shift = shiftToUpdate,
							SkillID = skill.SkillID,
							Skill = skill
						});
					}
				}
			}
		}

		private void UpdateShiftFunctions(string[] selectedFunctions, int[] selectedFunctionsMax, Shift shiftToUpdate)
		{
			if (selectedFunctions == null || selectedFunctionsMax == null)
			{
				shiftToUpdate.ShiftFunctions = new List<ShiftFunction>();
				return;
			}

			var selectedFunctionsHS = new HashSet<string>(selectedFunctions);
			if (shiftToUpdate.ShiftFunctions != null)
			{
				var shiftFunctions = new HashSet<int>
				(shiftToUpdate.ShiftFunctions.Select(c => c.Function.FunctionID));
				int cnt = 0;
				foreach (var function in _context.Function)
				{
					if (selectedFunctionsHS.Contains(function.FunctionID.ToString()))
					{
						if (!shiftFunctions.Contains(function.FunctionID))
						{
							shiftToUpdate.ShiftFunctions.Add(new ShiftFunction { ShiftID = shiftToUpdate.ShiftID, FunctionID = function.FunctionID, MaxEmployees = selectedFunctionsMax[cnt] });
						}
						else
						{
							shiftToUpdate.ShiftFunctions.Single(f => (f.FunctionID == function.FunctionID) && (f.ShiftID == shiftToUpdate.ShiftID)).MaxEmployees = selectedFunctionsMax[cnt];
						}
					}
					else
					{

						if (shiftFunctions.Contains(function.FunctionID))
						{
							ShiftFunction functionToRemove = shiftToUpdate.ShiftFunctions.FirstOrDefault(i => i.FunctionID == function.FunctionID);
							_context.Remove(functionToRemove);
						}
					}
					cnt++;
				}
			}
			else
			{
				shiftToUpdate.ShiftFunctions = new List<ShiftFunction>();
				int cnt = 0;
				foreach (var function in _context.Function)
				{
					if (selectedFunctionsHS.Contains(function.FunctionID.ToString()))
					{
						shiftToUpdate.ShiftFunctions.Add(new ShiftFunction
						{
							ShiftID = shiftToUpdate.ShiftID,
							Shift = shiftToUpdate,
							FunctionID = function.FunctionID,
							Function = function,
							MaxEmployees = selectedFunctionsMax[cnt]
						});
					}
					cnt++;
				}
			}

		}

		// GET: Shifts/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var shift = await _context.Shift
				.FirstOrDefaultAsync(m => m.ShiftID == id);
			if (shift == null)
			{
				return NotFound();
			}

			return View(shift);
		}

		// POST: Shifts/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{ 
            var shift = await _context.Shift
                .Include(s => s.ShiftChildren)
                .Include(s => s.ShiftFunctions).ThenInclude(f => f.Function)
                .Include(s => s.ShiftSkills).ThenInclude(s => s.Skill)
                .Include(s => s.Location)
                .FirstOrDefaultAsync(s => s.ShiftID == id);
            if (shift.Weekly)
            {
                if (shift.ShiftChildren.Count() > 0)
                {
                    TransferShiftChildren(shift);
                }
            }
            _context.Shift.Remove(shift);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
		}

        private void TransferShiftChildren(Shift shift)
        {
            ICollection<Shift> children = shift.ShiftChildren;
            Shift newParent = children.First();
            children.Remove(newParent);
            foreach (Shift c in children)
            {
                c.ParentShiftID = newParent.ShiftID;
            }
            newParent.ShiftChildren = children;
            newParent.ParentShiftID = null;
        }

		private bool ShiftExists(int id)
		{
			return _context.Shift.Any(e => e.ShiftID == id);
		}
	
		public async Task<IActionResult> Generate()
		{
			return View();
		}

		/// <summary>
		/// Generates a roster for the time given between <paramref name="start"/> and <paramref name="end"/> with the settings specified in <paramref name="Settings"/>
		/// </summary>
		/// <param name="Settings">a List of numbers</param>
		/// <param name="start">the start date of the generation</param>
		/// <param name="end">the end date of the generation</param>
		/// <remarks>If <paramref name="start"/> is <see cref="null"/> then it is set to now.
		/// If <paramref name="end"/> is <see cref="null"/> then it's set to 2 weeks from <paramref name="start"/></remarks>
		/// <exception cref="ArgumentException">thrown when one of the Settings is wrong</exception>
		/// <exception cref="ArgumentException">thrown when <paramref name="end"/> is earlier then <paramref name="start"/></exception>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Generate(int[] settings, int pairSettings, string avgExp, DateTime? start = null, DateTime? end = null)
		{
			// sets the value for start and end when they are null
			if (!start.HasValue)
				start = DateTime.Now;
			if (!end.HasValue)
				end = start.Value.AddDays(14);
			if (end < start)
				throw new ArgumentException();

			// fixes some C# floating point parsing bullshit
			float actAvgExp = float.Parse(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == "," ? avgExp.Replace('.', ','): avgExp.Replace(',','.'));
			
			// sets the settings ready for use
			GeneratorSettings genSettings = 0;
			try
			{
				foreach (GeneratorSettings i in (from i in settings select (GeneratorSettings)i))
					genSettings |= i;
				genSettings |= (GeneratorSettings)pairSettings;
			}
			catch
			{
				throw new ArgumentException("one of the values passed to Settings isn't viable", nameof(settings));
			}

			Dictionary<int, Dictionary<int, List<EmployeeModel>>> results = new Dictionary<int, Dictionary<int, List<EmployeeModel>>>();

			try
			{
				foreach (Shift shift in from shift in _context.Shift
										where shift.Start >= start && shift.End <= end
										select shift)
				{
					Dictionary<int, List<EmployeeModel>> thisShiftResults = new Dictionary<int, List<EmployeeModel>>();
					// look for each function this shift has
					foreach (int func in from shiftF in _context.ShiftFunctions
										 where shiftF.ShiftID == shift.ShiftID
										 select shiftF.FunctionID)
					{
						// Adds already assigned people to the list
						thisShiftResults.Add(func, (from work in _context.Works
													where work.FunctionID == func && work.ShiftID == shift.ShiftID
													select work.Employee).ToList());
					}
					Generator.Generate(shift, _context, ref thisShiftResults, actAvgExp, genSettings);
					results.Add(shift.ShiftID, thisShiftResults);
				}
			}
			catch
			{
				throw new NotImplementedException("generator error handling not implemented yet");
			}
			return RedirectToAction(nameof(Generated), "Shifts", JsonConvert.SerializeObject(results));
		}

		public async Task<IActionResult> Generated(string result = "")
		{
			// this is going to look so good in the url
			Dictionary<int, Dictionary<int, List<EmployeeModel>>> actResult = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, List<EmployeeModel>>>>(result) ?? new Dictionary<int, Dictionary<int, List<EmployeeModel>>>();
			return View(actResult);
		}

		[HttpPost, ActionName("Generated")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> GeneratedConfirmed(string[] selected)
		{
			Parallel.ForEach(selected, new ParallelOptions { MaxDegreeOfParallelism = 10}, async s =>
			{
				int[] IDs = (from id in s.Split(',')
							 select int.Parse(id.Trim())).ToArray();
				await _context.Works.AddAsync(new Work
				{
					Shift = _context.Shift.Single(x => x.ShiftID == IDs[0]),
					ShiftID = IDs[0],
					Function = _context.Function.Single(x => x.FunctionID == IDs[0]),
					FunctionID = IDs[1],
					Employee = _context.EmployeeModel.Single(x => x.ID == IDs[2]),
				});
			});
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllFollowing(int id)

        {
            var shift = await _context.Shift
                .Include(s => s.ShiftChildren)
                .Include(s => s.ShiftFunctions).ThenInclude(f => f.Function)
                .Include(s => s.ShiftSkills).ThenInclude(s => s.Skill)
                .Include(s => s.Location)
                .FirstOrDefaultAsync(s => s.ShiftID == id);
            if (shift.Weekly)
            {
                if (shift.ShiftChildren.Count() > 0)
                {
                    shift.ShiftChildren = new List<Shift>();
                    _context.Shift.Remove(shift);
                }
                else
                {
                    Shift parentShift = _context.Shift
                        .Include(s => s.ShiftChildren)
                        .Single(s => s.ShiftID == shift.ParentShiftID);
                    List<Shift> tmpChildren = new List<Shift>();
                    List<Shift> children = parentShift.ShiftChildren.OrderBy(s => s.Start).ToList();
                    bool foundChild = false;
                    foreach (Shift child in children)
                    {
                        if (child.ShiftID == shift.ShiftID)
                        {
                            foundChild = true;
                        }
                        if (foundChild)
                        {
                            _context.Remove(child);
                        } else
                        {
                            tmpChildren.Add(child);
                        }
                    }
                    parentShift.ShiftChildren = tmpChildren;
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
	}
}
