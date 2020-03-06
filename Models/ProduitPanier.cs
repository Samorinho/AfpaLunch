using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AfpaLunch.Models
{
    public class ProduitPanier : ItemPanier
    {
        public int IdProduit { get; set; }

        public override int GetIdProduit()
        {
            return IdProduit;
        }

    }
}