using System;
using System.Collections.Generic;
using System.Text;

namespace StraatModel
{
    public class Segment 
    {
        public Segment(int segmentID, Knoop beginKnoop, Knoop eindknoop, List<Punt> vertices)
        {
            BeginKnoop = beginKnoop;
            EindKnoop = eindknoop;
            SegmentID = segmentID;
            Vertices = vertices;
        }

        public Knoop BeginKnoop { get; set; }
        public Knoop EindKnoop { get; set; }
        public int SegmentID { get; set; }
        public List<Punt> Vertices { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Segment segment &&
                   EqualityComparer<Knoop>.Default.Equals(BeginKnoop, segment.BeginKnoop) &&
                   EqualityComparer<Knoop>.Default.Equals(EindKnoop, segment.EindKnoop) &&
                   SegmentID == segment.SegmentID &&
                   EqualityComparer<List<Punt>>.Default.Equals(Vertices, segment.Vertices);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BeginKnoop, EindKnoop, SegmentID, Vertices);
        }
    }
}
