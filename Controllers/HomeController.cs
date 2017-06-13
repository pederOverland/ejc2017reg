using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ecreg.Data;
using ecreg.Models;
using ImageSharp;
using ImageSharp.Processing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ecreg.Controllers
{
    public class HomeController : Controller
    {
        private readonly EcRegDb _db;
        private static ResizeOptions _resizeOptions = new ResizeOptions { Size = new Size(600, 600), Mode = ResizeMode.Max };
        public HomeController(EcRegDb db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [Authorize]
        public IActionResult Participants()
        {
            var nation = _getNation(User);
            ViewData["nation"] = nation;
            ViewData["contestants"] = _getContestants(nation).ToList();
            return View(null);
        }


        private IEnumerable<Contestant> _getContestants(string nation){
            return nation == "admin" ? _db.Contestants : _db.Contestants.Where(x=>x.Nation==nation);
        }

        private string _getNation(ClaimsPrincipal user)
        {
            var cString = user.Claims.FirstOrDefault(x => x.Type.Equals("user_metadata"))?.Value;
            var data = JsonConvert.DeserializeAnonymousType(cString, new { Nation = "" });
            return data.Nation;
        }

        [HttpPost]
        public IActionResult Upload(Contestant c)
        {
            c.Modified = DateTime.Now;
            if (c.ContestantId != 0)
            {
                _db.Update(c);
            }
            else
            {
                _db.Add(c);
            }
            _db.SaveChanges();
            if (c.Picture != null)
            {
                using (var image = Image.Load(c.Picture.OpenReadStream()))
                {
                    var outName = @"wwwroot/profiles/" + c.ContestantId + "_" + c.Nation + ".jpg";
                    image.MetaData.Quality = 75;
                    image.Resize(_resizeOptions).AutoOrient().Save(outName);
                }
            }
            return RedirectToAction("Participants");
        }

        [HttpPost]
        public IActionResult Delete(Contestant c)
        {
            _db.Remove(c);
            _db.SaveChanges();
            return RedirectToAction("Participants");
        }

        [HttpPost]
        public IActionResult Edit(Contestant c)
        {
            var nation = _getNation(User);
            ViewData["nation"] = nation;
            var contestants = _getContestants(nation).ToList();
            var contestant = contestants.FirstOrDefault(x => x.ContestantId == c.ContestantId);
            ViewData["contestants"] = contestants.Where(x => x.ContestantId != contestant.ContestantId).ToList();
            return View("Participants", contestant);
        }
        public IActionResult Error()
        {
            return View();
        }
    }
}
