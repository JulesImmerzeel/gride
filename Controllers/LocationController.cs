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
    public class LocationController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;

        public LocationController(ApplicationDbContext context, 
                                  SignInManager<IdentityUser> signInManager,
                                  UserManager<IdentityUser> userManager)
        {
            _context = context;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        // GET: Location
        public async Task<IActionResult> Index()
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                return View(await _context.Locations.ToListAsync());
            }

            else
            {
                return Redirect("https://localhost:44368/");
            }
        }

        // GET: Location/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {


                if (id == null)
                {
                    return NotFound();
                }

                var function = await _context.Locations
                    .FirstOrDefaultAsync(m => m.LocationID == id);
                if (function == null)
                {
                    return NotFound();
                }

                return View(function);
            }
            else
            {
                return NotFound();
                return Redirect("https://localhost:44368/");
            }
        }

        // GET: Location/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Location/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LocationID,Name,Street, StreetNumber, Additions, Postalcode, City, Country")] Location location)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(location);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(location);
            }
            else
            {
                return Redirect("https://localhost:44368/");
            }
        }

        // GET: Location/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            return View(location);
            }
            else
            {
                return Redirect("https://localhost:44368/");
            }
        }

        // POST: Location/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind( "LocationID,Name,Street, StreetNumber, Additions, Postalcode, City, Country")] Location location)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                if (id != location.LocationID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(location);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationExists(location.LocationID))
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
            return View(location);

            }
            else
            {
                return Redirect("https://localhost:44368/");
            }
        }

        // GET: Location/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                if (id == null)
            {
                return NotFound();
            }

            var function = await _context.Locations
                .FirstOrDefaultAsync(m => m.LocationID == id);
            if (function == null)
            {
                return NotFound();
            }

            return View(function);
            }
            else
            {
                return Redirect("https://localhost:44368/");
            }
        }

        // POST: Location/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (signInManager.IsSignedIn(User) && _context.EmployeeModel.Single(x => x.EMail == User.Identity.Name).Admin)
            {
                var location = await _context.Locations.FindAsync(id);
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            }
            else
            {
                return Redirect("https://localhost:44368/");
            }
        }

        private bool LocationExists(int id)
        {
            return _context.Locations.Any(e => e.LocationID == id);
        }
    }
}