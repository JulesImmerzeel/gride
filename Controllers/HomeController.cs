using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gride.Models;
using System.Web;

namespace Gride.Controllers
{
    public class HomeController : Controller
    {
        public int x = 0;
        public Schedule schedule = new Schedule();

        public IActionResult Index(int? id)
        {
            if (id == null)
            {
                id = schedule.weekNumber;
            }

            schedule.showingWeekNumber = (int)id;
            schedule.setWeek(schedule.showingWeekNumber);
            schedule.setShifts();
           
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
