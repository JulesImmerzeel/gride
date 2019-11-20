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
            List<Shift> allShifts = _context.Shift.ToList();

            if (id == null)
            {
                id = schedule._weekNumber;
            }

            schedule.currentWeek = (int)id;
            schedule.setWeek((int)id);
            schedule.makeSchedule();
            schedule.setShifts(allShifts);
           
            return View(schedule);
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
