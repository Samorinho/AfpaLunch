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

        public JsonResult AddProduit(int IdProduit, string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);
            List<ProduitPanier> produitPaniers = null;
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

                Produit produit = db.Produits.Find(IdProduit);
                ProduitPanier produitPanier = new ProduitPanier();
                produitPanier.IdProduit = IdProduit;
                produitPanier.Nom = produit.Nom;
                produitPanier.Description = produit.Description;
                produitPanier.Quantite = 1;
                produitPanier.Prix = produit.Prix;
                produitPanier.Photo = produit.Photos.First().Nom;
                produitPanier.IdRestaurant = produit.IdRestaurant;

                foreach (ProduitPanier item in produitPaniers)
                {
                    if (item.IdProduit != null)
                    {
                        item.Quantite++;
                    }
                }
                produitPaniers.Add(produitPanier);

                HttpContext.Application[idsession] = produitPaniers;
            }
            return Json(produitPaniers.Count, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveProduit(int IdProduit, string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);

            List<ProduitPanier> produitPaniers = (List<ProduitPanier>)HttpContext.Application[idsession];

            Produit produit = db.Produits.Find(IdProduit);
            ProduitPanier produitPanier = new ProduitPanier();

            produitPaniers.Remove(produitPanier);

            HttpContext.Application[idsession] = produitPaniers;

            return Json(produitPaniers.Count, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddQuantite(int IdProduit, string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);

            List<ProduitPanier> produitPaniers = (List<ProduitPanier>)HttpContext.Application[idsession];

            Produit produit = db.Produits.Find(IdProduit);
            ProduitPanier produitPanier = new ProduitPanier();
            produitPanier.Quantite++;

            HttpContext.Application[idsession] = produitPaniers;

            return Json(produitPaniers.Count, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveQuantite(int IdProduit, string idsession)
        {
            SessionUtilisateur sessionUtilisateur = db.SessionUtilisateurs.Find(Session.SessionID);

            List<ProduitPanier> produitPaniers = (List<ProduitPanier>)HttpContext.Application[idsession];

            Produit produit = db.Produits.Find(IdProduit);
            ProduitPanier produitPanier = new ProduitPanier();
            produitPanier.Quantite--;

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
            string message = "";

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

    }
}