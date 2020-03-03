using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AfpaLunch.Models
{
    public class ProduitComposePanier : ItemPanier
    {
        public int IdProduit { get; set; }
        public List<ProduitPanier> produits { get; set; }

        public override int GetIdProduit()
        {
            return IdProduit;
        }
    }
}