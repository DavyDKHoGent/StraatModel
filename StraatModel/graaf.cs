using System;
using System.Collections.Generic;
using System.Text;

namespace StraatModel
{
    public class Graaf 
    {
        public Graaf(int graafId, Dictionary<Knoop, List<Segment>> map)
        {
            GraafId = graafId;
            Map = map;
        }

        public int GraafId { get; set; }
        public Dictionary<Knoop, List<Segment>> Map { get; set; }
    }
}
