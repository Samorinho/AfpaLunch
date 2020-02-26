using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AfpaLunch.Models
{
    public class MenuPanier
    {
        public int IdMenu { get; set; }

        public List<Produit> produits { get; set; }

        public decimal Prix { get; set; }
        public int Quantite { get; set; }
    }
}