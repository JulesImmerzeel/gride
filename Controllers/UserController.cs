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

namespace Gride.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            EmployeeModel employee = await _context.EmployeeModel
                                        .Include(e => e.Skills)
                                        .Include(f => f.Functions)
                                        .Include(l => l.Locations)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(m => m.EMail == User.Identity.Name);

            return View(employee);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeModelID,Name,LastName,DoB,Gender,EMail,PhoneNumber,Admin,Skills,Functions,LoginID,Experience,Locations,ProfileImage")] EmployeeModel employeeModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeeModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employeeModel);
        }

        // GET: Employee/Edit
        public async Task<IActionResult> Edit()
        {
            EmployeeModel employee = await _context.EmployeeModel
                                        .Include(e => e.Skills)
                                        .Include(f => f.Functions)
                                        .Include(l => l.Locations)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(m => m.EMail == User.Identity.Name);

            if (EmployeeModel.Equals(employee, null)){
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employee/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ID,Name,LastName,DoB,Gender,EMail,PhoneNumber,Admin,LoginID,Experience,ProfileImage,Skills,Functions,Locations")] EmployeeModel employeeModel)
        {
             if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeeModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeModelExists(employeeModel.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var employee = await _context.EmployeeModel
                                        .Include(e => e.Skills)
                                        .Include(f => f.Functions)
                                        .Include(l => l.Locations)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(m => m.EMail == User.Identity.Name);

            foreach (var s in employee.Skills)
            {
                employeeModel.Skills.Add(s);
            }
            foreach (var l in employee.Locations)
            {
                employeeModel.Locations.Add(l);
            }
            foreach (var f in employee.Functions)
            {
                employeeModel.Functions.Add(f);
            }

            return View(employeeModel);
        }

        //Get: Employee/Request/itemToChange
        public async Task<IActionResult> Request(string item)
        {
            EmployeeModel employee = await _context.EmployeeModel
                                        .Include(e => e.Skills)
                                        .Include(f => f.Functions)
                                        .Include(l => l.Locations)
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

        private bool EmployeeModelExists(uint id)
        {
            return _context.EmployeeModel.Any(e => e.ID == id);
        }
    }
}
