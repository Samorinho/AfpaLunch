using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AfpaLunch;
using AfpaLunch.Models;

namespace AfpaLunch.Controllers
{
    public class CommandesController : Controller
    {
        private AfpEATEntities db = new AfpEATEntities();

        // GET: Commandes
        public ActionResult Index()
        {
            var commandes = db.Commandes.Include(c => c.EtatCommande).Include(c => c.Restaurant).Include(c => c.Utilisateur);
            return View(commandes.ToList());
        }

        // GET: Commandes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Commande commande = db.Commandes.Find(id);
            if (commande == null)
            {
                return HttpNotFound();
            }
            return View(commande);
        }

        // GET: Commandes/Create
        public ActionResult Create()
        {
            ViewBag.IdEtatCommande = new SelectList(db.EtatCommandes, "IdEtatCommande", "Nom");
            ViewBag.IdRestaurant = new SelectList(db.Restaurants, "IdRestaurant", "Description");
            ViewBag.IdUtilisateur = new SelectList(db.Utilisateurs, "IdUtilisateur", "Nom");
            return View();
        }

        // POST: Commandes/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdCommande,IdUtilisateur,IdRestaurant,IdEtatCommande,Date,Prix")] Commande commande)
        {
            if (Session["Utilisateur"] != null)
            {
                if (Session["Panier"] != null)
                {
                    try
                    {
                        Utilisateur utilisateur = (Utilisateur)Session["Utilisateur"];
                        List<Basket> baskets = (List<Basket>)Session["Panier"];
                        int IdUtilisateur = utilisateur.IdUtilisateur;
                        DateTime Date = DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        string err = ex.Message;
                    }
                }
            }
            else
            {
                return RedirectToAction("Connexion");
            }
            if (ModelState.IsValid)
            {
                db.Commandes.Add(commande);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdEtatCommande = new SelectList(db.EtatCommandes, "IdEtatCommande", "Nom", commande.IdEtatCommande);
            ViewBag.IdRestaurant = new SelectList(db.Restaurants, "IdRestaurant", "Description", commande.IdRestaurant);
            ViewBag.IdUtilisateur = new SelectList(db.Utilisateurs, "IdUtilisateur", "Nom", commande.IdUtilisateur);
            return View(commande);
        }

        // GET: Commandes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Commande commande = db.Commandes.Find(id);
            if (commande == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdEtatCommande = new SelectList(db.EtatCommandes, "IdEtatCommande", "Nom", commande.IdEtatCommande);
            ViewBag.IdRestaurant = new SelectList(db.Restaurants, "IdRestaurant", "Description", commande.IdRestaurant);
            ViewBag.IdUtilisateur = new SelectList(db.Utilisateurs, "IdUtilisateur", "Nom", commande.IdUtilisateur);
            return View(commande);
        }

        // POST: Commandes/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdCommande,IdUtilisateur,IdRestaurant,IdEtatCommande,Date,Prix")] Commande commande)
        {
            if (ModelState.IsValid)
            {
                db.Entry(commande).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdEtatCommande = new SelectList(db.EtatCommandes, "IdEtatCommande", "Nom", commande.IdEtatCommande);
            ViewBag.IdRestaurant = new SelectList(db.Restaurants, "IdRestaurant", "Description", commande.IdRestaurant);
            ViewBag.IdUtilisateur = new SelectList(db.Utilisateurs, "IdUtilisateur", "Nom", commande.IdUtilisateur);
            return View(commande);
        }

        // GET: Commandes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Commande commande = db.Commandes.Find(id);
            if (commande == null)
            {
                return HttpNotFound();
            }
            return View(commande);
        }

        // POST: Commandes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Commande commande = db.Commandes.Find(id);
            db.Commandes.Remove(commande);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
