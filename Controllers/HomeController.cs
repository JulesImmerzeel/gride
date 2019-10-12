using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gride.Models;

namespace Gride.Controllers
{
    public class HomeController : Controller
    {
        public int x = 0;
        public Schedule schedule = new Schedule();
        public IActionResult Index(int id)
        {
            int k = id;
            if (k == 1)
            {
                schedule.y++;
            }
            if (k == -1)
            {
                schedule.y--;
            }
        
            schedule.setWeek(schedule.y);
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
