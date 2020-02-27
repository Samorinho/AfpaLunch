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
    public class RestaurantsController : Controller
    {
        private AfpEATEntities db = new AfpEATEntities();

        // GET: Restaurants
        public ActionResult Index(string searchString)
        {
            var restaurants = db.Restaurants.Include(r => r.TypeCuisine);
            if (!String.IsNullOrEmpty(searchString))
            {
                restaurants = db.Restaurants.Where(r => r.Nom.Contains(searchString) || r.TypeCuisine.Nom.Contains(searchString) || r.Produits.FirstOrDefault().Nom.Contains(searchString));
            }
            ViewBag.TypeCuisine = db.Restaurants.ToList();
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

            Restaurant restaurant = db.Restaurants.Find(id);           

            if (restaurant == null)
            {
                return HttpNotFound();
            }
            return View(restaurant);
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
                    List<Basket> baskets = new List<Basket>();

                    Produit produit = db.Produits.Include(p => p.Photos).Where(p => p.IdProduit == idproduit).First();

                    Basket basket  = new Basket();
                    basket.monproduit = produit;
                    basket.Quantite++;
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
                    List<Basket> baskets = (List<Basket>)Session["Panier"];

                    Produit produit = db.Produits.Include(p => p.Photos).Where(p => p.IdProduit == idproduit).First();
                    Basket basket = new Basket();
                    basket.monproduit = produit;
                    basket.Quantite++;
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
