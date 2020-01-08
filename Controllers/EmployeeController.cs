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

        /// <summary>
        /// Set context, enviorment, user manager and sing in manager.
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="env">Hosting enviorment</param>
        /// <param name="signInManager">Sing in manager</param>
        /// <param name="userManager">User manager</param>
        public EmployeeController(ApplicationDbContext context, IHostingEnvironment env,
                                  SignInManager<IdentityUser> signInManager,
                                  UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
            this._context = context;
            this._env = env;
            this.signInManager = signInManager;
        }

        /// <summary>
        /// GET: EmployeeModels
        /// </summary>
        /// <returns>Employee View</returns>
        public async Task<IActionResult> Index()
        {
            // return not found if ID not given.
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                // create list of employees
                var employees = await _context.EmployeeModel.ToListAsync();

                // loop trough each employee and calculate worked hours
                foreach (EmployeeModel employee in employees)
                {
                    // get list of works associated with this employee
                    List<Work> works = _context.Works.Where(e => e.Employee == employee).Include(m => m.Employee).Include(s => s.Shift).ToList();

                    var worked = new WorkOverview(); // create work overview

                    // calculate worked hours bases on works hours
                    foreach (Work w in works)
                    {
                        //check if user is within this year
                        if (w.Shift.Start.Year == DateTime.Now.Year && w.Shift.Start.Month == DateTime.Now.Month)
                        {
                            worked.AddHours((int)(w.Shift.End - w.Shift.Start).TotalHours);
                            worked.SubtractHours(w.Delay);
                            worked.AddHours(w.Overtime);
                        }
                    }

                    // set WorkOverview to worked item
                    employee.Workoverview = worked;
                }

                return View(employees);
            }

            else
            {
                return Forbid(); // user is either not logged nor an admin, show forbidden page.
            }
        }

        /// <summary>
        /// GET: EmployeeModels/Details/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Employee View</returns>
        public async Task<IActionResult> Details(int? id)
        {
            // check if user is loggen in and user is an admin.
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                // return not found if ID not given.
                if (id == null)
                {
                    return NotFound();
                }

                // Get employee model and include skills, functions and locations
                var employeeModel = await _context.EmployeeModel
                                            .Include(e => e.EmployeeSkills)
                                                .ThenInclude(e => e.Skill)
                                            .Include(f => f.EmployeeFunctions)
                                                .ThenInclude(f => f.Function)
                                            .Include(l => l.EmployeeLocations)
                                                .ThenInclude(l => l.Location)
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(m => m.ID == id);

                //add supervisor to viewbag
                ViewBag.Supervisor = _context.EmployeeModel.FirstOrDefault(s => s.ID == employeeModel.SupervisorID);

                // if employee model is not found, return not found.
                if (employeeModel == null)
                {
                    return NotFound();
                }

                // get all works associated with this employee
                List<Work> works = _context.Works.Where(e => e.Employee == employeeModel).Include(m => m.Employee).Include(s => s.Shift).ToList();
                var workOverviewlist = new List<WorkOverview>(); // create a list containing work overview


                // fill the list with 12 items (every item represents a month of the year).
                // a WorkOverview is created for every month of the year,
                // it will add an item to the list and calculate the users worked hours for that given month.
                for (int i = 1; i <= 12; i++)
                {
                    // create new work overview and set month to current itterations number
                    var workOverview = new WorkOverview
                    {
                        Month = i //set month to i
                    };

                    // go trough each users work and add total worked hours
                    // the worked hours calculated by adding and subtracting the delay and overtime
                    // this way the actual worked hours are shown
                    foreach (Work w in works)
                    {
                        //check if work is in current year and month
                        if (w.Shift.Start.Year == DateTime.Now.Year && w.Shift.Start.Month == i)
                        {
                            workOverview.AddHours((int)(w.Shift.End - w.Shift.Start).TotalHours);
                            workOverview.SubtractHours(w.Delay);
                            workOverview.AddHours(w.Overtime);
                        }
                    }

                    //add item to workOverViewList
                    workOverviewlist.Add(workOverview);
                }

                //add workOverViewList to the viewdata
                ViewData["workOverview"] = workOverviewlist;

                return View(employeeModel);
            } else
            {
                return Forbid(); // user is either not logged nor an admin, show forbidden page.
            }
        }

        /// <summary>
        /// GET: EmployeeModels/Create
        /// </summary>
        /// <returns>Employee view</returns>
        public IActionResult Create()
        {
            // check if user is loggen in and user is an admin.
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                // get employee and set skills, functions and locations.
                var employee = new EmployeeModel
                {
                    EmployeeSkills = new List<EmployeeSkill>(),
                    EmployeeFunctions = new List<EmployeeFunction>(),
                    EmployeeLocations = new List<EmployeeLocations>()
                };

                // call populate functions
                PopulateAssignedFunctions(employee);
                PopulateAssignedSkills(employee);
                PopulateAssignedLocations(employee);
                PopulateSupervisorsDropDownList();

                return View();
            }
            else
            {
                return Forbid(); // user is either not logged nor an admin, show forbidden page.
            }
        }

        /// <summary>
        /// Create a list of supervisors.
        /// 
        /// A suprivisor is defined by an employee being an admin.
        /// </summary>
        /// <param name="selectedSupervisor">Employees current supervisor</param>
        private void PopulateSupervisorsDropDownList(object selectedSupervisor = null)
        {
            // get employees with admin true
            var supervisors = _context.EmployeeModel
                .Where(e => e.Admin == true)
                .AsNoTracking()
                .ToList();

            // add select list to viewbag with current supervisor selected.
            ViewBag.SupervisorID = new SelectList(supervisors, "ID", "Name", selectedSupervisor);
        }

        /// <summary>
        /// POST: EmployeeModels/Create
        /// </summary>
        /// <param name="employeeModel">Employee Model</param>
        /// <param name="selectedSkills">Selected Skills</param>
        /// <param name="selectedFunctions">Selected functions</param>
        /// <param name="selectedLocations">Selected locations</param>
        /// <param name="model">EmployeeViewModel used for image upload</param>
        /// <returns>Employee View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,LastName,DoB,Gender,EMail,PhoneNumber,Admin,LoginID,Experience,ProfileImage,SupervisorID")] EmployeeModel employeeModel, string[] selectedSkills, string[] selectedFunctions, string[] selectedLocations, EmployeeViewModel model)
        {
            // check if user is loggen in and user is an admin.
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                //employeeModel = setSupervisor(employeeModel);
                if (selectedSkills != null)
                {
                    // create list of skills
                    employeeModel.EmployeeSkills = new List<EmployeeSkill>();

                    // assign selected skills
                    foreach (var skill in selectedSkills)
                    {
                        // create employee skill and add to list
                        var skillToAdd = new EmployeeSkill
                        {
                            EmployeeModelID = employeeModel.ID,
                            Employee = employeeModel,
                            SkillID = int.Parse(skill),
                            Skill = _context.Skill.Single(s => s.SkillID == int.Parse(skill))
                        };

                        //add to list
                        employeeModel.EmployeeSkills.Add(skillToAdd);
                    }
                }

                // only if no list if selected functions is given
                if (selectedFunctions != null)
                {
                    // get list of functions 
                    employeeModel.EmployeeFunctions = new List<EmployeeFunction>();

                    // populate selected functions
                    foreach (var function in selectedFunctions)
                    {
                        var functionToAdd = new EmployeeFunction
                        {
                            EmployeeID = employeeModel.ID,
                            Employee = employeeModel,
                            FunctionID = int.Parse(function),
                            Function = _context.Function.Single(s => s.FunctionID == int.Parse(function))
                        };

                        employeeModel.EmployeeFunctions.Add(functionToAdd); // add to list
                    }
                }

                // only if no list if selected locations is given
                if (selectedLocations != null)
                {
                    // get locations of employee
                    employeeModel.EmployeeLocations = new List<EmployeeLocations>();

                    // populate selected locations
                    foreach (var location in selectedLocations)
                    {
                        var locationToAdd = new EmployeeLocations
                        {
                            EmployeeModelID = employeeModel.ID,
                            Employee = employeeModel,
                            LocationID = int.Parse(location),
                            Location = _context.Locations.Single(s => s.LocationID == int.Parse(location))
                        };
                        
                        employeeModel.EmployeeLocations.Add(locationToAdd); // add to list
                    }
                }

                string uniqueFileName = null; // will store generated filename

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

                employeeModel.ProfileImage = uniqueFileName; // set profile image to uploaded filename

                if (ModelState.IsValid)
                {
                    _context.Add(employeeModel);
                    await _context.SaveChangesAsync();

                    //redirect admin to register page
                    return Redirect("/Identity/Account/Register?email=" + employeeModel.EMail);

                    //return RedirectToAction(nameof(Index));
                }

                // call populate functions
                PopulateSupervisorsDropDownList(employeeModel.SupervisorID);
                PopulateAssignedFunctions(employeeModel);
                PopulateAssignedSkills(employeeModel);
                PopulateAssignedLocations(employeeModel);

                return View(employeeModel);
            }
            else
            {
                return Forbid(); // user is either not logged nor an admin, show forbidden page.
            }
        }

        /// <summary>
        /// GET: EmployeeModels/Edit/5
        /// </summary>
        /// <param name="id">ID of employee</param>
        /// <returns>Employee View</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            // check if user is loggen in and user is an admin.
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                // return not found if id is null
                if (id == null)
                {
                    return NotFound();
                }

                // get employee with skills, functions and locations
                var employeeModel = await _context.EmployeeModel
                    .Include(s => s.EmployeeSkills)
                        .ThenInclude(s => s.Skill)
                    .Include(s => s.EmployeeFunctions)
                        .ThenInclude(s => s.Function)
                    .Include(s => s.EmployeeLocations)
                        .ThenInclude(s => s.Location)
                    .FirstOrDefaultAsync(s => s.ID == id);

                // if employee not found, return not found
                if (employeeModel == null)
                {
                    return NotFound();
                }

                // call populate function
                PopulateAssignedFunctions(employeeModel);
                PopulateAssignedSkills(employeeModel);
                PopulateAssignedLocations(employeeModel);
                PopulateSupervisorsDropDownList(employeeModel.SupervisorID);

                return View(employeeModel);
            }
            else
            {
                return Forbid(); // user does not have access to this page.
            }
        }

        /// <summary>
        /// POST: EmployeeModels/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="selectedSkills"></param>
        /// <param name="selectedFunctions"></param>
        /// <param name="selectedLocations"></param>
        /// <param name="model"></param>
        /// <returns>Employee View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedSkills, string[] selectedFunctions, string[] selectedLocations, EmployeeViewModel model)
        {
            // check if user is loggen in and user is an admin.
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                // return not found if id is null
                if (id == null)
                {
                    return NotFound(); 
                }

                // get employee with skills, functions and locations
                var employeeToUpdate = await _context.EmployeeModel
                    .Include(s => s.EmployeeSkills)
                        .ThenInclude(s => s.Skill)
                    .Include(s => s.EmployeeFunctions)
                        .ThenInclude(s => s.Function)
                    .Include(s => s.EmployeeLocations)
                        .ThenInclude(s => s.Location)
                    .FirstOrDefaultAsync(s => s.ID == (int)id);

                string uniqueFileName = employeeToUpdate.ProfileImage; // set filename to users current image

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

                // try to update model
                if (await TryUpdateModelAsync<EmployeeModel>(employeeToUpdate, "",
                    e => e.Name, e => e.LastName, e => e.DoB, e => e.Gender, e => e.EMail, e => e.PhoneNumber, e => e.Admin, e => e.Experience, e => e.ProfileImage, e => e.SupervisorID))
                {
                    // call update function
                    UpdateEmployeeLocations(selectedLocations, employeeToUpdate);
                    UpdateEmployeeFunctions(selectedFunctions, employeeToUpdate);
                    UpdateEmployeeSkills(selectedSkills, employeeToUpdate);

                    // set employee image to new unique file name
                    employeeToUpdate.ProfileImage = uniqueFileName;

                    // try to save changes, store error.
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
                return Forbid(); // user is either not logged nor an admin, show forbidden page.
            }
        }

        /// <summary>
        /// GET: EmployeeModels/Delete/5
        /// </summary>
        /// <param name="id">ID of employee</param>
        /// <returns>Employee View</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            // check if user is logged in and an admin
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {

                // return not found if given id is null
                if (id == null)
                {
                    return NotFound();
                }

                // get employee model
                var employeeModel = await _context.EmployeeModel
                    .FirstOrDefaultAsync(m => m.ID == id);

                // return not found of employee is null (aka not found in database)
                if (employeeModel == null)
                {
                    return NotFound();
                }

                return View(employeeModel);
            }
            else
            {
                return Forbid(); // user does not have access to this page.
            }
        }

        /// <summary>
        /// POST: EmployeeModels/Delete/5
        /// </summary>
        /// <param name="id">Id of employee</param>
        /// <returns>Employee View</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                // get employee model
                var employeeModel = await _context.EmployeeModel.FindAsync(id);

                _context.EmployeeModel.Remove(employeeModel); // delete

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return Forbid(); // user does not have access
            }
        }

        /// <summary>
        /// Search database for existance of employee model
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True on found and false if not found</returns>
        private bool EmployeeModelExists(int id)
        {
            return _context.EmployeeModel.Any(e => e.ID == id); // search employee with given ID
        }

        /// <summary>
        /// Populates assigned skills of employee
        /// </summary>
        /// <param name="employee">Employee to populate</param>
        private void PopulateAssignedSkills(EmployeeModel employee)
        {
            // if employee has no skills create list
            if (employee.EmployeeSkills == null)
            {
                employee.EmployeeSkills = new List<EmployeeSkill>();
            }

            var allSkills = _context.Skill; //get all skills

            var employeeSkills = new HashSet<int>(employee.EmployeeSkills.Select(c => c.SkillID)); // get skills employe has

            var viewModel = new List<ShiftSkillsData>();

            // go trough all skills
            foreach (var skill in allSkills)
            {
                // add skill to viewModel
                viewModel.Add(new ShiftSkillsData
                {
                    SkillID = skill.SkillID,
                    Name = skill.Name,
                    Assigned = employeeSkills.Contains(skill.SkillID)
                });
            }

            // add skills to view data
            ViewData["Skills"] = viewModel;
        }

        /// <summary>
        /// Populates assigned functions of employee
        /// </summary>
        /// <param name="employee">Employee to populate</param>
        private void PopulateAssignedFunctions(EmployeeModel employee)
        {
            // create list if employee has no functions
            if (employee.EmployeeFunctions == null)
            {
                employee.EmployeeFunctions = new List<EmployeeFunction>();
            }

            var allFunctions = _context.Function; // get all functions from context

            var employeeFunctions = new HashSet<int>(employee.EmployeeFunctions.Select(c => c.FunctionID)); // get functions employee has

            var viewModel = new List<ShiftFunctionData>();

            // loop trough each functions
            foreach (var function in allFunctions)
            {
                // add to viewModel
                viewModel.Add(new ShiftFunctionData
                {
                    FunctionID = function.FunctionID,
                    Name = function.Name,
                    Assigned = employeeFunctions.Contains(function.FunctionID)
                });
            }

            // add view model to view data
            ViewData["Functions"] = viewModel;
        }

        /// <summary>
        /// Populates assigned locations of employee
        /// </summary>
        /// <param name="employee">Employee to populate</param>
        private void PopulateAssignedLocations(EmployeeModel employee)
        {
            // create list if user has no locations
            if (employee.EmployeeLocations == null)
            {
                employee.EmployeeLocations = new List<EmployeeLocations>();
            }

            var allLocations = _context.Locations; // get all locations from context
            var employeeLocations = new HashSet<int>(employee.EmployeeLocations.Select(c => c.LocationID)); // get locations of this employee
            var viewModel = new List<LocationData>();
            foreach (var location in allLocations)
            {
                // add to view model
                viewModel.Add(new LocationData
                {
                    LocationID = location.LocationID,
                    Name = location.Name,
                    Assigned = employeeLocations.Contains(location.LocationID)
                });
            }

            // add view model to view data
            ViewData["Locations"] = viewModel;
        }

        /// <summary>
        /// Update employee locations
        /// </summary>
        /// <param name="selectedLocations">Locations selected by user</param>
        /// <param name="employeeToUpdate">Employee Model for updating</param>
        private void UpdateEmployeeLocations(string[] selectedLocations, EmployeeModel employeeToUpdate)
        {
            // of none selected, set to empty list and return
            if (selectedLocations == null)
            {
                employeeToUpdate.EmployeeLocations = new List<EmployeeLocations>();
                return;
            }

            var selectedLocationsHS = new HashSet<string>(selectedLocations); // get hashed set of selected locations

            // if no existing locations create new list and set 
            if (employeeToUpdate.EmployeeLocations != null)
            {
                // create hash set to store locations in
                var employeeLocations = new HashSet<int>

                (employeeToUpdate.EmployeeLocations.Select(c => c.Location.LocationID));

                // go trough each location
                foreach (var location in _context.Locations)
                {
                    // if location was selected add to employee locations
                    if (selectedLocationsHS.Contains(location.LocationID.ToString()))
                    {
                        if (!employeeLocations.Contains(location.LocationID))
                        {
                            // add locations
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
                        // remove location if not selected
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
                // cteate list of employee locations
                employeeToUpdate.EmployeeLocations = new List<EmployeeLocations>();

                // go trough each location
                foreach (var location in _context.Locations)
                {
                    // if location is selected add to users locations
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

        /// <summary>
        /// Update employee functions
        /// </summary>
        /// <param name="selectedFunctions">Selected functions</param>
        /// <param name="employeeToUpdate">Employee to update</param>
        private void UpdateEmployeeFunctions(string[] selectedFunctions, EmployeeModel employeeToUpdate)
        {
            // of none selected set functions to functions to empty list
            if (selectedFunctions == null)
            {
                employeeToUpdate.EmployeeFunctions = new List<EmployeeFunction>();
                return;
            }

            var selectedFunctionsHS = new HashSet<string>(selectedFunctions); // create hash set of selected functions

            // check if user has exisitng functions
            if (employeeToUpdate.EmployeeFunctions != null)
            {
                // create hash set to store functions in
                var employeeFunctions = new HashSet<int>

                (employeeToUpdate.EmployeeFunctions.Select(c => c.Function.FunctionID));

                // go trough each functions
                foreach (var function in _context.Function)
                {
                    // if user has selected this function add
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
                        // if user has selected add
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
                // create new list of functions
                employeeToUpdate.EmployeeFunctions = new List<EmployeeFunction>();

                // go trough each functions
                foreach (var function in _context.Function)
                {
                    // if user has selected, add function to user
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

        /// <summary>
        /// Update selected employee skills
        /// </summary>
        /// <param name="selectedSkills">Skills selected</param>
        /// <param name="employeeToUpdate">Employee to update</param>
        private void UpdateEmployeeSkills(string[] selectedSkills, EmployeeModel employeeToUpdate)
        {
            //of no skills selected, set list to empty.
            if (selectedSkills == null)
            {
                employeeToUpdate.EmployeeSkills = new List<EmployeeSkill>();
                return;
            }

            // create hash set of selected skills
            var selectedSkillsHS = new HashSet<string>(selectedSkills);

            // check if user has existing skills
            if (employeeToUpdate.EmployeeSkills != null)
            {
                // create hash set to store skills in
                var employeeSkills = new HashSet<int>

                (employeeToUpdate.EmployeeSkills.Select(c => c.Skill.SkillID));

                // go trough each skill
                foreach (var skill in _context.Skill)
                {
                    // add to user if selected by user
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
                        // add to user if selected by user
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

                //go trough each skill
                foreach (var skill in _context.Skill)
                {
                    // add to user if selected by user
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
