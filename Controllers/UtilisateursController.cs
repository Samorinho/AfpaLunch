using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
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

        public PartialViewResult ModalHistorique(int? id)
        {
            Commande commande = db.Commandes.Find(id);
            ViewBag.Reco = commande;

            return PartialView(commande);
        }

        public ActionResult Connexion()
        {
            Utilisateur utilisateur = (Utilisateur)Session["Utilisateur"];
            if (utilisateur != null)
            {
                return RedirectToAction("Index", "Restaurants");
            }
            return View();
        }

        public string ChangePassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            password = Convert.ToBase64String(hashBytes);

            return password;
        }

        public bool ConnexionPassword(string passwordbdd, string passwordecran)
        {
            bool safeOk = true;

            // Extraction des bytes 
            byte[] hashBytes = Convert.FromBase64String(passwordbdd);

            // Salaison
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Hash sur le mot de passe tapé sur la page de connexion
            var pbkdf2 = new Rfc2898DeriveBytes(passwordecran, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Comparaison des résultats
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    // Erreur retournée si les résultats sont différents
                    safeOk = false;
                    throw new UnauthorizedAccessException();
                }
            }

            return safeOk;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Connexion([Bind(Include = "Matricule, Password")] Utilisateur utilisateur)
        {
            if (ModelState.IsValid)
            {
                Utilisateur user = db.Utilisateurs.First(u => u.Matricule == utilisateur.Matricule);

                byte[] hashBytes = Convert.FromBase64String(user.Password);

                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);

                var pbkdf2 = new Rfc2898DeriveBytes(utilisateur.Password, salt, 10000);
                byte[] hash = pbkdf2.GetBytes(20);

                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                    {
                        throw new UnauthorizedAccessException();
                    }
                }

                user.IdSession = Session.SessionID;

                Session["Utilisateur"] = user;

                db.SaveChanges();

                return RedirectToAction("Index", "Restaurants");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Details(FormCollection values)
        {           
            if (ModelState.IsValid)
            {
                Utilisateur utilisateur = (Utilisateur)Session["Utilisateur"];

                if (utilisateur != null && values["MotdePasse"] == values["PasswordBis"])
                {
                    byte[] salt;
                    new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

                    var pbkdf2 = new Rfc2898DeriveBytes(Convert.ToString(values["MotdePasse"]), salt, 10000);
                    byte[] hash = pbkdf2.GetBytes(20);

                    byte[] hashBytes = new byte[36];
                    Array.Copy(salt, 0, hashBytes, 0, 16);
                    Array.Copy(hash, 0, hashBytes, 16, 20);

                    utilisateur.Password = Convert.ToBase64String(hashBytes);

                    db.Entry(utilisateur).State = EntityState.Modified;
                    db.SaveChanges();

                    return View(utilisateur);
                }
            }

            return RedirectToAction("Connexion","Utilisateurs");
        }

        public ActionResult Favoris(int? id)
        {
            Utilisateur utilisateur = new Utilisateur();
            utilisateur = (Utilisateur)Session["Utilisateur"];

            if (utilisateur != null)
            {
                utilisateur = db.Utilisateurs.Find(id);
                //List<string> favoris = new List<string>();

                //foreach (Restaurant item in db.Restaurants)
                //{
                //    var fav = db.Restaurants.Where(r => r.Utilisateurs.FirstOrDefault().IdUtilisateur == id && r.IdRestaurant == item.IdRestaurant).ToList();

                //    if (fav != null && fav.Count > 0)
                //    {
                //        TempData[item.Nom] = fav;
                //        favoris.Add(item.Nom);
                //    }
                //}
                //ViewBag.Favourites = favoris;
                ViewBag.MesFavoris = db.Restaurants.Where(r => r.Utilisateurs.FirstOrDefault().IdUtilisateur == id).ToList();
            }

            return View();
        }

        // EN COURS 
        public ActionResult Historique()
        {
            Utilisateur utilisateur = new Utilisateur();
            utilisateur = (Utilisateur)Session["Utilisateur"];

            if (utilisateur != null)
            {
                //List<string> history = new List<string>();

                //foreach (Commande item in db.Commandes)
                //{
                //    var back = db.Commandes.Where(c => c.IdUtilisateur == utilisateur.IdUtilisateur).OrderByDescending(c => c.Date).ToList();

                //    if (back != null && back.Count > 0)
                //    {
                //        ViewData[item.IdCommande.ToString()] = back;
                //        history.Add(item.IdCommande.ToString());
                //    }
                //}

                //ViewBag.Histoires = history;
                //List<Commande> commandes = db.Commandes.Where(c => c.IdUtilisateur == utilisateur.IdUtilisateur).ToList();
                ViewBag.Histoire = db.Commandes.Where(c => c.IdUtilisateur == utilisateur.IdUtilisateur).OrderByDescending(c => c.IdCommande).ToList();

                //Commande commande = new Commande();
                //////////////////////////////////////////

                //var backup = db.Commandes.Where(c => c.IdUtilisateur == utilisateur.IdUtilisateur).OrderBy(c => c.Date).ToList();

                //var produits = db.Produits.Where(p => p.IdRestaurant == commande.IdRestaurant).ToList();

                //foreach (var historique in commandes)
                //{
                //    List<Produit> products = produits.Where(p => p.IdProduit == historique.Restaurant.Produits.FirstOrDefault().IdProduit).ToList();

                //    // On crée les items d'un select (dropdownlist)
                //    List<SelectListItem> items = new List<SelectListItem>();

                //    foreach (Produit produit in products)
                //    {
                //        items.Add(new SelectListItem { Text = produit.Nom, Value = produit.IdProduit.ToString() });
                //    }

                //    ViewData["produits" + historique.IdCommande] = items;
                //}
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
