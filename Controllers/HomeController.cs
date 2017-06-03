using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ecreg.Data;
using ecreg.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotoSauce.MagicScaler;

namespace ecreg.Controllers
{
    public class HomeController : Controller
    {
        private readonly EcRegDb _db;
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
            var contestants = _db.Contestants.Where(x => x.Nation == "Norway");
            return View(contestants);
        }

        [HttpPost]
        public IActionResult Upload(Contestant c)
        {
            c.Modified = DateTime.Now;
            if(c.ContestantId!=0){
                _db.Update(c);
            }else{
                _db.Add(c);
            }
            _db.SaveChanges();
            if (c.Picture != null)
            {
                using (var outStream = new FileStream(@"wwwroot/profiles/" + c.ContestantId + "_" + c.Nation + ".jpg", FileMode.Create))
                {
                    MagicImageProcessor.ProcessImage(c.Picture.OpenReadStream(), outStream, new ProcessImageSettings { Width = 400 });
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
            var contestants = _db.Contestants.Where(x => x.Nation == "Norway").ToList();
            var contestant = contestants.FirstOrDefault(x => x.ContestantId == c.ContestantId);
            ViewData["contestant"] = contestant;
            return View("Participants", contestants.Where(x => x.ContestantId != contestant.ContestantId));
        }
        public IActionResult Error()
        {
            return View();
        }
    }
}
