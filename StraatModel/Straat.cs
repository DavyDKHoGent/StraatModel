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
            Graaf = graaf;
        }

        public int StraatId { get; set; }
        public string Naam { get; set; }
        public Graaf Graaf { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Straat straat &&
                   StraatId == straat.StraatId &&
                   Naam == straat.Naam &&
                   EqualityComparer<Graaf>.Default.Equals(Graaf, straat.Graaf);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StraatId, Naam, Graaf);
        }
        public override string ToString()
        {
            return ($"{StraatId}, {Naam}, {Graaf.Map.Count}.");
        }
    }
}
