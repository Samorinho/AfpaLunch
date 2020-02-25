using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AfpaLunch.Models
{
    public class ProduitPanier
    {
        public int IdProduit { get; set; }

        public string Nom { get; set; }

        public string Description { get; set; }
        public decimal Prix { get; set; }
        public int Quantite { get; set; }

        public string Photo { get; set; }

        public int IdRestaurant { get; set; }
    }
}