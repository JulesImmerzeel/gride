using Gride.Data;
using Gride.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Threading.Tasks;

namespace Gride.Controllers
{
    /// <summary>
    /// A controller for all the skills pages
    /// </summary>
    [Authorize]
    public class SkillsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;

        /// <summary>
        /// The constructor
        /// </summary>
        public SkillsController(ApplicationDbContext context,
                                SignInManager<IdentityUser> signInManager,
                                UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
            _context = context;
            this.signInManager = signInManager;
        }

        // GET: SkillModels
        /// <summary>
        /// A basic protected page
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // checks if the user is loged in and an admin
            if (User.Identity.IsAuthenticated && _context.EmployeeModel.ToList().Find(x => x.EMail == User.Identity.Name).Admin)
            {
                return View(await _context.Skill.ToListAsync());
            }
            else
            {
                return Forbid();
            }
        }

        // GET: SkillModels/Details/5
        /// <summary>
        /// A page that shows the details of the skill with id <paramref name="id"/>
        /// </summary>
        /// <param name="id">The id of the skill</param>
        public async Task<IActionResult> Details(int? id)
        {
            // checks if the user is loged in and an admin
            if (User.Identity.IsAuthenticated && _context.EmployeeModel.ToList().Find(x => x.EMail == User.Identity.Name).Admin)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var skillModel = await _context.Skill
                    .FirstOrDefaultAsync(m => m.SkillID == id);
                // returns not found if the skill couldn't be found
                if (skillModel == null)
                {
                    return NotFound();
                }

                return View(skillModel);
            }
            else
            {
                return Forbid();
            }
        }

        // GET: SkillModels/Create
        /// <summary>
        /// A page that is responsible to show the what is necessary to make a skill
        /// </summary>
        public IActionResult Create()
        {
            // checks if the user is loged in and an admin
            if (User.Identity.IsAuthenticated && _context.EmployeeModel.ToList().Find(x => x.EMail == User.Identity.Name).Admin)
            {
                return View();

            }
            else
            {
                return Forbid();
            }
        }

        // POST: SkillModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Takes the skill from <see cref="Create"/> and adds it to the database
        /// </summary>
        /// <param name="skillModel">The skill that should be added to the database</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SkillID,Name")] Skill skillModel)
        {
            // checks if the user is loged in and an admin
            if (User.Identity.IsAuthenticated && _context.EmployeeModel.ToList().Find(x => x.EMail == User.Identity.Name).Admin)
            {
                // checks if the model is able to be added to the database
                if (ModelState.IsValid)
                {
                    // adds it to the database
                    _context.Add(skillModel);
                    await _context.SaveChangesAsync();
                    // redirect to index
                    return RedirectToAction(nameof(Index));
                }
                return View(skillModel);
            }
            else
            {
                return Forbid();
            }
        }

        // GET: SkillModels/Edit/5
        /// <summary>
        /// A page to edit existing skills
        /// </summary>
        /// <param name="id">The id of the skill to be edited</param>
        public async Task<IActionResult> Edit(int? id)
        {
            // checks if the user is loged in and an admin
            if (User.Identity.IsAuthenticated && _context.EmployeeModel.ToList().Find(x => x.EMail == User.Identity.Name).Admin)
            {
                // checks if the id is set
                if (id == null)
                {
                    return NotFound();
                }

                // Gets the actual skill if it exists
                var skillModel = await _context.Skill.FindAsync(id);
                if (skillModel == null)
                {
                    return NotFound();
                }
                return View(skillModel);
            }
            else
            {
                return Forbid();
            }
        }

        // POST: SkillModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Changes the skill to <paramref name="skillModel"/>
        /// </summary>
        /// <param name="id">the id of the skill to be updated</param>
        /// <param name="skillModel">The new data of the skill</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SkillID,Name")] Skill skillModel)
        {
            // checks if the user is loged in and an admin
            if (User.Identity.IsAuthenticated && _context.EmployeeModel.ToList().Find(x => x.EMail == User.Identity.Name).Admin)
            {
                // checks if the id and the id of skillModel are the same
                if (id != skillModel.SkillID)
                {
                    return NotFound();
                }

                // checks if the model is able to be added to the database
                if (ModelState.IsValid)
                {
                    // tries to update the database
                    try
                    {
                        _context.Update(skillModel);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        // checks if the problem is caused by the db
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
            else
            {
                return Forbid();
            }
        }

        // GET: SkillModels/Delete/5
        /// <summary>
        /// A deletion confirmation page 
        /// </summary>
        /// <param name="id">The id of the skill to be deleted</param>
        public async Task<IActionResult> Delete(int? id)
        {
            // checks if the model is able to be added to the database
            if (User.Identity.IsAuthenticated && _context.EmployeeModel.ToList().Find(x => x.EMail == User.Identity.Name).Admin)
            {
                // checks if is set
                if (id == null)
                {
                    return NotFound();
                }

                // Finds the skill with the id given
                var skillModel = await _context.Skill
                    .FirstOrDefaultAsync(m => m.SkillID == id);
                if (skillModel == null)
                {
                    return NotFound();
                }

                return View(skillModel);
            }
            else
            {
                return Forbid();
            }
        }

        // POST: SkillModels/Delete/5
        /// <summary>
        /// Deletes the skill with the id <paramref name="id"/>
        /// </summary>
        /// <param name="id">The id of the skill to be deleted</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // checks if the model is able to be added to the database
            if (User.Identity.IsAuthenticated && _context.EmployeeModel.ToList().Find(x => x.EMail == User.Identity.Name).Admin)
            {
                // finds the actual model
                var skillModel = await _context.Skill.FindAsync(id);
                // deletes the model
                _context.Skill.Remove(skillModel);
                // and saves the changes
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Checks if the skills exists
        /// </summary>
        private bool SkillExists(int id) => _context.Skill.Any(e => e.SkillID == id);
    }
}
