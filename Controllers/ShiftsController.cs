using Gride.Data;
using Gride.Gen;
using Gride.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Gride.Controllers
{
	[Authorize]
	public class ShiftsController : Controller
	{
		private readonly ApplicationDbContext _context;
		public Schedule schedule = new Schedule();
		private readonly SignInManager<IdentityUser> signInManager;
		private readonly UserManager<IdentityUser> userManager;

		public ShiftsController(ApplicationDbContext context,
								SignInManager<IdentityUser> signInManager,
								UserManager<IdentityUser> userManager)
		{
			this.signInManager = signInManager;
			this.userManager = userManager;
			_context = context;
		}

		// GET: Shifts
		/// <summary>
		/// Returns a page with a schedule where the shifts in a certain week are shown. 
		/// </summary>
		/// <param name="id">Weeknumber</param>
		/// <returns>View</returns>
		public async Task<IActionResult> Index(int? id)
		{
			if (User.Identity.IsAuthenticated && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
			{
				EmployeeModel employee = _context.EmployeeModel
					 .Single(e => e.EMail == User.Identity.Name);

				List<Models.Shift> allShifts = _context.Shift.ToList();

				if (id == null)
				{
					id = schedule._weekNumber;
				}

				schedule.currentWeek = (int)id;
				schedule.setWeek((int)id);
				schedule.makeSchedule();
				schedule.setShifts(allShifts);

				return View(schedule);
			}
			else
			{
				return Forbid();
			}
		}

		// GET: Shifts/Details/5
		/// <summary>
		/// Returns a page with the details of a certain shift. The time, location, skills required, functions and employees on the shift are shown.
		/// </summary>
		/// <param name="id">ShiftID</param>
		/// <returns>View</returns>
		public async Task<IActionResult> Details(int? id)
		{
			if (User.Identity.IsAuthenticated && _context.EmployeeModel.ToList().Find(x => x.EMail == User.Identity.Name).Admin)
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

				ViewData["Shiftlength"] = (int)(shift.End - shift.Start).TotalHours;

				return View(shift);
			}
			else
			{
				return Forbid();
			}
		}

		// GET: Shifts/Create
		/// <summary>
		/// Returns page with a form to create a new shift.
		/// </summary>
		/// <returns>View</returns>
		public IActionResult Create()
		{
			if (User.Identity.IsAuthenticated && _context.EmployeeModel.ToList().Find(x => x.EMail == User.Identity.Name).Admin)
			{
				var shift = new Shift();
				shift.ShiftSkills = new List<ShiftSkills>();
				shift.ShiftFunctions = new List<ShiftFunction>();
				PopulateAssignedFunction(shift);
				PopulateAssignedSkills(shift);
				PopulateLocationsDropDownList();
				return View();
			}
			else
			{
				return Forbid();
			}
		}
		/// <summary>
		/// Populates the create page with options to check all skills.
		/// </summary>
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

		/// <summary>
		/// Populates the create page with options to check all functions.
		/// </summary>
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

		/// <summary>
		/// Populates the dropdownlist on the create page with all locations.
		/// </summary>
		/// <param name="selectedLocation"></param>
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
		/// <summary>
		/// Creates new shift.
		/// </summary>
		/// <param name="shift">shift we want to create</param>
		/// <param name="selectedSkills">skills of the shift</param>
		/// <param name="selectedFunctions">functions of the shift</param>
		/// <param name="selectedFunctionsMax">maximum of employees of every function.</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Start,End,Weekly,LocationID,Location")] Shift shift, string[] selectedSkills, string[] selectedFunctions, int[] selectedFunctionsMax)
		{
			if (User.Identity.IsAuthenticated && _context.EmployeeModel.ToList().Find(x => x.EMail == User.Identity.Name).Admin)
			{
				//Add skills to shift
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
					//Add functions to shift.
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
						//If the shift is weekly we want new shifts every week. This are the shiftchildren.
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
			else
			{
				return Forbid();
			}
		}

		/// <summary>
		/// Creates the children of a shift. Every child has the same start en end time, but on different days. 
		/// The children have all different employees, so these can't be passed down from the parentShift.
		/// Everything else is the same for the children.
		/// </summary>
		/// <param name="shift">Shift we want weekly children of.</param>
		/// <returns>List with 52 shifts. One for every week for a year.</returns>
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
		/// <summary>
		/// Returns page on which we can edit a shift.
		/// </summary>
		/// <param name="id">ShiftID of shift we want to edit.</param>
		/// <returns>View</returns>
		public async Task<IActionResult> Edit(int? id)
		{
			if (User.Identity.IsAuthenticated && _context.EmployeeModel.ToList().Find(x => x.EMail == User.Identity.Name).Admin)
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
			else
			{
				return Forbid();
			}
		}
		/// <summary>
		/// Populates the edit page with options to check or uncheck the employees.
		/// </summary>
		/// <param name="shift">shift we want to change</param>
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
				if (shiftEmployees.Contains(employee.ID))
					viewModel.Add(new WorkData
					{
						EmployeeID = employee.ID,
						Name = employee.Name + " " + employee.LastName,
						Assigned = true,
						Delay = delay,
						Overtime = overtime
					});
			}
			ViewData["Employees"] = viewModel;
		}
		/// <summary>
		/// Populates the edit page with options to check all functions.
		/// </summary>
		/// <param name="shift">Shift we want to change</param>
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
		/// <summary>
		/// Populates the create page with options to check all skills.
		/// </summary>
		/// <param name="shift">Shift we want to change</param>
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
		/// <summary>
		/// Edits a shift
		/// </summary>
		/// <param name="id">ShiftID of shift we want to change</param>
		/// <param name="selectedSkills">skills of the shift</param>
		/// <param name="selectedFunctions">functions of the shift</param>
		/// <param name="selectedFunctionsMax">Maximum of employees of the functions of the shift.</param>
		/// <param name="selectedEmployees">employees of the shift</param>
		/// <returns>View</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int? id, string[] selectedSkills, string[] selectedFunctions, int[] selectedFunctionsMax, string[] selectedEmployees)

		{
			if (User.Identity.IsAuthenticated && _context.EmployeeModel.ToList().Find(x => x.EMail == User.Identity.Name).Admin)
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

				if (await TryUpdateModelAsync<Models.Shift>(shiftToUpdate, "",
					s => s.Start, s => s.End, s => s.LocationID))
				{
					if (String.IsNullOrWhiteSpace(shiftToUpdate.Location.Name))
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
			else
			{
				return Forbid();
			}
		}

		/// <summary>
		/// Edits the employees of a shift
		/// </summary>
		/// <param name="selectedEmployees">Employees of the shift</param>
		/// <param name="shiftToUpdate">Shift we want to change</param>
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
		/// <summary>
		/// Edits the skills of a shift.
		/// </summary>
		/// <param name="selectedSkills">skills of the shift</param>
		/// <param name="shiftToUpdate">shift we want to change</param>
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
		/// <summary>
		/// Edits the functions of a shift
		/// </summary>
		/// <param name="selectedFunctions">functions of the shift</param>
		/// <param name="selectedFunctionsMax">maximum of employees of the functions of the shift.</param>
		/// <param name="shiftToUpdate">Shift we want to change</param>
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
		/// <summary>
		/// Returns page with a shift we want to delete
		/// </summary>
		/// <param name="id">ShiftID of shift we want to delete</param>
		/// <returns>View</returns>
		public async Task<IActionResult> Delete(int? id)
		{
			if (User.Identity.IsAuthenticated && _context.EmployeeModel.ToList().Find(x => x.EMail == User.Identity.Name).Admin)
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
			else
			{
				return Forbid();
			}
		}

		// POST: Shifts/Delete/5
		/// <summary>
		/// Deletes a shift
		/// </summary>
		/// <param name="id">ShiftID of deleted shift.</param>
		/// <returns></returns>
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (User.Identity.IsAuthenticated && _context.EmployeeModel.ToList().Find(x => x.EMail == User.Identity.Name).Admin)
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
			else
			{
				return Forbid();
			}
		}

		/// <summary>
		/// After deleting a shift we want the next child of that shift to become the parentshift.
		/// </summary>
		/// <param name="shift">shift we want to delete.</param>
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

		/// <summary>
		/// Check if shift exists
		/// </summary>
		/// <param name="id">ShiftID of shift we searched for.</param>
		/// <returns></returns>
		private bool ShiftExists(int id)
		{
			return _context.Shift.Any(e => e.ShiftID == id);
		}

		/// <summary>
		/// Sends the data to the Generate Page
		/// </summary>
		public async Task<IActionResult> Generate() => User.Identity.IsAuthenticated && _context.EmployeeModel.ToList().Find(x => x.EMail == User.Identity.Name).Admin ? (IActionResult)View(): (IActionResult)Forbid();

		/// <summary>
		/// Generates a roster for the time given between <paramref name="start"/> and <paramref name="end"/> with the settings specified in <paramref name="Settings"/>
		/// </summary>
		/// <param name="settings">a List of numbers</param>
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

			// fixes some C# floating point parsing problems
			float actAvgExp;
			if (!float.TryParse(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == "," ? avgExp?.Replace('.', ',') : avgExp?.Replace(',', '.'), out actAvgExp))
				actAvgExp = 3;

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

			// the ids of every shift, function, and employee
			Dictionary<int, Dictionary<int, List<int>>> IDres = new Dictionary<int, Dictionary<int, List<int>>>();
			Parallel.ForEach(results.Keys, new ParallelOptions { MaxDegreeOfParallelism = 10 }, sID =>
			{
				Dictionary<int, List<int>> ShiftList = new Dictionary<int, List<int>>();
				foreach (int fID in results[sID].Keys)
					ShiftList.Add(fID, (from emp in results[sID][fID]
										select emp.ID).ToList());
				lock (IDres)
					IDres.Add(sID, ShiftList);
			});
			// Saves the data in the session
			HttpContext.Session.Set("genRes", Encoding.Default.GetBytes(JsonConvert.SerializeObject(IDres)));
			return RedirectToAction(nameof(Generated));
		}

		/// <summary>
		/// Sends the data to the generated page
		/// </summary>
		public async Task<IActionResult> Generated()
		{
			if(!(User.Identity.IsAuthenticated && _context.EmployeeModel.ToList().Find(x => x.EMail == User.Identity.Name).Admin))
				return Forbid(); 
			// Gets the data from the session
			HttpContext.Session.TryGetValue("genRes", out byte[] bytes);
			// Empties the session
			HttpContext.Session.Remove("genRes");
			// Gets the data from the session as something we can use
			Dictionary<int, Dictionary<int, List<int>>> IDResult = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, List<int>>>>(Encoding.Default.GetString(bytes)) ?? new Dictionary<int, Dictionary<int, List<int>>>();
			// Changes the IDResult in actual data we can use
			Dictionary<int, Dictionary<int, List<EmployeeModel>>> actResult = new Dictionary<int, Dictionary<int, List<EmployeeModel>>>();
			List<EmployeeModel> employees = await _context.EmployeeModel.ToListAsync();
			Parallel.ForEach(IDResult.Keys, new ParallelOptions { MaxDegreeOfParallelism = 10 }, sID =>
			{
				Dictionary<int, List<EmployeeModel>> ShiftList = new Dictionary<int, List<EmployeeModel>>();
				foreach (int fID in IDResult[sID].Keys)
					ShiftList.Add(fID, (from emp in IDResult[sID][fID]
										select employees.First(x => x.ID == emp)).ToList());
				lock (actResult)
					actResult.Add(sID, ShiftList);
			});
			// Sends the data to the view
			return View(actResult);
		}

		/// <summary>
		/// Adds everyone from <paramref name="selected"/> in the database
		/// </summary>
		/// <param name="selected">The selected employees with shift and function</param>
		/// <remarks>the form of the <see cref="string"/> in <paramref name="selected"/> is ShiftID, FunctionID, EmployeeID</remarks>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Generated(string[] selected)
		{
			foreach (string s in selected)
			{
				// makes s in a integer data that can be used
				// the form of IDs is [ShiftID, FunctionID, EmployeeID]
				int[] IDs = (from id in s.Split(',')
							 select int.Parse(id.Trim())).ToArray();
				await _context.Works.AddAsync(new Work
				{
					Shift = _context.Shift.Single(x => x.ShiftID == IDs[0]),
					ShiftID = IDs[0],
					Function = _context.Function.Single(x => x.FunctionID == IDs[1]),
					FunctionID = IDs[1],
					Employee = _context.EmployeeModel.Single(x => x.ID == IDs[2]),
					EmployeeID = IDs[2],
				});
			}
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		/// <summary>
		/// Sends the data to the page
		/// </summary>
		public async Task<IActionResult> AssignEmployee(int? id)
		{
			if (!User.Identity.IsAuthenticated || !_context.EmployeeModel.ToList().Find(x => x.EMail == User.Identity.Name).Admin)
				return Forbid();
			if (!id.HasValue)
				return NotFound();
			return View(id.Value);
		}

		/// <summary>
		/// Adds the employee to the shift and function specified
		/// </summary>
		/// <param name="id">The shift id</param>
		/// <param name="EmployeeID">The id of the employees</param>
		/// <param name="FunctionID">The id of the function</param>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AssignEmployee(int? id, int? EmployeeID, int? FunctionID)
		{
			// Checks if the data is correct
			if (!id.HasValue)
				return NotFound();
			if (!EmployeeID.HasValue)
				return NotFound(EmployeeID);
			if (!FunctionID.HasValue)
				return NotFound(FunctionID);
			if (_context.Works.ToList().Exists(x => x.FunctionID == FunctionID && x.EmployeeID == EmployeeID))
				return BadRequest("User is already assigned to this function with this shift");

			// Added the new employee to the works
			_context.Works.Add(new Work
			{
				Employee = await _context.EmployeeModel.FirstAsync(x => x.ID == EmployeeID),
				EmployeeID = EmployeeID.Value,
				Shift = await _context.Shift.FirstAsync(x => x.ShiftID == id),
				ShiftID = id.Value,
				Function = await _context.Function.FirstAsync(x => x.FunctionID == FunctionID),
				FunctionID = FunctionID.Value,
			});
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Details), new { id });
		}


		/// <summary>
		/// Deletes all the following shifts of a weekly shift. In other words, it deletes all the following children of a shift.
		/// </summary>
		/// <param name="id">ShiftID of the first shift we want to delete.</param>
		/// <returns>View</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteAllFollowing(int? id)

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
						}
						else
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