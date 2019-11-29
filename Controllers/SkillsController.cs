using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gride.Data;
using Gride.Models;
using Microsoft.AspNetCore.Authorization;

namespace Gride.Controllers
{
    [Authorize]
    public class SkillsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SkillsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SkillModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Skill.ToListAsync());
        }

        // GET: SkillModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var skillModel = await _context.Skill
                .FirstOrDefaultAsync(m => m.SkillID == id);
            if (skillModel == null)
            {
                return NotFound();
            }

            return View(skillModel);
        }

        // GET: SkillModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SkillModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SkillID,Name,EmployeeModelID")] Skill skillModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(skillModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(skillModel);
        }

        // GET: SkillModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var skillModel = await _context.Skill.FindAsync(id);
            if (skillModel == null)
            {
                return NotFound();
            }
            return View(skillModel);
        }

        // POST: SkillModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SkillID,Name")] Skill skillModel)
        {
            if (id != skillModel.SkillID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(skillModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SkillExists(skillModel.SkillID))
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
            return View(skillModel);
        }

        // GET: SkillModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var skillModel = await _context.Skill
                .FirstOrDefaultAsync(m => m.SkillID == id);
            if (skillModel == null)
            {
                return NotFound();
            }

            return View(skillModel);
        }

        // POST: SkillModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var skillModel = await _context.Skill.FindAsync(id);
            _context.Skill.Remove(skillModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SkillExists(int id)
        {
            return _context.Skill.Any(e => e.SkillID == id);
        }
    }
}
