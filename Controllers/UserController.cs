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
            Employee employee = await _context.Employee
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
        public async Task<IActionResult> Create([Bind("EmployeeID,Name,LastName,DoB,Gender,EMail,PhoneNumber,Admin,LoginID,Experience,ProfileImage")] Employee Employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(Employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(Employee);
        }

        // GET: Employee/Edit
        public async Task<IActionResult> Edit()
        {
            Employee employee = await _context.Employee
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(m => m.EMail == User.Identity.Name);

            if (Employee.Equals(employee, null)){
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employee/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("EmployeeID,Name,LastName,DoB,Gender,EMail,PhoneNumber,Admin,LoginID,Experience,ProfileImage")] Employee Employee)
        {
             if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(Employee.EmployeeID))
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

            var employee = await _context.Employee
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(m => m.EMail == User.Identity.Name);

            employee.Skills = employee.Skills;
            employee.Functions = employee.Functions;
            employee.Locations = employee.Locations;

            _context.Update(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //Get: Employee/Request/itemToChange
        public new async Task<IActionResult> Request(string item)
        {
            Employee employee = await _context.Employee
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(m => m.EMail == User.Identity.Name);

            if (Employee.Equals(employee, null))
            {
                return NotFound();
            }

            return View(employee);
        }

        //Post: Employee/Request/itemToChange
        [HttpPost]
        [ValidateAntiForgeryToken]
#pragma warning disable 1998
        public new async Task<IActionResult> Request()
        {
            return RedirectToAction(nameof(Index));
        }
#pragma warning restore 1998

            // GET: Employee/Delete/5
            public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Employee = await _context.Employee
                .FirstOrDefaultAsync(m => m.EmployeeID == id);
            if (Employee == null)
            {
                return NotFound();
            }

            return View(Employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var Employee = await _context.Employee.FindAsync(id);
            _context.Employee.Remove(Employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(long id)
        {
            return _context.Employee.Any(e => e.EmployeeID == id);
        }
    }
}
