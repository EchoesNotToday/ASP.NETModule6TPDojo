using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASP.NETModule6TPDojo.Models
{
    public class SamouraiVM
    {
        public Samourai Samourai { get; set; }
        public List<Arme> Armes { get; set; }
        public int? IdSelected { get; set; }
    }
}