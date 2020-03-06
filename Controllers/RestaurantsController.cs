using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AfpaLunch;
using AfpaLunch.Models;

namespace AfpaLunch.Controllers
{
    public class RestaurantsController : Controller
    {
        private AfpEATEntities db = new AfpEATEntities();

        // GET: Restaurants
        public ActionResult Index(/*string searchString*/)
        {
            var restaurants = db.Restaurants.Include(r => r.TypeCuisine);
            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    restaurants = db.Restaurants.Where(r => r.Nom.Contains(searchString) || r.TypeCuisine.Nom.Contains(searchString) || r.Produits.FirstOrDefault().Nom.Contains(searchString));
            //}
            ViewBag.TypeCuisine = db.TypeCuisines.OrderBy(t => t.Nom).ToList();
            ViewBag.Restaurants = db.Restaurants.Include(r => r.TypeCuisine).ToList();
            return View(restaurants.ToList());
        }

        // GET: Restaurants/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Bouffe = new SelectList(db.Produits.Where(p => p.IdRestaurant == id && p.IdCategorie == 2 || p.IdCategorie == 7 || p.IdCategorie == 8 || p.IdCategorie == 9).ToList(), "IdProduit", "Nom");
            ViewBag.Boisson = new SelectList(db.Produits.Where(p => p.IdRestaurant == id && p.IdCategorie == 6).ToList(), "IdProduit", "Nom");
            ViewBag.Dessert = new SelectList(db.Produits.Where(p => p.IdRestaurant == id && p.IdCategorie == 4).ToList(), "IdProduit", "Nom");

            List<string> nomcategorie = new List<string>();
            Restaurant restaurant = db.Restaurants.Include(r => r.Produits).Include(r => r.Menus).Where(r => r.IdRestaurant == id).First();

            foreach (Categorie item in db.Categories)
            {
                var produits = db.Produits.Where(p => p.IdCategorie == item.IdCategorie && p.IdRestaurant == id).ToList();
                
                
                if (produits != null && produits.Count > 0)
                {
                    TempData[item.Nom] = produits;
                    nomcategorie.Add(item.Nom);
                }
            }

            ViewBag.NomsCategories = nomcategorie;

            if (restaurant == null)
            {
                return HttpNotFound();
            }
            return View(restaurant);
        }

        public ActionResult DetailsResto(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Restaurant restaurant = db.Restaurants.Include(r => r.Produits).Include(r => r.Menus).Where(r => r.IdRestaurant == id).First();

            if (restaurant == null)
            {
                return HttpNotFound();
            }

            //Nouvelle liste de string

            List<string> nomscategories = new List<string>();

            // On parcourt les catégories du restaurant en question

            foreach (Categorie item in db.Categories)
            {
                var produits = db.Produits.Where(p => p.IdCategorie == item.IdCategorie && p.IdRestaurant == id).ToList();


                if (produits != null && produits.Count > 0)
                {
                    // On ajoute la liste des produits au "ViewData"

                    ViewData[item.Nom] = produits;
                    nomscategories.Add(item.Nom);
                }
            }

            ViewBag.NomsCategories = nomscategories;

            
            return View(restaurant);
        }

        [HttpPost]
        public ActionResult Upload()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        string path = AppDomain.CurrentDomain.BaseDirectory + "Images/";  
                        string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/Images/"), fname);
                        file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  
                    return Json("Votre image a bien été envoyée !");
                }
                catch (Exception ex)
                {
                    return Json("Une erreure s'est produite : " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        // GET: Restaurants/Create
        public ActionResult Create()
        {
            ViewBag.IdTypeCuisine = new SelectList(db.TypeCuisines, "IdTypeCuisine", "Nom");
            return View();
        }

        // POST: Restaurants/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdRestaurant,IdTypeCuisine,Description,Nom,Tag,Budget,Adresse,CodePostal,Ville,Telephone,Mobile,Email,Reponsable,Login,Password")] Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                db.Restaurants.Add(restaurant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdTypeCuisine = new SelectList(db.TypeCuisines, "IdTypeCuisine", "Nom", restaurant.IdTypeCuisine);
            return View(restaurant);
        }

        [HttpGet, ActionName("AddPanier")]

        public ActionResult AddPanier(int idrestaurant, int idproduit)
        {
            if (Session["Panier"] == null)
            {
                try
                {
                    List<Panier> baskets = new List<Panier>();

                    Produit produit = db.Produits.Include(p => p.Photos).Where(p => p.IdProduit == idproduit).First();

                    Panier basket  = new Panier();
                    baskets.Add(basket);
                    Session["Panier"] = baskets;
                }
                catch (Exception ex)
                {
                    string er = ex.Message;
                }
            }
            else
            {
                try
                {
                    List<Panier> baskets = (List<Panier>)Session["Panier"];

                    Produit produit = db.Produits.Include(p => p.Photos).Where(p => p.IdProduit == idproduit).First();
                    Panier basket = new Panier();
                    baskets.Add(basket);
                    Session["Panier"] = baskets;
                }
                catch (Exception ex)
                {
                    string er = ex.Message;
                }
            }
            return RedirectToAction("Details/" + idrestaurant);
        }

        // GET: Restaurants/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurant restaurant = db.Restaurants.Find(id);
            if (restaurant == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdTypeCuisine = new SelectList(db.TypeCuisines, "IdTypeCuisine", "Nom", restaurant.IdTypeCuisine);
            return View(restaurant);
        }

        // POST: Restaurants/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdRestaurant,IdTypeCuisine,Description,Nom,Tag,Budget,Adresse,CodePostal,Ville,Telephone,Mobile,Email,Reponsable,Login,Password")] Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(restaurant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdTypeCuisine = new SelectList(db.TypeCuisines, "IdTypeCuisine", "Nom", restaurant.IdTypeCuisine);
            return View(restaurant);
        }

        // GET: Restaurants/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurant restaurant = db.Restaurants.Find(id);
            if (restaurant == null)
            {
                return HttpNotFound();
            }
            return View(restaurant);
        }

        // POST: Restaurants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Restaurant restaurant = db.Restaurants.Find(id);
            db.Restaurants.Remove(restaurant);
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
