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
    public class WorksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Works
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Works.Include(w => w.Employee).Include(w => w.Shift);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Works/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var work = await _context.Works
                .Include(w => w.Employee)
                .Include(w => w.Shift)
                .FirstOrDefaultAsync(m => m.WorkID == id);
            if (work == null)
            {
                return NotFound();
            }

            return View(work);
        }

        // GET: Works/Create
        public IActionResult Create(int ShiftId = 0)
        {
            ViewData["EmployeeID"] = new SelectList(_context.EmployeeModel, "ID", "EMail");
            ViewData["ShiftID"] = new SelectList(_context.Shift, "ShiftID", "ShiftID", ShiftId); ;
            return View();
        }

        // POST: Works/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkID,EmployeeID,ShiftID,Overtime,Delay")] Work work)
        {
            if (ModelState.IsValid)
            {
                _context.Add(work);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeID"] = new SelectList(_context.EmployeeModel, "ID", "EMail", work.EmployeeID);
            ViewData["ShiftID"] = new SelectList(_context.Shift, "ShiftID", "ShiftID", work.ShiftID);
            return View(work);
        }

        // GET: Works/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var work = await _context.Works.FindAsync(id);
            if (work == null)
            {
                return NotFound();
            }
            ViewData["EmployeeID"] = new SelectList(_context.EmployeeModel, "ID", "EMail", work.EmployeeID);
            ViewData["ShiftID"] = new SelectList(_context.Shift, "ShiftID", "ShiftID", work.ShiftID);
            return View(work);
        }

        // POST: Works/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkID,EmployeeID,ShiftID,Overtime,Delay")] Work work)
        {
            if (id != work.WorkID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(work);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkExists(work.WorkID))
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
            ViewData["EmployeeID"] = new SelectList(_context.EmployeeModel, "ID", "EMail", work.EmployeeID);
            ViewData["ShiftID"] = new SelectList(_context.Shift, "ShiftID", "ShiftID", work.ShiftID);
            return View(work);
        }

        // GET: Works/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var work = await _context.Works
                .Include(w => w.Employee)
                .Include(w => w.Shift)
                .FirstOrDefaultAsync(m => m.WorkID == id);
            if (work == null)
            {
                return NotFound();
            }

            return View(work);
        }

        // POST: Works/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var work = await _context.Works.FindAsync(id);
            _context.Works.Remove(work);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkExists(int id)
        {
            return _context.Works.Any(e => e.WorkID == id);
        }
    }
}
