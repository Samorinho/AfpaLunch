using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AfpaLunch;

namespace AfpaLunch.Views
{
    public class UtilisateursController : Controller
    {
        private AfpEATEntities db = new AfpEATEntities();

        // GET: Utilisateurs
        public ActionResult Index()
        {
            return View(db.Utilisateurs.ToList());
        }

        public ActionResult Connexion()
        {
            Utilisateur utilisateur = new Utilisateur();
            utilisateur = (Utilisateur)Session["Utilisateur"];
            if (utilisateur != null)
            {
                return RedirectToAction("Index", "Restaurants"); 
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Connexion([Bind(Include ="Matricule, Password")] Utilisateur utilisateur)
        {
            if (ModelState.IsValid)
            {
                Utilisateur user = db.Utilisateurs.FirstOrDefault(u => u.Matricule == utilisateur.Matricule && u.Password == utilisateur.Password);
                if (user != null)
                {
                    user.IdSession = Session.SessionID;
                    db.SaveChanges();

                    Session["Utilisateur"] = user;

                    return RedirectToAction("Index", "Restaurants");
                }
            }
         
            return View();       
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Details(FormCollection values)
        {
            Utilisateur utilisateur = new Utilisateur();

            if (ModelState.IsValid)
            {
                utilisateur = (Utilisateur)Session["Utilisateur"];
              
                if (utilisateur != null && utilisateur.Password == Convert.ToString(values["OldPassword"]))
                {
                    utilisateur.Password = Convert.ToString(values["MotdePasse"]);
                    
                    if (utilisateur.Password == Convert.ToString(values["PasswordBis"]))
                    {
                        db.Entry(utilisateur).State = EntityState.Modified;
                        db.SaveChanges();
                    }                  
                }
            }

            return View(utilisateur);
        }

        public ActionResult Historique()
        {
            Utilisateur utilisateur = new Utilisateur();
            utilisateur = (Utilisateur)Session["Utilisateur"];

            if (utilisateur != null)
            {
                List<string> history = new List<string>();

                foreach (Commande item in db.Commandes)
                {
                    var back = db.Commandes.Where(c => c.IdUtilisateur == utilisateur.IdUtilisateur).OrderBy(c => c.Date).ToList();

                    //if (back != null && back.Count > 0)
                    //{
                    //    ViewData[item.IdCommande.ToString()] = back;
                    //    history.Add(item.IdCommande.ToString());
                    //}
                }

                ViewBag.Histoires = history;
                ViewBag.Histoire = db.Commandes.Where(c => c.IdUtilisateur == utilisateur.IdUtilisateur).ToList();
            }
            

            return View();
        }
        public ActionResult Deconnexion()
        {
            Session.Clear();
            return RedirectToAction("Index", "Restaurants");
        }
        // GET: Utilisateurs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Utilisateur utilisateur = db.Utilisateurs.Find(id);
            if (utilisateur == null)
            {
                return HttpNotFound();
            }
            return View(utilisateur);
        }

        // GET: Utilisateurs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Utilisateurs/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdUtilisateur,Nom,Prenom,Matricule,Password,Statut,Solde")] Utilisateur utilisateur)
        {
            if (ModelState.IsValid)
            {
                db.Utilisateurs.Add(utilisateur);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(utilisateur);
        }
        public ActionResult ValidationCommande(int? id)
        {
            Utilisateur utilisateur = db.Utilisateurs.Find(id);
            Session["Utilisateur"] = utilisateur;
            return RedirectToAction("Index");
        }

        public ActionResult RecapCommande()
        {
            return View();
        }
        // GET: Utilisateurs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Utilisateur utilisateur = db.Utilisateurs.Find(id);
            if (utilisateur == null)
            {
                return HttpNotFound();
            }
            return View(utilisateur);
        }

        // POST: Utilisateurs/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdUtilisateur,Nom,Prenom,Matricule,Password,Statut,Solde")] Utilisateur utilisateur)
        {
            if (ModelState.IsValid)
            {
                db.Entry(utilisateur).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(utilisateur);
        }

        // GET: Utilisateurs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Utilisateur utilisateur = db.Utilisateurs.Find(id);
            if (utilisateur == null)
            {
                return HttpNotFound();
            }
            return View(utilisateur);
        }

        // POST: Utilisateurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Utilisateur utilisateur = db.Utilisateurs.Find(id);
            db.Utilisateurs.Remove(utilisateur);
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
