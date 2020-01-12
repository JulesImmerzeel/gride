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
    public class WorksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;

        /// <summary>
        /// Init works controller
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="signInManager">Sing in manager</param>
        /// <param name="userManager">user manager</param>
        public WorksController(ApplicationDbContext context,
                               SignInManager<IdentityUser> signInManager,
                               UserManager<IdentityUser> userManager)
        {
            _context = context;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        /// <summary>
        /// GET: Works
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            // Check if user is logged in and admin
            if (User.Identity.IsAuthenticated && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                var applicationDbContext = _context.Works.Include(w => w.Employee).Include(w => w.Shift);
                return View(await applicationDbContext.ToListAsync());
            }
            else
            {
                return Forbid(); // user does not have access to this page
            }
        }


        /// <summary>
        /// GET: Works/Details/5
        /// </summary>
        /// <param name="id">Id of works</param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            // check if user is logged in and is admin
            if (User.Identity.IsAuthenticated && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                //if no id is given return not found view
                if (id == null)
                {
                    return NotFound();
                }

                // get works and include employee and shift
                var work = await _context.Works
                    .Include(w => w.Employee)
                    .Include(w => w.Shift)
                    .FirstOrDefaultAsync(m => m.WorkID == id);

                // check if found
                if (work == null)
                {
                    return NotFound(); // return not found if works is null
                }

                return View(work); // return view with work model
            }
            else
            {
                return Forbid(); // user does not have access to this page
            }
        }

        /// <summary>
        /// GET: Works/Create
        /// </summary>
        /// <param name="ShiftId">Optional shift id to create works for</param>
        /// <returns></returns>
        public IActionResult Create(int ShiftId = 0)
        {
            // check if user is logged in and is admin
            if (User.Identity.IsAuthenticated && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                // create selected list for employees
                ViewData["EmployeeID"] = new SelectList(_context.EmployeeModel, "ID", "EMail");

                // select select list with shift id, and if shift id is given make it selected
                ViewData["ShiftID"] = new SelectList(_context.Shift, "ShiftID", "ShiftID", ShiftId);

                return View();
            }
            else
            {
                return Forbid(); // user does not have access to this page
            }
        }

        /// <summary>
        /// POST: Works/Create
        /// </summary>
        /// <param name="work">Work model</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkID,EmployeeID,ShiftID,Overtime,Delay")] Work work)
        {
            // check if user is logged in and is admin
            if (User.Identity.IsAuthenticated && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                // check if model is valid
                if (ModelState.IsValid)
                {
                    _context.Add(work);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                // create dropdown list for employee with current employee selected
                ViewData["EmployeeID"] = new SelectList(_context.EmployeeModel, "ID", "EMail", work.EmployeeID);
                // create shift dropdownlist with current shift selected
                ViewData["ShiftID"] = new SelectList(_context.Shift, "ShiftID", "ShiftID", work.ShiftID);

                return View(work);
            }
            else
            {
                return Forbid(); // user does not have access to this page
            }
        }

        /// <summary>
        /// GET: Works/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id)
        {
            // check if user is logged in and is an admin
            if (User.Identity.IsAuthenticated && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                // if no id is given return not found view
                if (id == null)
                {
                    return NotFound();
                }

                // find work
                var work = await _context.Works.FindAsync(id);

                // if not found, return not found view
                if (work == null)
                {
                    return NotFound();
                }

                // create dropdownlist for employee, with current employee selected
                ViewData["EmployeeID"] = new SelectList(_context.EmployeeModel, "ID", "EMail", work.EmployeeID);
                // create dropdownllist for shift with current shift selected
                ViewData["ShiftID"] = new SelectList(_context.Shift, "ShiftID", "ShiftID", work.ShiftID);
                return View(work);
            }
            else
            {
                return Forbid(); // user does not have access to this page
            }
        }

        /// <summary>
        /// POST: Works/Edit/5
        /// </summary>
        /// <param name="id">Id of work to edit</param>
        /// <param name="work">Work model</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkID,EmployeeID,ShiftID,Overtime,Delay")] Work work)
        {
            // check if user is logged in and an admin
            if (User.Identity.IsAuthenticated && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                // if no id is given return not found view
                if (id != work.WorkID)
                {
                    return NotFound();
                }

                // check if model is valid, else try to add/update to db
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(work);
                        await _context.SaveChangesAsync(); // save changes
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!WorkExists(work.WorkID))
                        {
                            return NotFound(); // return not found view of unsuccessfull
                        }
                        else
                        {
                            throw;
                        }
                    }


                    return RedirectToAction(nameof(Index));
                }

                // create dropdownlist for employee, with current employee selected
                ViewData["EmployeeID"] = new SelectList(_context.EmployeeModel, "ID", "EMail", work.EmployeeID);
                // create dropdownlist for shift, with current shift selected
                ViewData["ShiftID"] = new SelectList(_context.Shift, "ShiftID", "ShiftID", work.ShiftID);

                return View(work);
            }
            else
            {
                return Forbid(); // user does not have access to this page
            }
        }

        /// <summary>
        /// GET: Works/Delete/5
        /// </summary>
        /// <param name="id">Id of works to delete</param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id)
        {
            // check if user is logged in and is an admin
            if (User.Identity.IsAuthenticated && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                // return not found if given id is null
                if (id == null)
                {
                    return NotFound();
                }

                // get work and include employee and shift
                var work = await _context.Works
                    .Include(w => w.Employee)
                    .Include(w => w.Shift)
                    .FirstOrDefaultAsync(m => m.WorkID == id);

                // return not found, if model can not be found in db
                if (work == null)
                {
                    return NotFound();
                }

                return View(work);
            }
            else
            {
                return Forbid(); // user does not have access to this page
            }
        }

        /// <summary>
        /// POST: Works/Delete/5
        /// </summary>
        /// <param name="id">ID of works to delete</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // check if user is logged in and is an admin
            if (User.Identity.IsAuthenticated && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                var work = await _context.Works.FindAsync(id);
                _context.Works.Remove(work);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return Forbid(); //user does not habve access to this page
            }
        }

        /// <summary>
        /// Check if works exists by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Boolean</returns>
        private bool WorkExists(int id)
        {
            return _context.Works.Any(e => e.WorkID == id);
        }
    }
}
