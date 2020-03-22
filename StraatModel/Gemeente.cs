using System;
using System.Collections.Generic;
using System.Text;

namespace StraatModel
{
    public class Gemeente
    {
        public Gemeente(int id, string naam, List<Straat> straten)
        {
            Id = id;
            Naam = naam;
            this.straten = straten;
        }

        public int Id { get; set; }
        public string Naam { get; set; }
        public List<Straat> straten { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Gemeente gemeente &&
                   Id == gemeente.Id &&
                   Naam == gemeente.Naam &&
                   EqualityComparer<List<Straat>>.Default.Equals(straten, gemeente.straten);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Naam, straten);
        }
    }
}
