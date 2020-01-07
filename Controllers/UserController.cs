using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gride.Data;
using Gride.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Gride.ViewModels;
using Microsoft.AspNetCore.Hosting;

namespace Gride.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;

        public UserController(ApplicationDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            EmployeeModel employee = await _context.EmployeeModel
                                        .Include(e => e.EmployeeSkills)
                                            .ThenInclude(e => e.Skill)
                                        .Include(f => f.EmployeeFunctions)
                                            .ThenInclude(f => f.Function)
                                        .Include(l => l.EmployeeLocations)
                                            .ThenInclude(l => l.Location)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(m => m.EMail == User.Identity.Name);

            return View(employee);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            return View();
        }

        // GET: Employee/Edit
        public async Task<IActionResult> Edit()
        {
            EmployeeModel employee = await _context.EmployeeModel
                                        .Include(e => e.EmployeeSkills)
                                            .ThenInclude(e => e.Skill)
                                        .Include(f => f.EmployeeFunctions)
                                            .ThenInclude(f => f.Function)
                                        .Include(l => l.EmployeeLocations)
                                            .ThenInclude(l => l.Location)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(m => m.EMail == User.Identity.Name);

            if (EmployeeModel.Equals(employee, null)){
                return NotFound();
            }

            PopulateAssignedSkills(employee);
            PopulateAssignedFunctions(employee);
            PopulateAssignedLocations(employee);

            return View(employee);
        }

        private void PopulateAssignedSkills(EmployeeModel employee)
        {
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


        // POST: Employee/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedLocations, EmployeeViewModel model)
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
                e => e.Name, e => e.LastName, e => e.DoB, e => e.Gender, e => e.EMail, e => e.PhoneNumber, e => e.Admin, e => e.Experience, e => e.ProfileImage))
            {
                UpdateEmployeeLocations(selectedLocations, employeeToUpdate);

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

        //Get: Employee/Request/itemToChange
        public async Task<IActionResult> Request(string item)
        {
            EmployeeModel employee = await _context.EmployeeModel
                                        .Include(e => e.EmployeeSkills)
                                            .ThenInclude( e => e.Skill)
                                        .Include(f => f.EmployeeFunctions)
                                            .ThenInclude(f => f.Function)
                                        .Include(l => l.EmployeeLocations)
                                            .ThenInclude(l => l.Location)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(m => m.EMail == User.Identity.Name);

            if (EmployeeModel.Equals(employee, null))
            {
                return NotFound();
            }

            return View(employee);
        }

        //Post: Employee/Request/itemToChange
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Request()
        {
            return RedirectToAction(nameof(Index));
        }

            // GET: Employee/Delete/5
            public async Task<IActionResult> Delete(uint? id)
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

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(uint id)
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
    }
}
