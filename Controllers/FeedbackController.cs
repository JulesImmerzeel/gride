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
using Microsoft.AspNetCore.Identity;

namespace Gride.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;

        public FeedbackController(ApplicationDbContext context,
                                SignInManager<IdentityUser> signInManager,
                                UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
            _context = context;
            this.signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                return View(await _context.Feedback.ToListAsync());
            }
            else
            {
                return Forbid();
            }
        }


        public IActionResult Create()
        {
                return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, Title, FeedbackDescription, FeedbackPostDate, Fixed")] Feedback feedback)
        {
            if (signInManager.IsSignedIn(User))
            {
                if (ModelState.IsValid)
                {
                    DateTime today = DateTime.Now;
                    feedback.Fixed = false;
                    feedback.FeedbackPostDate = today;
                    _context.Add(feedback);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(feedback);
            }
            else
            {
                return Forbid();
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {

                if (id == null)
                {
                    return NotFound();
                }

                var function = await _context.Feedback
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (function == null)
                {
                    return NotFound();
                }

                return View(function);
            }
            else
            {
                return Forbid();
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var function = await _context.Feedback
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (function == null)
                {
                    return NotFound();
                }

                return View(function);
            }
            else
            {
                return Forbid();
            }
        }

        // POST: Location/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                var feedback = await _context.Feedback.FindAsync(id);
                _context.Feedback.Remove(feedback);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return Forbid();
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var feedback = await _context.Feedback.FindAsync(id);
                if (feedback == null)
                {
                    return NotFound();
                }
                return View(feedback);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Title, FeedbackDescription, FeedbackPostDate, Fixed")] Feedback feedback)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                if (id != feedback.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(feedback);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!FeedbackExists(feedback.Id))
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
                return View(feedback);

            }
            else
            {
                return Forbid();
            }
        }

        private bool FeedbackExists(int id)
        {
            return _context.Feedback.Any(e => e.Id == id);
        }
    }
}