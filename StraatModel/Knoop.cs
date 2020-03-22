using System;
using System.Collections.Generic;
using System.Text;

namespace StraatModel
{
    public class Knoop
    {
        public int KnoopID { get; set; }
        public Punt punt { get; set; }
        public Knoop(int id, Punt punt)
        {
            this.KnoopID = id;
            this.punt = punt;
        }
        public override bool Equals(object obj)
        {
            return obj is Knoop knoop &&
                   KnoopID == knoop.KnoopID;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(KnoopID, punt);
        }
    }
}
