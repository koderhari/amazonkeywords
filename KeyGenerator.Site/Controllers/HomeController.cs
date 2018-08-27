using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KeyGenerator.Site.Models;

namespace KeyGenerator.Site.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult _OpenProduct(string url)
        {
            var sees = new List<string>
            {
                "dsadsa",
                "sss",
                "wwww",
            };
            return Json(sees);
        }

        [HttpPost]
        public IActionResult _GenerateKeywords(string[] keywords)
        {
            return Json(keywords);
        }
    }
}
