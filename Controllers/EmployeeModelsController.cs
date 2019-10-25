using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gride.Data;
using Gride.Models;

namespace Gride.Controllers
{
    public class EmployeeModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EmployeeModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.EmployeeModel.ToListAsync());
        }

        // GET: EmployeeModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeModel = await _context.EmployeeModel
                                        .Include(e => e.EmployeeSkills)
                                            .ThenInclude(e => e.Skill)
                                        .Include(f => f.EmployeeFunctions)
                                            .ThenInclude(f => f.Function)
                                        .Include(l => l.EmployeeLocations)
                                            .ThenInclude(l => l.Location)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(m => m.ID == id);

            if (employeeModel == null)
            {
                return NotFound();
            }

            return View(employeeModel);
        }

        // GET: EmployeeModels/Create
        public IActionResult Create()
        {
            var employee = new EmployeeModel();
            employee.EmployeeSkills = new List<EmployeeSkill>();
            employee.EmployeeFunctions = new List<EmployeeFunction>();
            employee.EmployeeLocations = new List<EmployeeLocations>();
            PopulateAssignedFunctions(employee);
            PopulateAssignedSkills(employee);
            PopulateAssignedLocations(employee);
            PopulateGenderDropDownList();
            return View();
        }

        // POST: EmployeeModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,LastName,DoB,Gender,EMail,PhoneNumber,Admin,LoginID,Experience,ProfileImage")] EmployeeModel employeeModel , string[] selectedSkills, string[] selectedFunctions, string[] selectedLocations)
        {
            if (selectedSkills != null)
            {
                employeeModel.EmployeeSkills = new List<EmployeeSkill>();
                foreach (var skill in selectedSkills)
                {
                    var skillToAdd = new EmployeeSkill 
                    { 
                        EmployeeModelID = employeeModel.ID, 
                        Employee = employeeModel, 
                        SkillID = int.Parse(skill), 
                        Skill = _context.Skill.Single(s => s.SkillID == int.Parse(skill)) 
                    };
                    employeeModel.EmployeeSkills.Add(skillToAdd);
                }
            }
            if (selectedFunctions != null)
            {
                employeeModel.EmployeeFunctions = new List<EmployeeFunction>();
                foreach (var function in selectedFunctions)
                {
                    var functionToAdd = new EmployeeFunction
                    {
                        EmployeeID = employeeModel.ID,
                        Employee = employeeModel,
                        FunctionID = int.Parse(function),
                        Function = _context.Function.Single(s => s.FunctionID == int.Parse(function))
                    };
                    employeeModel.EmployeeFunctions.Add(functionToAdd);
                }
            }
            if (selectedLocations != null)
            {
                employeeModel.EmployeeLocations = new List<EmployeeLocations>();
                foreach (var location in selectedLocations)
                {
                    var locationToAdd = new EmployeeLocations
                    {
                        EmployeeModelID = employeeModel.ID,
                        Employee = employeeModel,
                        LocationID = int.Parse(location),
                        Location = _context.Locations.Single(s => s.LocationID == int.Parse(location))
                    };
                    employeeModel.EmployeeLocations.Add(locationToAdd);
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(employeeModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateGenderDropDownList();
            return View(employeeModel);
        }

        // GET: EmployeeModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeModel = await _context.EmployeeModel.FindAsync(id);
            if (employeeModel == null)
            {
                return NotFound();
            }
            PopulateAssignedFunctions(employeeModel);
            PopulateAssignedSkills(employeeModel);
            PopulateAssignedLocations(employeeModel);
            PopulateGenderDropDownList();
            return View(employeeModel);
        }

        // POST: EmployeeModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedSkills, string[] selectedFunctions, string[] selectedLocations)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeToUpdate = await _context.EmployeeModel
                .Include(s => s.EmployeeSkills)
                    .ThenInclude(s => s.Skill)
                .Include(s => s.EmployeeFunctions)
                    .ThenInclude(s => s.Function)
                .Include(s => s.EmployeeLocations)
                    .ThenInclude(s => s.Location)
                .FirstOrDefaultAsync(s => s.ID == _context.EmployeeModel.Single(e => e.EMail == User.Identity.Name).ID);

            if (await TryUpdateModelAsync<EmployeeModel>(employeeToUpdate, "",
                e => e.Name, e => e.LastName, e => e.DoB, e => e.Gender, e => e.EMail, e => e.PhoneNumber, e => e.Admin, e => e.Experience, e => e.ProfileImage))
            {
                UpdateEmployeeLocations(selectedLocations, employeeToUpdate);
                UpdateEmployeeFunctions(selectedFunctions, employeeToUpdate);
                UpdateEmployeeSkills(selectedSkills, employeeToUpdate);
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
            PopulateGenderDropDownList();
            return View(employeeToUpdate);
        }

        // GET: EmployeeModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeModel = await _context.EmployeeModel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (employeeModel == null)
            {
                return NotFound();
            }

            return View(employeeModel);
        }

        // POST: EmployeeModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeModel = await _context.EmployeeModel.FindAsync(id);
            _context.EmployeeModel.Remove(employeeModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeModelExists(int id)
        {
            return _context.EmployeeModel.Any(e => e.ID == id);
        }


        private void PopulateAssignedSkills(EmployeeModel employee)
        {
            if (employee.EmployeeSkills == null)
            {
                employee.EmployeeSkills = new List<EmployeeSkill>();
            }
            var allSkills = _context.Skill;
            var employeeSkills = new HashSet<int>(employee.EmployeeSkills.Select(c => c.SkillID));
            var viewModel = new List<ShiftSkillsData>();
            foreach (var skill in allSkills)
            {
                viewModel.Add(new ShiftSkillsData
                {
                    SkillID = skill.SkillID,
                    Name = skill.Name,
                    Assigned = employeeSkills.Contains(skill.SkillID)
                });
            }
            ViewData["Skills"] = viewModel;
        }

        private void PopulateAssignedFunctions(EmployeeModel employee)
        {
            if (employee.EmployeeFunctions == null)
            {
                employee.EmployeeFunctions = new List<EmployeeFunction>();
            }

            var allFunctions = _context.Function;
            var employeeFunctions = new HashSet<int>(employee.EmployeeFunctions.Select(c => c.FunctionID));
            var viewModel = new List<ShiftFunctionData>();
            foreach (var function in allFunctions)
            {
                viewModel.Add(new ShiftFunctionData
                {
                    FunctionID = function.FunctionID,
                    Name = function.Name,
                    Assigned = employeeFunctions.Contains(function.FunctionID)
                });
            }
            ViewData["Functions"] = viewModel;
        }

        private void PopulateAssignedLocations(EmployeeModel employee)
        {
            if (employee.EmployeeLocations == null)
            {
                employee.EmployeeLocations = new List<EmployeeLocations>();
            }
            var allLocations = _context.Locations;
            var employeeLocations = new HashSet<int>(employee.EmployeeLocations.Select(c => c.LocationID));
            var viewModel = new List<LocationData>();
            foreach (var location in allLocations)
            {
                viewModel.Add(new LocationData
                {
                    LocationID = location.LocationID,
                    Name = location.Name,
                    Assigned = employeeLocations.Contains(location.LocationID)
                });
            }
            ViewData["Locations"] = viewModel;
        }

        private void UpdateEmployeeLocations(string[] selectedLocations, EmployeeModel employeeToUpdate)
        {
            if (selectedLocations == null)
            {
                employeeToUpdate.EmployeeLocations = new List<EmployeeLocations>();
                return;
            }

            var selectedLocationsHS = new HashSet<string>(selectedLocations);
            if (employeeToUpdate.EmployeeLocations != null)
            {
                var employeeLocations = new HashSet<int>
                (employeeToUpdate.EmployeeLocations.Select(c => c.Location.LocationID));
                foreach (var location in _context.Locations)
                {
                    if (selectedLocationsHS.Contains(location.LocationID.ToString()))
                    {
                        if (!employeeLocations.Contains(location.LocationID))
                        {
                            employeeToUpdate.EmployeeLocations.Add(new EmployeeLocations
                            {
                                EmployeeModelID = employeeToUpdate.ID,
                                LocationID = location.LocationID,
                                Employee = employeeToUpdate,
                                Location = location
                            });
                        }
                    }
                    else
                    {

                        if (employeeLocations.Contains(location.LocationID))
                        {
                            EmployeeLocations locationToRemove = employeeToUpdate.EmployeeLocations.FirstOrDefault(i => i.LocationID == location.LocationID);
                            _context.Remove(locationToRemove);
                        }
                    }
                }
            }
            else
            {
                employeeToUpdate.EmployeeLocations = new List<EmployeeLocations>();
                foreach (var location in _context.Locations)
                {
                    if (selectedLocationsHS.Contains(location.LocationID.ToString()))
                    {
                        employeeToUpdate.EmployeeLocations.Add(new EmployeeLocations
                        {
                            EmployeeModelID = employeeToUpdate.ID,
                            Employee = employeeToUpdate,
                            LocationID = location.LocationID,
                            Location = location
                        });
                    }
                }
            }
        }

        private void UpdateEmployeeFunctions(string[] selectedFunctions, EmployeeModel employeeToUpdate)
        {
            if (selectedFunctions == null)
            {
                employeeToUpdate.EmployeeFunctions = new List<EmployeeFunction>();
                return;
            }

            var selectedFunctionsHS = new HashSet<string>(selectedFunctions);
            if (employeeToUpdate.EmployeeFunctions != null)
            {
                var employeeFunctions = new HashSet<int>
                (employeeToUpdate.EmployeeFunctions.Select(c => c.Function.FunctionID));
                foreach (var function in _context.Function)
                {
                    if (selectedFunctionsHS.Contains(function.FunctionID.ToString()))
                    {
                        if (!employeeFunctions.Contains(function.FunctionID))
                        {
                            employeeToUpdate.EmployeeFunctions.Add(new EmployeeFunction
                            {
                                EmployeeID = employeeToUpdate.ID,
                                FunctionID = function.FunctionID,
                                Employee = employeeToUpdate,
                                Function = function
                            });
                        }
                    }
                    else
                    {

                        if (employeeFunctions.Contains(function.FunctionID))
                        {
                            EmployeeFunction functionToRemove = employeeToUpdate.EmployeeFunctions.FirstOrDefault(i => i.FunctionID == function.FunctionID);
                            _context.Remove(functionToRemove);
                        }
                    }
                }
            }
            else
            {
                employeeToUpdate.EmployeeFunctions = new List<EmployeeFunction>();
                foreach (var function in _context.Function)
                {
                    if (selectedFunctionsHS.Contains(function.FunctionID.ToString()))
                    {
                        employeeToUpdate.EmployeeFunctions.Add(new EmployeeFunction
                        {
                            EmployeeID = employeeToUpdate.ID,
                            Employee = employeeToUpdate,
                            FunctionID = function.FunctionID,
                            Function = function
                        });
                    }
                }
            }
        }

        private void UpdateEmployeeSkills(string[] selectedSkills, EmployeeModel employeeToUpdate)
        {
            if (selectedSkills == null)
            {
                employeeToUpdate.EmployeeSkills = new List<EmployeeSkill>();
                return;
            }

            var selectedSkillsHS = new HashSet<string>(selectedSkills);
            if (employeeToUpdate.EmployeeSkills != null)
            {
                var employeeSkills = new HashSet<int>
                (employeeToUpdate.EmployeeSkills.Select(c => c.Skill.SkillID));
                foreach (var skill in _context.Skill)
                {
                    if (selectedSkillsHS.Contains(skill.SkillID.ToString()))
                    {
                        if (!employeeSkills.Contains(skill.SkillID))
                        {
                            employeeToUpdate.EmployeeSkills.Add(new EmployeeSkill
                            {
                                EmployeeModelID = employeeToUpdate.ID,
                                SkillID = skill.SkillID,
                                Employee = employeeToUpdate,
                                Skill = skill
                            });
                        }
                    }
                    else
                    {

                        if (employeeSkills.Contains(skill.SkillID))
                        {
                            EmployeeSkill skillToRemove = employeeToUpdate.EmployeeSkills.FirstOrDefault(i => i.SkillID == skill.SkillID);
                            _context.Remove(skillToRemove);
                        }
                    }
                }
            }
            else
            {
                employeeToUpdate.EmployeeSkills = new List<EmployeeSkill>();
                foreach (var skill in _context.Skill)
                {
                    if (selectedSkillsHS.Contains(skill.SkillID.ToString()))
                    {
                        employeeToUpdate.EmployeeSkills.Add(new EmployeeSkill
                        {
                            EmployeeModelID = employeeToUpdate.ID,
                            Employee = employeeToUpdate,
                            SkillID = skill.SkillID,
                            Skill = skill
                        });
                    }
                }
            }
        }
        private void PopulateGenderDropDownList()
        {
            ViewBag.Gender = new SelectList(new List<Gender>{Gender.Male, Gender.Female, Gender.Not_Specified});
        }
    }
}
