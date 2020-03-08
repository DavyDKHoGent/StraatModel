using System;
using System.Collections.Generic;
using System.Text;

namespace StraatModel
{
    public class Punt
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Punt(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public override bool Equals(object obj)
        {
            return obj is Punt punt &&
                   X == punt.X &&
                   Y == punt.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
