using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gride.Models;
using System.Web;
using Microsoft.EntityFrameworkCore;
using Gride.Data;

namespace Gride.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;
        public int x = 0;
        public Schedule schedule = new Schedule();

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                EmployeeModel employee = _context.EmployeeModel.Where(e => e.EMail == User.Identity.Name).Single();
                List<Shift> allShifts = new List<Shift>();

                ICollection<Work> works = _context.Works
                   .Where(e => e.Employee == employee)
                   .Include(e => e.Employee)
                   .Include(e => e.Shift)
                   .AsNoTracking()
                   .ToList();

                List<Shift> shifts = new List<Shift>();

                foreach (Work w in works)
                {
                    allShifts.Add(w.Shift);
                }

                if (id == null)
                {
                    id = schedule._weekNumber;
                }

                schedule.currentWeek = (int)id;
                schedule.setWeek((int)id);
                schedule.makeSchedule();
                schedule.setShifts(allShifts);

                var workOverviewlist = new List<WorkOverview>();

                for (int i = 1; i <= 12; i++)
                {
                    var workOverview = new WorkOverview
                    {
                        Month = i
                    };


                    foreach (Work w in works)
                    {
                        if (w.Shift.Start.Year == 2019 && w.Shift.Start.Month == i)
                        {
                            workOverview.AddHours((int)(w.Shift.End - w.Shift.Start).TotalHours);
                            workOverview.SubtractHours(w.Delay);
                            workOverview.AddHours(w.Overtime);
                        }
                    }

                    workOverviewlist.Add(workOverview);
                }

                ViewData["workOverview"] = workOverviewlist;

                return View(schedule);
            } else
            {
                return Redirect("/Identity/Account/Login");
            }
        }
       
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}
