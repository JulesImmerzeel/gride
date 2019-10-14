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
using Gride.Models.Binds;
using Gride.Classes;

namespace Gride.Controllers
{
    public class ShiftsController : Controller
    {
		private readonly ApplicationDbContext _context;

		private ICollection<Skill> Skills
		{
			get => HttpContext.Session.Get<ICollection<Skill>>("Skills");
			set => HttpContext.Session.Set("Skills", value);
		}

		private ICollection<ShiftAndFunctionBind> FunctionBinds
		{
			get => HttpContext.Session.Get<ICollection<ShiftAndFunctionBind>>("FunctionBinds");
			set => HttpContext.Session.Set("FunctionBinds", value);
		}

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
				.Include(s => s.Skills)
				.Include(s => s.Functions)
                .FirstOrDefaultAsync(m => m.ShiftID == id);
			ViewData["Skills"] = shift.Skills;
			for (int i = 0; i < shift.Functions.Count; i++)
				shift.Functions.ToList()[i].Function = await _context.Function.FirstAsync(x => x.FunctionID == shift.Functions.ToList()[i].FunctionID);
			List<ShiftAndFunctionBind> functionBinds = new List<ShiftAndFunctionBind>();
			foreach (ShiftAndFunctionBind functionBind in shift.Functions)
				functionBinds.Add(new ShiftAndFunctionBind()
				{
					Function = functionBind.Function,
					FunctionID = functionBind.FunctionID,
					maxEmployees = functionBind.maxEmployees,
					ShiftID = shift.ShiftID,
				});
			FunctionBinds = functionBinds;
			ViewData["FunctionBinds"] = shift.Functions;

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
			ViewData["Function"] = new SelectList(_context.Function, "FunctionID", "Name");
			
			Skills = new List<Skill>();
			ViewData["Skills"] = Skills;

			FunctionBinds = new List<ShiftAndFunctionBind>();
			ViewData["FunctionBinds"] = FunctionBinds;

			return View();
        }

        // POST: Shifts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShiftID,Start,End,LocationID,Skills,Functions")] Shift shift)
        {
			var s = Skills;

			if (shift.Skills == null)
				shift.Skills = new List<Skill>();
			else
				shift.Skills.Clear();

			// Me trying to fix an identity error using the exact same objects (so also the same hash codes) in the database
			foreach (Skill skill in s)
				shift.Skills.Add(_context.Skill.ToList().Find(x => skill.Name == x.Name && skill.SkillID == x.SkillID));

			shift.Location = await _context.Location.FirstAsync(x => x.LocationID == shift.LocationID);

			if (ModelState.IsValid)
			{
				_context.Add(shift);

				foreach (ShiftAndFunctionBind functionBind in FunctionBinds)
				{
					Function func = _context.Function.ToList().Find(x => x.FunctionID == functionBind.FunctionID);
					ShiftAndFunctionBind actualbind = new ShiftAndFunctionBind()
					{
						Shift = shift,
						ShiftID = shift.ShiftID,
						Function = func,
						FunctionID = func.FunctionID,
						maxEmployees = functionBind.maxEmployees,
					};
					shift.Functions.Add(actualbind);
					_context.Add(actualbind);
				}
				await _context.SaveChangesAsync();
			
				return RedirectToAction(nameof(Index));
			}

            ViewData["LocationID"] = new SelectList(_context.Location, "LocationID", "Name", shift.LocationID);
			ViewData["Skill"] = new SelectList(_context.Skill, "SkillID", "Name");
			ViewData["Function"] = new SelectList(_context.Function, "FunctionID", "Name");
			ViewData["Skills"] = shift.Skills;
			ViewData["FunctionBinds"] = shift.Functions;
			return View(shift);
        }

		[HttpPost, ActionName("AddSkill")]
		public async void AddSkill([Bind("SkillID,Name")] Skill skill)
		{
			ICollection<Skill> skills = Skills;
			skills.Add(skill);
			Skills = skills;
		}

		[HttpPost, ActionName("DeleteSkill")]
		public async void RemoveSkill([Bind("SkillID,Name")] Skill skill)
		{
			ICollection<Skill> skills = Skills;
			skills.ToList().RemoveAll(x => x.SkillID == skill.SkillID);
			Skills = skills;
		}

		[HttpPost, ActionName("AddFunc")]
		public async void AddFunc([Bind("FunctionID,Function,maxEmployees")] ShiftAndFunctionBind functionBind)
		{
			ICollection<ShiftAndFunctionBind> funcs = FunctionBinds;
			funcs.Add(functionBind);
			FunctionBinds = funcs;
		}

		[HttpPost, ActionName("DeleteFunc")]
		public async void RemoveFunc([Bind("FunctionID,Function,maxEmployees")] ShiftAndFunctionBind functionBind)
		{
			ICollection<ShiftAndFunctionBind> funcs = FunctionBinds;
			funcs.ToList().RemoveAll(x => x.FunctionID == functionBind.FunctionID);
			FunctionBinds = funcs;
		}

