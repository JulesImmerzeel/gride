using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gride.Data;
using Gride.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Gride.Controllers
{
    [Authorize]
    public class FunctionController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;

        public FunctionController(ApplicationDbContext context,
                                  SignInManager<IdentityUser> signInManager,
                                  UserManager<IdentityUser> userManager)
        {
            _context = context;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        // GET: Function
        public async Task<IActionResult> Index()
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                return View(await _context.Function.ToListAsync());
            }
            else
            {
                return Forbid();
            }
        }

        // GET: Function/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var function = await _context.Function
                    .FirstOrDefaultAsync(m => m.FunctionID == id);
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

        // GET: Function/Create
        public IActionResult Create()
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                return View();
            }
            else
            {
                return Forbid();
            }
        }

        // POST: Function/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FunctionID,Name,EmployeeModelID")] Function function)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(function);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(function);
            }
            else
            {
                return Forbid();
            }
        }

        // GET: Function/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var function = await _context.Function.FindAsync(id);
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

        // POST: Function/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FunctionID,Name,EmployeeID")] Function function)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                if (id != function.FunctionID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(function);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!FunctionExists(function.FunctionID))
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
                return View(function);
            }
            else
            {
                return Forbid();
            }
        }

        // GET: Function/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var function = await _context.Function
                    .FirstOrDefaultAsync(m => m.FunctionID == id);
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

        // POST: Function/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                var function = await _context.Function.FindAsync(id);
                _context.Function.Remove(function);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return Forbid();
            }
        }

        private bool FunctionExists(int id)
        {
            return _context.Function.Any(e => e.FunctionID == id);
        }
    }
}