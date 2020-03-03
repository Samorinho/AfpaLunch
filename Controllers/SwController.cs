﻿using AfpaLunch.Models;
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

        public JsonResult AddMenu(int IdMenu, List<int> IdProduits, string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);
            Panier produitPaniers = null;

            if (sessionUtilisateur != null)
            {
                if (HttpContext.Application[idsession] != null)
                {
                    produitPaniers = (Panier)HttpContext.Application[idsession];
                }
                else
                {
                    produitPaniers = new Panier();
                }

                Menu menu = db.Menus.Find(IdMenu);

                if (menu != null)
                {
                    MenuPanier menuPanier = new MenuPanier();
                    menuPanier.IdMenu = IdMenu;

                    foreach (int IdProduit in IdProduits)
                    {
                        ProduitPanier produitPanier = FindProduit(IdProduit);
                        
                        if (produitPanier != null)
                        {
                            menuPanier.produits.Add(produitPanier);
                        }                        
                    }

                    produitPaniers.Add(menuPanier);
                }

                HttpContext.Application[idsession] = produitPaniers;
            }

            return Json(produitPaniers.Quantite, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddProduit(int IdProduit, string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);
            Panier produitPaniers = null;

            if (sessionUtilisateur != null)
            {
                if (HttpContext.Application[idsession] != null)
                {
                    produitPaniers = (Panier)HttpContext.Application[idsession];
                }
                else
                {
                    produitPaniers = new Panier();
                }

                if (IdProduit > 0)
                {
                    ProduitPanier produitPanier = FindProduit(IdProduit);

                    if (produitPanier != null)
                    {
                        produitPaniers.Add(produitPanier);
                    }
                }

                HttpContext.Application[idsession] = produitPaniers;
            }

            return Json(produitPaniers.Quantite, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveProduit(int IdProduit, string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);

            Panier produitPaniers = (Panier)HttpContext.Application[idsession];

            ProduitPanier produitPanier = new ProduitPanier();

            int quantite = 0;

            var prod = produitPaniers.Find(p => p.GetIdProduit() == IdProduit);
            if (prod != null && produitPaniers.Count > 1)
            {
                prod.Quantite--;
                quantite -= prod.Quantite;
            }
            else if (prod != null && produitPaniers.Count > 0)
            {
                produitPaniers.Remove(produitPanier);
                quantite -= produitPanier.Quantite;
            }

            HttpContext.Application[idsession] = produitPaniers;

            return Json(quantite, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProduits(string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);
            List<ProduitPanier> panier = null;

            if (sessionUtilisateur != null && HttpContext.Application[idsession] != null)
            {
                panier = (List<ProduitPanier>)HttpContext.Application[idsession];
            }

            return Json(panier, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveCommande(string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);
            Panier panier = null;

            if (sessionUtilisateur != null && HttpContext.Application[idsession] != null)
            {
                panier = (Panier)HttpContext.Application[idsession];
            }

            try
            {
                Utilisateur utilisateur = db.Utilisateurs.First(p => p.IdSession == idsession);

                if (utilisateur != null && utilisateur.Solde > 0 && panier != null && panier.Count > 0)
                {
                    decimal prixTotal = 0;

                    panier.GetTotal();
                    prixTotal = panier.Total;

                    if (prixTotal <= utilisateur.Solde)
                    {
                        Commande commande = new Commande();
                        commande.IdUtilisateur = utilisateur.IdUtilisateur;
                        commande.IdRestaurant = panier.IdRestaurant;
                        commande.Date = DateTime.Now;
                        commande.Prix = prixTotal;
                        commande.IdEtatCommande = 1;

                        utilisateur.Solde -= prixTotal;

                        foreach (ItemPanier item in panier)
                        {
                            if (item is ProduitPanier)
                            {
                                CommandeProduit commandeProduit = new CommandeProduit();
                                commandeProduit.IdProduit = item.GetIdProduit();
                                commandeProduit.Prix = item.Prix;
                                commandeProduit.Quantite = item.Quantite;

                                commande.CommandeProduits.Add(commandeProduit);
                            }

                            else if (item is MenuPanier menuPanier)
                            {
                                List<ProduitPanier> produitPaniers = menuPanier.produits;
                                Menu menu = db.Menus.Find(item.GetIdMenu());

                                foreach ( ProduitPanier produitPanier in produitPaniers)
                                {
                                    CommandeProduit commandeProduit = new CommandeProduit();
                                    commandeProduit.IdProduit = item.GetIdProduit();
                                    commandeProduit.Prix = item.Prix;
                                    commandeProduit.Quantite = item.Quantite;
                                    commandeProduit.Menus.Add(menu);
                                    //commandeProduit.Menus.FirstOrDefault().IdMenu = item.GetIdMenu();

                                    commande.CommandeProduits.Add(commandeProduit);
                                }
                            }

                            else if (item is ProduitComposePanier produitComposePanier)
                            {
                                List<ProduitPanier> produitPaniers = produitComposePanier.produits;
                                foreach (ProduitPanier produitPanier in produitPaniers)
                                {
                                    CommandeProduit commandeProduit = new CommandeProduit();
                                    commandeProduit.IdProduit = item.GetIdProduit();
                                    commandeProduit.Prix = item.Prix;
                                    commandeProduit.Quantite = item.Quantite;

                                    commande.CommandeProduits.Add(commandeProduit);
                                }
                            }

                        }

                        db.Commandes.Add(commande);
                        db.SaveChanges();

                        HttpContext.Application.Clear();
                    }
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

        public JsonResult Hasard(int idfood, int idboisson, string idsession)
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

                var prod = produitPaniers.Find(p => p.IdProduit == idfood || p.IdProduit == idboisson);
                if (prod != null)
                {
                    prod.Quantite++;
                    quantite += prod.Quantite;
                    HttpContext.Application[idsession] = produitPaniers;
                    return Json(quantite, JsonRequestBehavior.AllowGet);
                }

                Produit produit = db.Produits.Find(idfood, idboisson);
                foreach (ProduitPanier item in produitPaniers)
                {
                    //item.IdProduit = idfood || item.IdProduit = idboisson;
                }
                ProduitPanier produitPanier = new ProduitPanier();
                produitPanier.IdProduit = idfood;
                produitPanier.Nom = produit.Nom;
                produitPanier.Description = produit.Description;
                produitPanier.Quantite = 1;
                produitPanier.Prix = produit.Prix;
                produitPanier.Photo = produit.Photos.First().Nom;
                produitPanier.IdRestaurant = produit.IdRestaurant;
                produitPaniers.Add(produitPanier);

                quantite++;

                HttpContext.Application[idsession] = produitPaniers;
            }
            var rnd = new Random();

            var first = db.Produits.Where(p => p.Categorie.IdCategorie == 3 || p.Categorie.IdCategorie == 5 || p.Categorie.IdCategorie == 7 || p.Categorie.IdCategorie == 8 || p.Categorie.IdCategorie == 9).OrderBy(p => rnd.Next());
            var second = db.Produits.Where(p => p.IdCategorie == 2).OrderBy(p => rnd.Next());

            return Json(quantite, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRestos(int? IdTypeCuisine, string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);

            if (sessionUtilisateur != null)
            {
                var resto = db.Restaurants.Where(r => r.IdTypeCuisine == IdTypeCuisine).Select(r => new
                {
                    IdRestaurant = r.IdRestaurant

                }).ToList();

                return Json(resto, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult Recherche(string search)
        {
            List<Resto> marecherche = null;
            //List<Restaurant> requete = null;
            try
            {
                marecherche = db.Restaurants.Where(r => r.Nom.Contains(search) || r.Tag.Contains(search)).Select(r => new Resto()
                {
                    IdRestaurant = r.IdRestaurant,
                    Nom = r.Nom
                }).ToList();
            }

            catch (Exception ex)
            {
                string er = ex.Message;
            }

            return Json(marecherche, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveFavoris(int idrestaurant, string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);
            Utilisateur utilisateur = (Utilisateur)Session["Utilisateur"];

            if (sessionUtilisateur != null && sessionUtilisateur.IdSession == idsession)
            {
                Restaurant restaurant = db.Restaurants.Find(idrestaurant);
                db.Utilisateurs.Where(r => r.Restaurants.FirstOrDefault().Utilisateurs.FirstOrDefault().IdUtilisateur == utilisateur.IdUtilisateur);
                //db.Utilisateurs.Remove(utilisateur);
                //db.SaveChanges();
            }

            return Json(idrestaurant, JsonRequestBehavior.AllowGet);
        }

        private ProduitPanier FindProduit(int IdProduit)
        {
            Produit produit = db.Produits.Find(IdProduit);
            ProduitPanier produitPanier = null;

            if (produitPanier != null)
            {
                produitPanier = new ProduitPanier();
                produitPanier.IdProduit = IdProduit;
                produitPanier.Nom = produit.Nom;
                produitPanier.Description = produit.Description;
                produitPanier.Quantite = 1;
                produitPanier.Prix = produit.Prix;
                produitPanier.Photo = produit.Photos.First().Nom;
                produitPanier.IdRestaurant = produit.IdRestaurant;
            }
            
            return produitPanier;
        }
    }
}