using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Gride.Controllers
{
    public class FeedbackController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Report(string reportName)
        {
            if (reportName != null)
            {
                Console.WriteLine("Title");
                return View();
            }
            Console.WriteLine("dag");
            return View();
        }
    }
}