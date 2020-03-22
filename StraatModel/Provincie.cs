using System;
using System.Collections.Generic;
using System.Text;

namespace StraatModel
{
    public class Provincie
    {
        public Provincie(int id, string naam, List<Gemeente> gemeentes)
        {
            Id = id;
            Naam = naam;
            Gemeentes = gemeentes;
        }

        public int Id { get; set; }
        public string Naam { get; set; }
        public List<Gemeente> Gemeentes { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Provincie provincie &&
                   Id == provincie.Id &&
                   Naam == provincie.Naam &&
                   EqualityComparer<List<Gemeente>>.Default.Equals(Gemeentes, provincie.Gemeentes);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Naam, Gemeentes);
        }
    }
}
