using AfpaLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AfpaLunch.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Panier()
        {
            Panier panier = (Panier)HttpContext.Application[Session.SessionID];

            return View(panier);
        }

        public ActionResult PanierAjax()
        {
            return View();
        }
    }
}