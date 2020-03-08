using System;
using System.Collections.Generic;
using System.Text;

namespace StraatModel
{
    public class Straat
    {
        public Straat(int straatId, string naam, Graaf graaf)
        {
            StraatId = straatId;
            Naam = naam;
        }
        public int StraatId { get; set; }
        public string Naam { get; set; }
        public Graaf Graaf { get; set; }
    }
}
