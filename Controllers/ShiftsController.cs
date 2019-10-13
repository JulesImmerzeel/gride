using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Session;
using Gride.Data;
using Gride.Models;
using Gride.Classes;

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
            var applicationDbContext = _context.Shift.Include(s => s.Location).Include( s => s.Functions).Include(s => s.Skills);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Shifts/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shift = await _context.Shift
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
            ViewData["LocationID"] = new SelectList(_context.Location, "LocationID", "Name");
			ViewData["Skill"] = new SelectList(_context.Skill, "SkillID", "Name");
			List<Skill> skills = new List<Skill>();
			HttpContext.Session.Set("Skills", skills);
			ViewData["Skills"] = skills;
			
            return View();
        }

        // POST: Shifts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShiftID,Start,End,LocationID,Skills,Functions")] Shift shift)
        {
			shift.Skills = HttpContext.Session.Get<List<Skill>>("Skills");
            if (ModelState.IsValid)
            {
                _context.Add(shift);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LocationID"] = new SelectList(_context.Location, "LocationID", "Name", shift.LocationID);
			ViewData["Skill"] = new SelectList(_context.Skill, "SkillID", "Name");
			ViewData["Skills"] = shift.Skills as List<Skill>;
			return View(shift);
        }

		[HttpPost, ActionName("AddSkill")]
		public async void AddSkill([Bind("SkillID,Name")] Skill skill)
		{
			List<Skill> skills = HttpContext.Session.Get<List<Skill>>("Skills");
			skills.Add(_context.Skill.ToList().Find(x => skill.Name == x.Name && skill.SkillID == x.SkillID));
			HttpContext.Session.Set("Skills", skills);
		}

		[HttpPost, ActionName("DeleteSkill")]
		public async void RemoveSkill([Bind("SkillID,Name")] Skill skill)
		{
			List<Skill> skills = HttpContext.Session.Get<List<Skill>>("Skills");
			skills.RemoveAll(x => skill.Name == x.Name && skill.SkillID == x.SkillID);
			HttpContext.Session.Set("Skills", skills);
		}

		// GET: Shifts/Edit/5
		public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shift = await _context.Shift.FindAsync(id);
            if (shift == null)
            {
                return NotFound();
            }
            ViewData["LocationID"] = new SelectList(_context.Location, "LocationID", "Name", shift.LocationID);
			ViewData["Skill"] = new SelectList(_context.Skill, "SkillID", "Name");
			HttpContext.Session.Set("Skills", shift.Skills as List<Skill>);
			ViewData["Skills"] = shift.Skills as List<Skill>;
			return View(shift);
        }

        // POST: Shifts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ShiftID,Start,End,LocationID")] Shift shift)
        {
            if (id != shift.ShiftID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shift);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShiftExists(shift.ShiftID))
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
            ViewData["LocationID"] = new SelectList(_context.Location, "LocationID", "Name", shift.LocationID);
			ViewData["Skill"] = new SelectList(_context.Skill, "SkillID", "Name");
			HttpContext.Session.Set("Skills", shift.Skills as List<Skill>);
			ViewData["Skills"] = shift.Skills as List<Skill>;
			return View(shift);
        }

        // GET: Shifts/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shift = await _context.Shift
                .Include(s => s.Location)
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
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var shift = await _context.Shift.FindAsync(id);
            _context.Shift.Remove(shift);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShiftExists(long id)
        {
            return _context.Shift.Any(e => e.ShiftID == id);
        }
    }
}
