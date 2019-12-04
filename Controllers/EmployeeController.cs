using Gride.Data;
using Gride.Models;
using Gride.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;

        public EmployeeController(ApplicationDbContext context, IHostingEnvironment env,
                                  SignInManager<IdentityUser> signInManager,
                                  UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
            this._context = context;
            this._env = env;
            this.signInManager = signInManager;
        }

        // GET: EmployeeModels
        public async Task<IActionResult> Index()
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                return View(await _context.EmployeeModel.ToListAsync());
            }

            else
            {
                return Forbid();
            }
        }


        // GET: EmployeeModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
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

                ViewBag.Supervisor = _context.EmployeeModel.FirstOrDefault(s => s.ID == employeeModel.SupervisorID);
                if (employeeModel == null)
                {
                    return NotFound();
                }

                return View(employeeModel);
            }
            else
            {
                return Forbid();
            }

            List<Work> works = _context.Works.Where(e => e.Employee == employeeModel).Include(m => m.Employee).Include(s => s.Shift).ToList();
            var workOverviewlist = new List<WorkOverview>();

            for (int i = 1; i <= 12; i++)
            {
                var workOverview = new WorkOverview
                {
                    Month = i
                };


                foreach (Work w in works)
                {
                    if (w.Shift.Start.Year == 2019 && w.Shift.Start.Month == i)
                    {
                        workOverview.AddHours((int)(w.Shift.End - w.Shift.Start).TotalHours);
                        workOverview.SubtractHours(w.Delay);
                        workOverview.AddHours(w.Overtime);
                    }
                }

                workOverviewlist.Add(workOverview);
            }

            ViewData["workOverview"] = workOverviewlist;

            return View(employeeModel);
        }

        // GET: EmployeeModels/Create
        public IActionResult Create()
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                var employee = new EmployeeModel();
                employee.EmployeeSkills = new List<EmployeeSkill>();
                employee.EmployeeFunctions = new List<EmployeeFunction>();
                employee.EmployeeLocations = new List<EmployeeLocations>();
                PopulateAssignedFunctions(employee);
                PopulateAssignedSkills(employee);
                PopulateAssignedLocations(employee);
                PopulateSupervisorsDropDownList();
                return View();
            }
            else
            {
                return Forbid();
            }
        }

        private void PopulateSupervisorsDropDownList(object selectedSupervisor = null)
        {

            var supervisors = _context.EmployeeModel
                .Where(e => e.Admin == true)
                .AsNoTracking()
                .ToList();

            ViewBag.SupervisorID = new SelectList(supervisors, "ID", "Name", selectedSupervisor);
        }

        // POST: EmployeeModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,LastName,DoB,Gender,EMail,PhoneNumber,Admin,LoginID,Experience,ProfileImage,SupervisorID")] EmployeeModel employeeModel, string[] selectedSkills, string[] selectedFunctions, string[] selectedLocations, EmployeeViewModel model)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                //employeeModel = setSupervisor(employeeModel);
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

            string uniqueFileName = null;

            // If the Photo property on the incoming model object is not null, then the user
            // has selected an image to upload.
            if (model.ProfileImage != null)
            {
                // The image must be uploaded to the images folder in wwwroot
                // To get the path of the wwwroot folder we are using the inject
                // HostingEnvironment service provided by ASP.NET Core
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                // To make sure the file name is unique we are appending a new
                // GUID value and and an underscore to the file name
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                // Use CopyTo() method provided by IFormFile interface to
                // copy the file to wwwroot/images folder
                model.ProfileImage.CopyTo(new FileStream(filePath, FileMode.Create));
            }

            employeeModel.ProfileImage = uniqueFileName;

            if (ModelState.IsValid)
            {
                _context.Add(employeeModel);
                await _context.SaveChangesAsync();

                //redirect admin to register page
                return Redirect("/Identity/Account/Register?email=" + employeeModel.EMail);

                //return RedirectToAction(nameof(Index));
            }

            PopulateSupervisorsDropDownList(employeeModel.SupervisorID);
            PopulateAssignedFunctions(employeeModel);
            PopulateAssignedSkills(employeeModel);
            PopulateAssignedLocations(employeeModel);
            return View(employeeModel);
            }
            else
            {
                return Forbid();
            }
        }

        // GET: EmployeeModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                if (id == null)
            {
                return NotFound();
            }

            var employeeModel = await _context.EmployeeModel
                .Include(s => s.EmployeeSkills)
                    .ThenInclude(s => s.Skill)
                .Include(s => s.EmployeeFunctions)
                    .ThenInclude(s => s.Function)
                .Include(s => s.EmployeeLocations)
                    .ThenInclude(s => s.Location)
                .FirstOrDefaultAsync(s => s.ID == id);
            if (employeeModel == null)
            {
                return NotFound();
            }
            PopulateAssignedFunctions(employeeModel);
            PopulateAssignedSkills(employeeModel);
            PopulateAssignedLocations(employeeModel);
            PopulateSupervisorsDropDownList(employeeModel.SupervisorID);
            return View(employeeModel);
            }
            else
            {
                return Forbid();
            }
        }

        // POST: EmployeeModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedSkills, string[] selectedFunctions, string[] selectedLocations, EmployeeViewModel model)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
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
                .FirstOrDefaultAsync(s => s.ID == (int)id);

            string uniqueFileName = employeeToUpdate.ProfileImage;

            // If the Photo property on the incoming model object is not null, then the user
            // has selected an image to upload.
            if (model.ProfileImage != null)
            {
                // The image must be uploaded to the images folder in wwwroot
                // To get the path of the wwwroot folder we are using the inject
                // HostingEnvironment service provided by ASP.NET Core
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                // To make sure the file name is unique we are appending a new
                // GUID value and and an underscore to the file name
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                // Use CopyTo() method provided by IFormFile interface to
                // copy the file to wwwroot/images folder
                model.ProfileImage.CopyTo(new FileStream(filePath, FileMode.Create));
            }

            if (await TryUpdateModelAsync<EmployeeModel>(employeeToUpdate, "",
                e => e.Name, e => e.LastName, e => e.DoB, e => e.Gender, e => e.EMail, e => e.PhoneNumber, e => e.Admin, e => e.Experience, e => e.ProfileImage, e => e.SupervisorID))
            {
                UpdateEmployeeLocations(selectedLocations, employeeToUpdate);
                UpdateEmployeeFunctions(selectedFunctions, employeeToUpdate);
                UpdateEmployeeSkills(selectedSkills, employeeToUpdate);


                employeeToUpdate.ProfileImage = uniqueFileName;


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
            return View(employeeToUpdate);
            }
            else
            {
                return Forbid();
            }
        }

        // GET: EmployeeModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
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
            else
            {
                return Forbid();
            }
        }

        // POST: EmployeeModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                var employeeModel = await _context.EmployeeModel.FindAsync(id);
            _context.EmployeeModel.Remove(employeeModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            }
            else
            {
                return Forbid();
            }
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
    }
}
