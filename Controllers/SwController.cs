using AfpaLunch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AfpaLunch.Controllers
{
    public class SwController : Controller
    {
        private AfpEATEntities db = new AfpEATEntities();
        // GET: Sw
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult AddMenu(int IdMenu, string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);
            List<MenuPanier> menuPaniers = null;
            List<ProduitPanier> produitPaniers = null;
            if (sessionUtilisateur != null)
            {
                if (HttpContext.Application[idsession] != null)
                {
                    menuPaniers = (List<MenuPanier>)HttpContext.Application[idsession];
                }
                else
                {
                    menuPaniers = new List<MenuPanier>();
                }

                Menu menu = db.Menus.Find(IdMenu);

                MenuPanier menuPanier = new MenuPanier();
                menuPanier.IdMenu = menu.IdMenu;
                menuPanier.produits = db.Produits.Where(p => p.IdCategorie == p.Categorie.Menus.FirstOrDefault().Categories.FirstOrDefault().IdCategorie).ToList();
                menuPanier.Prix = menu.Prix;
                menuPanier.Quantite = 1;
                menuPaniers.Add(menuPanier);
                foreach (ProduitPanier item in produitPaniers)
                {                   
                    Produit produit = new Produit();
                    ProduitPanier produitPanier = new ProduitPanier();
                    //produitPanier.IdProduit = Convert.ToInt32(values["Bouffe"]);
                    produitPanier.Nom = produit.Nom;
                    produitPanier.Description = produit.Description;
                    produitPanier.Quantite = 1;
                    produitPanier.Prix = produit.Prix;
                    produitPanier.Photo = produit.Photos.First().Nom;
                    produitPanier.IdRestaurant = produit.IdRestaurant;
                    produitPaniers.Add(produitPanier);
                }

                HttpContext.Application[idsession] = menuPaniers;
            }
            return Json(menuPaniers.Count, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddProduit(int IdProduit, string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);
            List<ProduitPanier> produitPaniers = null;
            int quantite = 0;
            if (sessionUtilisateur != null)
            {
                if (HttpContext.Application[idsession] != null)
                {
                    produitPaniers = (List<ProduitPanier>)HttpContext.Application[idsession];
                }
                else
                {
                    produitPaniers = new List<ProduitPanier>();
                }

                if (produitPaniers.Count > 0)
                {
                    foreach (ProduitPanier item in produitPaniers)
                    {
                        if (item.IdProduit == IdProduit)
                        {
                            item.Quantite++;
                            quantite += item.Quantite;
                        }
                        else
                        {
                            Produit produit = db.Produits.Find(IdProduit);
                            ProduitPanier produitPanier = new ProduitPanier();
                            produitPanier.IdProduit = IdProduit;
                            produitPanier.Nom = produit.Nom;
                            produitPanier.Description = produit.Description;
                            produitPanier.Quantite = 1;
                            produitPanier.Prix = produit.Prix;
                            produitPanier.Photo = produit.Photos.First().Nom;
                            produitPanier.IdRestaurant = produit.IdRestaurant;
                            produitPaniers.Add(produitPanier);

                            quantite++;
                        }


                    }
                }

                else
                {
                    Produit produit = db.Produits.Find(IdProduit);
                    ProduitPanier produitPanier = new ProduitPanier();
                    produitPanier.IdProduit = IdProduit;
                    produitPanier.Nom = produit.Nom;
                    produitPanier.Description = produit.Description;
                    produitPanier.Quantite = 1;
                    produitPanier.Prix = produit.Prix;
                    produitPanier.Photo = produit.Photos.First().Nom;
                    produitPanier.IdRestaurant = produit.IdRestaurant;
                    produitPaniers.Add(produitPanier);

                    quantite++;
                }

                HttpContext.Application[idsession] = produitPaniers;
            }
            return Json(quantite, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveProduit(int IdProduit, string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);

            List<ProduitPanier> produitPaniers = (List<ProduitPanier>)HttpContext.Application[idsession];

            foreach (ProduitPanier item in produitPaniers)
            {
                if (item.IdProduit == IdProduit)
                {
                    produitPaniers.Remove(item);
                }
            }

            HttpContext.Application[idsession] = produitPaniers;

            return Json(produitPaniers.Count, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddQuantite(int IdProduit, string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);

            List<ProduitPanier> produitPaniers = (List<ProduitPanier>)HttpContext.Application[idsession];

            foreach (ProduitPanier item in produitPaniers)
            {
                if (item.IdProduit == IdProduit)
                {
                    item.Quantite++;
                }
            }

            HttpContext.Application[idsession] = produitPaniers;

            return Json(produitPaniers.Count, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveQuantite(int IdProduit, string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);

            List<ProduitPanier> produitPaniers = (List<ProduitPanier>)HttpContext.Application[idsession];

            foreach (ProduitPanier item in produitPaniers)
            {
                if (item.IdProduit == IdProduit)
                {
                    item.Quantite--;
                }
            }

            HttpContext.Application[idsession] = produitPaniers;

            return Json(produitPaniers.Count, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetProduits(string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);
            List<ProduitPanier> panier = null;

            if (sessionUtilisateur != null)
            {
                if (HttpContext.Application[idsession] != null)
                {
                    panier = (List<ProduitPanier>)HttpContext.Application[idsession];
                }
            }
            return Json(panier, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveCommande(string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);
            List<ProduitPanier> panier = null;
            int idrestaurant = 0;

            if (sessionUtilisateur != null)
            {
                if (HttpContext.Application[idsession] != null)
                {
                    panier = (List<ProduitPanier>)HttpContext.Application[idsession];
                }
            }
            try
            {
                Utilisateur utilisateur = db.Utilisateurs.FirstOrDefault(p => p.IdSession == idsession);

                if (utilisateur != null && utilisateur.Solde > 0 && panier != null && panier.Count > 0)
                {
                    decimal prixTotal = 0;

                    foreach (ProduitPanier produitPanier in panier)
                    {
                        prixTotal += produitPanier.Prix * produitPanier.Quantite;
                        idrestaurant = produitPanier.IdRestaurant;
                    }

                    if (prixTotal <= utilisateur.Solde)
                    {
                        Commande commande = new Commande();
                        commande.IdUtilisateur = utilisateur.IdUtilisateur;
                        commande.IdRestaurant = idrestaurant;
                        commande.Date = DateTime.Now;
                        commande.Prix = prixTotal;
                        commande.IdEtatCommande = 1;
                        //db.Commandes.Add(commande);
                        //db.SaveChanges();
                        utilisateur.Solde -= prixTotal;

                        foreach (ProduitPanier produitPanier in panier)
                        {
                            CommandeProduit commandeProduit = new CommandeProduit();
                            //commandeProduit.IdCommande = commande.IdCommande;
                            commandeProduit.IdProduit = produitPanier.IdProduit;
                            commandeProduit.Prix = produitPanier.Prix;
                            commandeProduit.Quantite = produitPanier.Quantite;

                            commande.CommandeProduits.Add(commandeProduit);

                            try
                            {
                                Produit produit = db.Produits.Find(commandeProduit.IdProduit);

                                if (produit.IdProduit == commandeProduit.IdProduit)
                                {
                                    produit.Quantite -= commandeProduit.Quantite;
                                }
                            }
                            catch (Exception ex)
                            {
                                string er = ex.Message;
                            }
                        }

                        db.Commandes.Add(commande);
                        db.SaveChanges();

                        HttpContext.Application.Clear();
                    }
                }
                else
                {
                    string ex = "Vous devez vous connecter pour effectuer votre commande";
                    return Json(ex, JsonRequestBehavior.AllowGet);
                }
                return Json(new { idutilisateur = utilisateur.IdUtilisateur }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string er = ex.Message;
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoginUtilisateur(string idsession, string matricule, string password)
        {
            Utilisateur utilisateur = db.Utilisateurs.FirstOrDefault(u => u.Matricule == matricule && u.Password == password);

            if (utilisateur != null)
            {
                utilisateur.IdSession = idsession;
                db.SaveChanges();

                return Json(new { error = 0, message = "Vous disposez de " + utilisateur.Solde + "€" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = 1, message = "Vous n'êtes pas connecté(e)" }, JsonRequestBehavior.AllowGet);
        }

    }
}