using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace StraatModel
{
    public class DataManager
    {
        private Dictionary<int, string> _straatData;
        private List<string[]> _data;
        public DataManager()
        {
            _straatData = new Dictionary<int, string>();
            _data = new List<string[]>();
            ReadDataBestand();
            ReadStraatBestand();
        }
        public void MaakStraat()
        {

        }
        public Graaf MaakGraaf(Dictionary<string, List<Segment>> straatnaamEnListSegmenten)
        {
            Dictionary<Knoop, List<Segment>> map = new Dictionary<Knoop, List<Segment>>();
            foreach (var straatnaamEnListSegment in straatnaamEnListSegmenten)
            {
                for (int i = 0; i < straatnaamEnListSegment.Value.Count; i++)
                {
                    map.Add(straatnaamEnListSegment.Value[i].BeginKnoop, new List<Segment>() { straatnaamEnListSegment.Value[i] });
                }
                Graaf graaf = new Graaf(1, map);
                Straat straat = new Straat(1, straatnaamEnListSegment.Key, graaf);
            }
            return null;
        }
        public void MaakAlles()
        {
            #region geheugensteun
            //var wegSegmentID = collecties[0]; // int => Segment (ok)
            //var geo = collecties[1]; // lange lijst van doubles (ok)
            //var morfologie = collecties[2]; // int (wordt niet gebruikt)
            //var status = collecties[3]; // int (wordt niet gebruikt)
            //var beginKnoop = collecties[4]; //int => Segment (ok)
            //var eindknoop = collecties[5]; // int => segment (ok)
            //var linksStraatNaamID = collecties[6]; // int (if -9 dan wegsmijten) (ok)
            //var rechtsStraatNaamID = collecties[7]; //int (if -9 dan wegsmijten) (ok)
            #endregion
            Dictionary<string, List<Segment>> straatnaamEnListSegmenten = new Dictionary<string, List<Segment>>();
            Dictionary<string, Dictionary<Knoop, List<Segment>>> straat = new Dictionary<string, Dictionary<Knoop, List<Segment>>>();
            foreach (string[] collectie in _data)
            {
                int linksStraatNaamId = int.Parse(collectie[6]);
                int rechtsStraatNaamId = int.Parse(collectie[7]);
                if (linksStraatNaamId != -9 && rechtsStraatNaamId != -9)
                {
                    // test
                    Dictionary<string, Dictionary<Knoop, List<Segment>>> voorlopigeStraat = new Dictionary<string, Dictionary<Knoop, List<Segment>>>();
                    Segment segment = MaakSegment(collectie);
                    Dictionary<Knoop, List<Segment>> map = new Dictionary<Knoop, List<Segment>>();
                    map.Add(segment.BeginKnoop, new List<Segment>() { segment });
                    voorlopigeStraat.Add(_straatData[linksStraatNaamId], map);
                    // test

                    if (linksStraatNaamId == rechtsStraatNaamId)
                    {
                        if (!straatnaamEnListSegmenten.ContainsKey(_straatData[linksStraatNaamId]))
                            straatnaamEnListSegmenten.Add(_straatData[linksStraatNaamId], new List<Segment>() { segment });
                        else
                            straatnaamEnListSegmenten[_straatData[linksStraatNaamId]].Add(segment);
                    }
                    else if (linksStraatNaamId == -9 && rechtsStraatNaamId != -9)
                    {
                        if (!straatnaamEnListSegmenten.ContainsKey(_straatData[rechtsStraatNaamId]))
                            straatnaamEnListSegmenten.Add(_straatData[rechtsStraatNaamId], new List<Segment>() { segment });
                        else
                            straatnaamEnListSegmenten[_straatData[rechtsStraatNaamId]].Add(segment);
                    }
                    else if (linksStraatNaamId != 9 && rechtsStraatNaamId == -9)
                    {
                        if (!straatnaamEnListSegmenten.ContainsKey(_straatData[linksStraatNaamId]))
                            straatnaamEnListSegmenten.Add(_straatData[linksStraatNaamId], new List<Segment>() { segment });
                        else
                            straatnaamEnListSegmenten[_straatData[linksStraatNaamId]].Add(segment);
                    }
                }
            }

            MaakGraaf(straatnaamEnListSegmenten);
            //foreach (var straatnaamEnListSegment in straatnaamEnListSegmenten)
            //{
            //    Dictionary<Knoop, List<Segment>> map = new Dictionary<Knoop, List<Segment>>();
            //    foreach (var listSegment in straatnaamEnListSegment.Value)
            //    {
            //        map.Add(listSegment.BeginKnoop, straatnaamEnListSegment.Value);
            //    }
            //    Graaf graaf = new Graaf(1, map);
            //}



        }
        public List<Punt> MaakPunten(string punten)
        {
            List<Punt> vertices = new List<Punt>();
            // lijst van 2 doubles met spatie gesplitst en komma's tussen de doubles (double double, double double, ...)
            string[] nummers = punten.Split(", ");
            nummers[nummers.Length - 1] = nummers[nummers.Length - 1].Remove(nummers[nummers.Length - 1].Length - 1);
            nummers[0] = nummers[0].Remove(0, 12);
            // nog 2 doubles per index

            for (int i = 0; i < nummers.Length; i++) // doubles worden hier gesplitst, in punten gezet en toegevoegd aan de lijst punten.
            {
                string[] doubles = nummers[i].Split(" ");
                double.TryParse(doubles[0], out double X);
                double.TryParse(doubles[1], out double Y);
                vertices.Add(new Punt(X, Y));
            }
            return vertices;
        }
        public Segment MaakSegment(string[] collectie)
        {
            List<Punt> vertices = MaakPunten(collectie[1]);

            // begin -en eindknopen aanmaken
            int beginKnoopID = int.Parse(collectie[4]);
            Knoop beginKnoop = new Knoop(beginKnoopID, vertices[0]);
            int EindKnoopID = int.Parse(collectie[5]);
            Knoop eindKnoop = new Knoop(EindKnoopID, vertices[vertices.Count - 1]);

            // segment aanmaken
            int wegSegmentId = int.Parse(collectie[0]);
            Segment segment = new Segment(wegSegmentId, beginKnoop, eindKnoop, vertices);

            return segment;
        }
        public void ReadDataBestand()
        {
            string line;
            using (StreamReader r = new StreamReader(@"C:\Users\davy\Documents\data\WRdata-master\WRdata.csv"))
            {
                string headerline = r.ReadLine();
                while ((line = r.ReadLine()) != null)
                {
                    string[] collectie = line.Split(";");
                    if (collectie.Length != 8)
                        throw new ArgumentException($"De string is niet correct ingevuld");
                    else
                        _data.Add(collectie);
                }
            }
        }

        public void ReadStraatBestand()
        {
            string line;
            using (StreamReader r = new StreamReader(@"C:\Users\davy\Documents\data\WRdata-master\WRstraatnamen.csv"))
            {
                string headerline = r.ReadLine();
                string testline = r.ReadLine();
                while ((line = r.ReadLine()) != null)
                {
                    string[] collectie = line.Split(";");
                    if (collectie.Length != 2)
                        throw new ArgumentException($"De string is niet correct ingevuld");
                    else
                    {
                        int straatId = int.Parse(collectie[0]);
                        if (straatId != -9)
                            _straatData.Add(straatId, collectie[1].Trim());
                    }
                }
            }
        }
    }
}
