﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AfpaLunch
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AfpEATEntities : DbContext
    {
        public AfpEATEntities()
            : base("name=AfpEATEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Categorie> Categories { get; set; }
        public virtual DbSet<Commande> Commandes { get; set; }
        public virtual DbSet<CommandeProduit> CommandeProduits { get; set; }
        public virtual DbSet<EtatCommande> EtatCommandes { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<Operation> Operations { get; set; }
        public virtual DbSet<Photo> Photos { get; set; }
        public virtual DbSet<Produit> Produits { get; set; }
        public virtual DbSet<Restaurant> Restaurants { get; set; }
        public virtual DbSet<TypeCuisine> TypeCuisines { get; set; }
        public virtual DbSet<TypeVersement> TypeVersements { get; set; }
        public virtual DbSet<Utilisateur> Utilisateurs { get; set; }
    }
}
