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
        public Graaf(Dictionary<Knoop, List<Segment>> map)
        {
            Map = map;
        }
        public int GraafId { get; set; }
        public Dictionary<Knoop, List<Segment>> Map { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Graaf graaf &&
                   GraafId == graaf.GraafId &&
                   EqualityComparer<Dictionary<Knoop, List<Segment>>>.Default.Equals(Map, graaf.Map);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GraafId, Map);
        }
        public override string ToString()
        {
            string eind = null;
            foreach (var m in Map)
            {
                string value = null;
                foreach (var t in m.Value)
                {
                    value += $"{t.ToString()}";
                }
                eind += $"        <entry>\n{m.Key.ToString()} {value}\n         </entry>\n";
            }
            return $"   <Graaf>\n   <GraafId>{GraafId}</GraafId>\n      <Map>\n{eind}       </Map>\n    </Graaf>";
        }
    }
}
