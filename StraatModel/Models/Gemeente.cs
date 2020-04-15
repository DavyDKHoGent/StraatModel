using System;
using System.Collections.Generic;
using System.Text;

namespace StraatModel
{
    public class Gemeente
    {
        public Gemeente(string naam, List<Straat> straten)
        {
            Naam = naam;
            this.Straten = straten;
        }

        public string Naam { get; set; }
        public List<Straat> Straten { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Gemeente gemeente &&
                   Naam == gemeente.Naam &&
                   EqualityComparer<List<Straat>>.Default.Equals(Straten, gemeente.Straten);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Naam, Straten);
        }
        public override string ToString()
        {
            //foreach (Straat straat in Straten)
            //{
            //    straat.
            //}
            //Straten.ForEach(straat => straat.Naam)
            return ($"<Gemeente>{Naam}\n</Gemeente>");
        }
    }
}
