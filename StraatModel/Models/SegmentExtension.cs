using System;
using System.Collections.Generic;
using System.Text;

namespace StraatModel
{
    public static class SegmentExtension
    {
        public static double Length(this Segment s)
        {
            double l = 0.0;
            for (int i = 1; i < s.GetCountVertices(); i++)
            {
                l += Math.Sqrt(Math.Pow(s.Vertices[i].X - s.Vertices[i - 1].X, 2) + Math.Pow(s.Vertices[i].Y - s.Vertices[i - 1].Y, 2));
            }
            return l;
        }
    }
}
