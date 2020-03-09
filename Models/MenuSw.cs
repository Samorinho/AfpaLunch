using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AfpaLunch.Models
{
    public class MenuSw
    {
        public int IdMenu { get; set; }

        public List<int> produits { get; set; }

        public MenuSw()
        {
            IdMenu = 0;
            produits = new List<int>();
        }
    }
}