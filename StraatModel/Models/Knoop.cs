using System;
using System.Collections.Generic;
using System.Text;

namespace StraatModel
{
    public class Knoop
    {
        public int KnoopId { get; set; }
        public Punt Punt { get; set; }
        public Knoop(int id, Punt punt)
        {
            this.KnoopId = id;
            this.Punt = punt;
        }
        public override bool Equals(object obj)
        {
            return obj is Knoop knoop &&
                   KnoopId == knoop.KnoopId;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(KnoopId, Punt);
        }
        public override string ToString()
        {
            return $"           <Knoop>\n           <KnoopId>{KnoopId}</KnoopId>\n             {Punt.ToString()}\n         </Knoop>";
        }
    }
}
