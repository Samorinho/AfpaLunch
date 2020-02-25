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

        public ActionResult Deconnexion()
        {
            Session.Clear();
            return RedirectToAction("Index");
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
            List<ProduitPanier> panier = (List<ProduitPanier>)HttpContext.Application[Session.SessionID];

            return View(panier);
        }

        public ActionResult PanierAjax()
        {
            return View();
        }
    }
}