using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gride.Data;
using Gride.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gride.Controllers
{
    [Authorize]
    public class RosterController : Controller
    {
        private readonly ApplicationDbContext _context;
        public Schedule schedule = new Schedule();

        public RosterController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? id)
        {
            List<Shift> allShifts = _context.Shift.ToList();

            EmployeeModel employee = _context.EmployeeModel
                .Single(e => e.EMail == User.Identity.Name);

            if (id == null)
            {
                id = schedule._weekNumber;
            }

            List<Work> works = _context.Works.Where(e => e.Employee == employee).Include(m => m.Employee).Include(s => s.Shift).ToList();
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

            schedule.currentWeek = (int)id;
            schedule.setWeek((int)id);
            schedule.makeSchedule();
            schedule.setShifts(allShifts);

            ViewData["workOverview"] = workOverviewlist;

            return View(schedule);
        }
    }
}