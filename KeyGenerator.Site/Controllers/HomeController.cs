using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KeyGenerator.Site.Models;
using KeyGenerator.Core.Parsers.Interfaces;

namespace KeyGenerator.Site.Controllers
{
    public class HomeController : Controller
    {
        private IKeywordsService _keywordsService;

        public HomeController(IKeywordsService keywordsService)
        {
            _keywordsService = keywordsService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> _OpenProduct(string url)
        {
            var seeds =await  _keywordsService.ExctractKeywordsForProductPageAsync(url);
            return Json(seeds);
        }

        [HttpPost]
        public async Task<IActionResult> _GenerateKeywords(string[] seeds)
        {
            var keywords = await _keywordsService.GetSuggestionsAsync(seeds);
            return Json(keywords);
        }
    }
}