		// GET: Shifts/Edit/5
		public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shift = await _context.Shift.Include(x => x.Skills).Include(x => x.Functions).FirstOrDefaultAsync(x => x.ShiftID == id);
            if (shift == null)
            {
                return NotFound();
            }
            ViewData["LocationID"] = new SelectList(_context.Location, "LocationID", "Name", shift.LocationID);
			ViewData["Skill"] = new SelectList(_context.Skill, "SkillID", "Name");
			ViewData["Function"] = new SelectList(_context.Function, "FunctionID", "Name");

			Skills = shift.Skills;
			ViewData["Skills"] = shift.Skills;
			for (int i = 0; i < shift.Functions.Count; i++)
				shift.Functions.ToList()[i].Function = await _context.Function.FirstAsync(x => x.FunctionID == shift.Functions.ToList()[i].FunctionID);
			List<ShiftAndFunctionBind> functionBinds = new List<ShiftAndFunctionBind>();
			foreach (ShiftAndFunctionBind functionBind in shift.Functions)
				functionBinds.Add(new ShiftAndFunctionBind()
				{
					Function = functionBind.Function,
					FunctionID = functionBind.FunctionID,
					maxEmployees = functionBind.maxEmployees,
					ShiftID = shift.ShiftID,
				});
			FunctionBinds = functionBinds;
			ViewData["FunctionBinds"] = shift.Functions;
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
					var s = Skills;

					if (shift.Skills == null)
						shift.Skills = new List<Skill>();
					else
						shift.Skills.Clear();

					// Me trying to fix an identity error using the exact same objects (so also the same hash codes) in the database
					foreach (Skill skill in s)
						shift.Skills.Add(_context.Skill.ToList().Find(x => skill.Name == x.Name && skill.SkillID == x.SkillID));

					shift.Functions.Clear();

					foreach (ShiftAndFunctionBind functionBind in FunctionBinds)
					{
						Function func = _context.Function.ToList().Find(x => x.FunctionID == functionBind.FunctionID);
						ShiftAndFunctionBind actualbind = new ShiftAndFunctionBind()
						{
							Shift = shift,
							ShiftID = shift.ShiftID,
							Function = func,
							FunctionID = func.FunctionID,
							maxEmployees = functionBind.maxEmployees,
						};
						shift.Functions.Add(actualbind);
						_context.Add(actualbind);
					}

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
			for (int i = 0; i < shift.Functions.Count; i++)
				shift.Functions.ToList()[i].Function = await _context.Function.FirstAsync(x => x.FunctionID == shift.Functions.ToList()[i].FunctionID);
			ViewData["Skill"] = new SelectList(_context.Skill, "SkillID", "Name");
			ViewData["Function"] = new SelectList(_context.Function, "FunctionID", "Name");
			Skills = shift.Skills;
			ViewData["Skills"] = shift.Skills;
			List<ShiftAndFunctionBind> functionBinds = new List<ShiftAndFunctionBind>();
			foreach (ShiftAndFunctionBind functionBind in shift.Functions)
				functionBinds.Add(new ShiftAndFunctionBind()
				{
					Function = functionBind.Function,
					FunctionID = functionBind.FunctionID,
					maxEmployees = functionBind.maxEmployees,
					ShiftID = shift.ShiftID,
				});
			FunctionBinds = functionBinds;
			ViewData["FunctionBinds"] = shift.Functions;
			return View(shift);
        }

        // GET: Shifts/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

			var shift = await _context.Shift.Include(x => x.Location).Include(x => x.Skills).Include(x => x.Functions).FirstOrDefaultAsync(x => x.ShiftID == id);
			if (shift == null)
            {
                return NotFound();
            }
			for (int i = 0; i < shift.Functions.Count; i++)
				shift.Functions.ToList()[i].Function = await _context.Function.FirstAsync(x => x.FunctionID == shift.Functions.ToList()[i].FunctionID);
			ViewData["Skill"] = new SelectList(_context.Skill, "SkillID", "Name");
			ViewData["Function"] = new SelectList(_context.Function, "FunctionID", "Name");
			Skills = shift.Skills;
			ViewData["Skills"] = shift.Skills;
			List<ShiftAndFunctionBind> functionBinds = new List<ShiftAndFunctionBind>();
			foreach (ShiftAndFunctionBind functionBind in shift.Functions)
				functionBinds.Add(new ShiftAndFunctionBind()
				{
					Function = functionBind.Function,
					FunctionID = functionBind.FunctionID,
					maxEmployees = functionBind.maxEmployees,
					ShiftID = shift.ShiftID,
				});
			FunctionBinds = functionBinds;
			ViewData["FunctionBinds"] = shift.Functions;
			return View(shift);
        }

        // POST: Shifts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var shift = await _context.Shift.Include(s => s.Skills).Include(s => s.Functions).FirstAsync(s => s.ShiftID == id);
			shift.Skills.Clear();
			foreach (ShiftAndFunctionBind bind in shift.Functions)
				_context.shiftAndFunctionBind.Remove(bind);
			shift.Functions.Clear();
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
