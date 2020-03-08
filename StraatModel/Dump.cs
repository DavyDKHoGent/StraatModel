//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace StraatModel
//{
//    class Dump
//    {
//        using System;
//using System.Collections.Generic;
//using System.Text;
//using System.IO;

//namespace StraatModel
//    {
//        public class DataManager
//        {
//            Dictionary<int, string> _data;
//            public DataManager()
//            {
//                _data = new Dictionary<int, string>();
//                ReadStraatBestand();
//            }
//            public string
//            public void CreateGraaf()
//            {
//                string line;
//                using (StreamReader r = new StreamReader(@"C:\Users\davy\Documents\data\WRdata-master\WRdata.csv"))
//                {
//                    Dictionary<Knoop, List<Segment>> graaf = new Dictionary<Knoop, List<Segment>>();

//                    string headerLine = r.ReadLine();
//                    while ((line = r.ReadLine()) != null)
//                    {
//                        string[] Collectie = line.Split(";");

//                        #region geheugensteun
//                        var wegSegmentID = collecties[0]; // int => Segment (ok)
//                        var geo = collecties[1]; // lange lijst van doubles (ok)
//                        var morfologie = collecties[2]; // int (wordt niet gebruikt)
//                        var status = collecties[3]; // int (wordt niet gebruikt)
//                        var beginKnoop = collecties[4]; //int => Segment (ok)
//                        var eindknoop = collecties[5]; // int => segment (ok)
//                        var linksStraatNaamID = collecties[6]; // int (if -9 dan wegsmijten)
//                        var rechtsStraatNaamID = collecties[7]; //int (if -9 dan wegsmijten)
//                        #endregion
//                        int linksStraatNaamId = int.Parse(Collectie[6]);
//                        int rechtsStraatNaamId = int.Parse(Collectie[7]);
//                        if (linksStraatNaamId != -9 && rechtsStraatNaamId != -9)
//                        {
//                            lijst van 2 doubles met spatie gesplitst en komma's tussen de doubles (double double, double double, ...)
//                            string[] nummers = Collectie[1].Split(", ");
//                            nummers[nummers.Length - 1] = nummers[nummers.Length - 1].Remove(nummers[nummers.Length - 1].Length - 1);
//                            nummers[0] = nummers[0].Remove(0, 12);
//                            nog 2 doubles per index

//                            List<Punt> Vertices = new List<Punt>();
//                            for (int i = 0; i < nummers.Length; i++) // doubles worden hier gesplitst, in punten gezet en toegevoegd aan de lijst punten.
//                            {
//                                string[] doubles = nummers[i].Split(" ");
//                                double.TryParse(doubles[0], out double X);
//                                double.TryParse(doubles[1], out double Y);
//                                Punt punt = new Punt(X, Y);
//                                Vertices.Add(punt);
//                            }

//                            begin - en eindknopen aanmaken
//                            int beginKnoopID = int.Parse(Collectie[4]);
//                            Knoop beginKnoop = new Knoop(beginKnoopID, Vertices[0]);
//                            int EindKnoopID = int.Parse(Collectie[5]);
//                            Knoop eindKnoop = new Knoop(EindKnoopID, Vertices[Vertices.Count - 1]);

//                            segment aanmaken
//                            int wegSegmentId = int.Parse(Collectie[0]);
//                            Segment segment = new Segment(beginKnoop, eindKnoop, wegSegmentId, Vertices);

//                            Dictionary van graaf vullen(klopt nog niet volledig)
//                            if (!graaf.ContainsKey(beginKnoop))
//                                graaf.Add(beginKnoop, new List<Segment>() { segment });
//                            else
//                                graaf[beginKnoop].Add(segment);



//                        }
//                    }
//                }
//            }
//            public Dictionary<int, string> ReadStraatBestand()
//            {
//                string line;
//                Dictionary<int, string> _data = new Dictionary<int, string>();
//                using (StreamReader r = new StreamReader(@"C:\Users\davy\Documents\data\WRdata-master\WRstraatnamen.csv"))
//                {
//                    string headerline = r.ReadLine();
//                    while ((line = r.ReadLine()) != null)
//                    {
//                        string[] collectie = line.Split(";");
//                        int straatId = int.Parse(collectie[0]);
//                        if (straatId != -9)
//                            _data.Add(straatId, collectie[1]);
//                    }
//                }
//                return _data;
//            }
//        }
//    }

//}
//}
