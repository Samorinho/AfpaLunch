using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AfpaLunch.Models
{
    public class Resto
    {
        public int IdRestaurant { get; set; }

        public string Nom { get; set; }

        public Restaurant Restaurant { get; set; }
    }
}