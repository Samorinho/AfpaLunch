using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AfpaLunch.Models
{
    public class Panier : List<ItemPanier>
    {
        public int IdRestaurant { get; set; }
        public int Quantite { get; private set; }
        public decimal Total { get; private set; }

        private void CalculPanier()
        {
            Quantite = 0;
            Total = 0;
            Browse();
        }

        public bool AddItem(ItemPanier itemPanier)
        {
            int IdProduit = itemPanier.GetIdProduit();
            int IdMenu = itemPanier.GetIdMenu();
            bool isReturnOk = true;

            if(itemPanier != null)
            {
                if ((itemPanier is ProduitPanier || itemPanier is ProduitComposePanier) && IdProduit > 0)
                {
                    ItemPanier item = this.FirstOrDefault(p => p.GetIdProduit() == IdProduit);

                    if (item != null)
                    {
                        item.Quantite++;
                    }
                    else
                    {
                        this.Add(itemPanier);
                    }
                }

                else if (itemPanier is MenuPanier && IdMenu > 0)
                {
                    ItemPanier item = this.FirstOrDefault(p => p.GetIdMenu() == IdMenu);

                    if (item != null)
                    {
                        item.Quantite++;
                    }
                    else
                    {
                        this.Add(item);
                    }

                    this.Add(itemPanier);                    
                }

                else
                {
                    isReturnOk = false;
                }

                CalculPanier();
            }
            
            else
            {
                isReturnOk = false;
            }

            return isReturnOk;
        }

        public bool RemoveItem(int? IdProduit, int? IdMenu)
        {
            bool isReturnOk = false;
            ItemPanier item = null;

            if (IdProduit != null && IdProduit > 0)
            {
                 item = this.FirstOrDefault(p => p.GetIdProduit() == IdProduit);
            }

            else if (IdMenu != null && IdMenu > 0)
            {
                 item = this.FirstOrDefault(p => p.GetIdMenu() == IdMenu);
            }

            if (item != null)
            {
                this.Remove(item);
                isReturnOk = true;

                CalculPanier();
            }

            return isReturnOk;
        }
        private void Browse()
        {
            foreach (ItemPanier item in this)
            {
                Total += item.Quantite * item.Prix;
                Quantite += item.Quantite;
                IdRestaurant = item.IdRestaurant;
            }

        }
    }
}