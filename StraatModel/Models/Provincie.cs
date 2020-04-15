using System;
using System.Collections.Generic;
using System.Text;

namespace StraatModel
{
    public class Provincie
    {
        public Provincie(string naam, List<Gemeente> gemeentes)
        {
            Naam = naam;
            Gemeentes = gemeentes;
        }

        public string Naam { get; set; }
        public List<Gemeente> Gemeentes { get; set; }
        public override bool Equals(object obj)
        {
            return obj is Provincie provincie &&
                   Naam == provincie.Naam &&
                   EqualityComparer<List<Gemeente>>.Default.Equals(Gemeentes, provincie.Gemeentes);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Naam, Gemeentes);
        }
        public override string ToString()
        {
            return ($"naam: {Naam}, aantal gemeentes: {Gemeentes.Count}");
        }
    }
}
