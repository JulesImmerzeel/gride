using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gride.Data;
using Gride.Models;

namespace Gride.Views.Shift
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
            var shift = new Models.Shift();
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

        // POST: Shifts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Start,End,LocationID,Location, Weekly")] Models.Shift shift, string[] selectedSkills, string[] selectedFunctions, int[] selectedFunctionsMax)
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
                    ICollection<Models.Shift> children = CreateChildren(shift);
                    foreach (Models.Shift child in children)
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

        private ICollection<Models.Shift> CreateChildren(Models.Shift shift)
        {
            Models.Shift tmpShift = new Models.Shift();
            tmpShift.Start = shift.Start;
            tmpShift.End = shift.End;
            tmpShift.Weekly = true;
            tmpShift.Location = shift.Location;
            tmpShift.LocationID = shift.LocationID;
            tmpShift.ParentShiftID = shift.ShiftID;
            ICollection<Models.Shift> children = new List<Models.Shift>();
            for (int i = 1; i < 52; i++)
            {
                Models.Shift child = new Models.Shift();
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

        private void PopulateAssignedEmployees(Models.Shift shift)
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

        private void PopulateAssignedFunction(Models.Shift shift)
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

        private void PopulateAssignedSkills(Models.Shift shift)
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

        private void UpdateShiftEmployees(string[] selectedEmployees, Models.Shift shiftToUpdate)
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

        private void UpdateShiftSkills(string[] selectedSkills, Models.Shift shiftToUpdate)
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

        private void UpdateShiftFunctions(string[] selectedFunctions, int[] selectedFunctionsMax, Models.Shift shiftToUpdate)
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

        private void TransferShiftChildren(Models.Shift shift)
        {
            ICollection<Models.Shift> children = shift.ShiftChildren;
            Models.Shift newParent = children.First();
            children.Remove(newParent);
            foreach (Models.Shift c in children)
            {
                c.ParentShiftID = newParent.ShiftID;
            }
            newParent.ShiftChildren = children;
            newParent.ParentShiftID = null;
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
                    shift.ShiftChildren = new List<Models.Shift>();
                    _context.Shift.Remove(shift);
                }
                else
                {
                    Models.Shift parentShift = _context.Shift
                        .Include(s => s.ShiftChildren)
                        .Single(s => s.ShiftID == shift.ParentShiftID);
                    List<Models.Shift> tmpChildren = new List<Models.Shift>();
                    List<Models.Shift> children = parentShift.ShiftChildren.OrderBy(s => s.Start).ToList();
                    bool foundChild = false;
                    foreach (Models.Shift child in children)
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
