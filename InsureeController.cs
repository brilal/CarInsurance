using CarInsurance.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceContext db = new InsuranceContext();

        // GET: Insuree
        public ActionResult Index()
        {
            return View(db.Insurees.ToList());
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insuree/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                decimal quote = 50m; // Base monthly cost

                // Calculate age
                int age = DateTime.Now.Year - insuree.DateOfBirth.Year;
                if (insuree.DateOfBirth > DateTime.Now.AddYears(-age)) age--;

                // Age-based adjustments
                if (age <= 18)
                    quote += 100;
                else if (age >= 19 && age <= 25)
                    quote += 50;
                else
                    quote += 25;

                // Car year adjustments
                if (insuree.CarYear < 2000)
                    quote += 25;
                else if (insuree.CarYear > 2015)
                    quote += 25;

                // Car make/model adjustments
                if (insuree.CarMake.ToLower() == "porsche")
                {
                    quote += 25;
                    if (insuree.CarModel.ToLower() == "911 carrera")
                        quote += 25;
                }

                // Speeding tickets
                quote += insuree.SpeedingTickets * 10;

                // DUI adjustment (25%)
                if (insuree.DUI)
                    quote *= 1.25m;

                // Coverage type adjustment (50%)
                if (insuree.CoverageType.ToLower() == "full")
                    quote *= 1.50m;

                // Save final quote
                insuree.Quote = quote;

                db.Insurees.Add(insuree);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(insuree);
        }

        // ADMIN PAGE
        public ActionResult Admin()
        {
            return View(db.Insurees.ToList());
        }
    }
}
