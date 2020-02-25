using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AfpaLunch.Models
{
    public class Basket
    {

        public Produit monproduit { get; set; }
        public decimal Prix { get; set; }
        public int Quantite { get; set; }
    }
}