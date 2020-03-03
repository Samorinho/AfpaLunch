using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AfpaLunch.Models
{
    public class MenuPanier : ItemPanier
    {
        public int IdMenu { get; set; }

        public List<ProduitPanier> produits { get; set; }

        public MenuPanier()
        {
            produits = new List<ProduitPanier>();
        }

        public override int GetIdMenu()
        {
            return IdMenu;
        }
    }
}