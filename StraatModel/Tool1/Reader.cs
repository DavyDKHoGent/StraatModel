using System;
using System.Collections.Generic;
using System.IO;

namespace StraatModel
{
    public class Reader
    {
        private string _path;
        private Dictionary<int, Knoop> _knopen = new Dictionary<int, Knoop>();
        public Reader(string path)
        {
            this._path = path;
        }
        private Dictionary<int, List<Segment>> OrderSegmentenPerStraatId()
        {
            Dictionary<int, List<Segment>> straatIdEnListSegmenten = new Dictionary<int, List<Segment>>();
            string line;
            using (StreamReader r = new StreamReader(_path + @"\WRdata.csv"))
            {
                string headerline = r.ReadLine();
                while ((line = r.ReadLine()) != null)
                {
                    string[] collectie = line.Split(";");

                    // controle of de straat een naam heeft.
                    int linksStraatNaamId = int.Parse(collectie[6]);
                    int rechtsStraatNaamId = int.Parse(collectie[7]);
                    if (!(linksStraatNaamId == -9 && rechtsStraatNaamId == -9))
                    {
                        // segment aanmaken
                        Segment segment = MaakSegment(collectie);

                        // segment toevoegen per straatId in bovenstaande dictionary
                        // controle op Id's
                        // bij 2 dezelfde id's wordt de straat in 1 gemeente geplaatst 
                        if (linksStraatNaamId == rechtsStraatNaamId)
                        {
                            if (!straatIdEnListSegmenten.ContainsKey(linksStraatNaamId))
                                straatIdEnListSegmenten.Add(linksStraatNaamId, new List<Segment>() { segment });
                            else
                                straatIdEnListSegmenten[linksStraatNaamId].Add(segment);
                        }
                        // bij 2 verschillende id's wordt er gekeken of een id != -9 zoniet word de straat in 2 gemeentes gestopt
                        else if (linksStraatNaamId != rechtsStraatNaamId)
                        {
                            int[] straatIds = new int[] { linksStraatNaamId, rechtsStraatNaamId };
                            for (int i = 0; i < straatIds.Length; i++)
                            {
                                int straatId = straatIds[i];
                                if (straatId != -9)
                                {
                                    if (!straatIdEnListSegmenten.ContainsKey(straatId))
                                        straatIdEnListSegmenten.Add(straatId, new List<Segment>() { segment });
                                    else
                                        straatIdEnListSegmenten[straatId].Add(segment);
                                }
                            }
                        }

                    }
                }
            }
            return straatIdEnListSegmenten;
        }
        private Segment MaakSegment(string[] collectie)
        {
            // lijst met punten maken.
            List<Punt> vertices = MaakVertices(collectie[1]);

            // begin -en eindknopen aanmaken
            int beginKnoopId = int.Parse(collectie[4]);
            if (!_knopen.ContainsKey(beginKnoopId))
                _knopen.Add(beginKnoopId, new Knoop(beginKnoopId, vertices[0]));

            int eindKnoopId = int.Parse(collectie[5]);
            if (!_knopen.ContainsKey(eindKnoopId))
                _knopen.Add(eindKnoopId, new Knoop(eindKnoopId, vertices[vertices.Count - 1]));

            // segment aanmaken
            int wegSegmentId = int.Parse(collectie[0]);
            Segment segment = new Segment(wegSegmentId, _knopen[beginKnoopId], _knopen[eindKnoopId], vertices);

            return segment;
        }
        private List<Punt> MaakVertices(string punten)
        {
            // lijst van 2 doubles met spatie gesplitst en komma's tussen de doubles (double double, double double, ...)
            string[] nummers = punten.Split(", ");
            nummers[^1] = nummers[^1].Remove(nummers[^1].Length - 1);
            nummers[0] = nummers[0].Remove(0, 12);
            // nog 2 doubles per index

            // doubles splitsen, in punten zetten en toevoegen aan de lijst punten.
            List<Punt> vertices = new List<Punt>();
            for (int i = 0; i < nummers.Length; i++)
            {
                string[] doubles = nummers[i].Split(" ");
                double x = double.Parse(doubles[0].Replace(".", ","));
                double y = double.Parse(doubles[1].Replace(".", ","));
                vertices.Add(new Punt(x, y));
            }
            return vertices;
        }
        public Dictionary<int, int> StraatIdMetGemeenteId()
        {
            string line;
            Dictionary<int, int> straatIdEnGemeenteId = new Dictionary<int, int>();
            using (StreamReader r = new StreamReader(_path + @"\WRGemeenteID.csv"))
            {
                string headerLine = r.ReadLine();
                while ((line = r.ReadLine()) != null)
                {
                    string[] collectie = line.Split(";");
                    int straatId = int.Parse(collectie[0]);
                    int gemeenteId = int.Parse(collectie[1]);
                    straatIdEnGemeenteId.Add(straatId, gemeenteId);
                }
            }
            return straatIdEnGemeenteId;
        }
        private Dictionary<int, string> ReadStraatBestand()
        {
            Dictionary<int, string> straatData = new Dictionary<int, string>();
            string line;
            using (StreamReader r = new StreamReader(_path + @"\WRstraatnamen.csv"))
            {
                string headerline = r.ReadLine();
                string testline = r.ReadLine();
                while ((line = r.ReadLine()) != null)
                {
                    string[] collectie = line.Split(";");
                    int straatId = int.Parse(collectie[0]);
                    if (straatId != -9)
                        straatData.Add(straatId, collectie[1].Trim());
                }
            }
            return straatData;
        }
        private Dictionary<int, List<Straat>> MaakStratenEnLinkMetGemeenteId()
        {
            Dictionary<int, List<Segment>> straatIdEnListSegmenten = OrderSegmentenPerStraatId();
            Dictionary<int, string> straatIdEnNaam = ReadStraatBestand();
            Dictionary<int, int> straatIdEnGemeenteId = StraatIdMetGemeenteId();
            Dictionary<int, List<Straat>> gemeenteIdsEnStraten = new Dictionary<int, List<Straat>>();
            int nieuwId = 1;

            foreach (var straatIdEnSegment in straatIdEnListSegmenten)
            {
                // aanmaken graaf en straat
                Dictionary<Knoop, List<Segment>> map = new Dictionary<Knoop, List<Segment>>();
                foreach (Segment segment in straatIdEnSegment.Value)
                {
                    if (!map.ContainsKey(segment.BeginKnoop))
                        map.Add(segment.BeginKnoop, new List<Segment>() { segment });
                    else
                        map[segment.BeginKnoop].Add(segment);

                    if (!map.ContainsKey(segment.EindKnoop))
                        map.Add(segment.EindKnoop, new List<Segment>() { segment });
                    else
                        map[segment.EindKnoop].Add(segment);
                }
                Graaf graaf = new Graaf(nieuwId, map);
                Straat straat = new Straat(nieuwId, straatIdEnNaam[straatIdEnSegment.Key], graaf);
                // graaf en straat is aangemaakt
                int straatId = straatIdEnSegment.Key;

                if (straatIdEnGemeenteId.ContainsKey(straatId))
                {
                    // straten linken aan gemeenteIds en toevoegen aan dictionary.
                    if (!gemeenteIdsEnStraten.ContainsKey(straatIdEnGemeenteId[straatId]))
                        gemeenteIdsEnStraten.Add(straatIdEnGemeenteId[straatId], new List<Straat>() { straat });
                    else
                        gemeenteIdsEnStraten[straatIdEnGemeenteId[straatId]].Add(straat);
                    nieuwId++;
                }
            }
            return gemeenteIdsEnStraten;
        }
        private List<int> ProvincieIds()
        {
            List<int> provincieIDs = new List<int>();
            using (StreamReader r = new StreamReader(_path + @"\ProvincieIDsVlaanderen.csv"))
            {
                string header = r.ReadLine();
                string[] collectie = header.Split(",");
                for (int i = 0; i < collectie.Length; i++)
                {
                    int id = int.Parse(collectie[i]);
                    provincieIDs.Add(id);
                }
            }
            return provincieIDs;
        }
        private Dictionary<string, List<int>> ProvincieLinkMetGemeente()
        {
            List<int> provincieIDs = ProvincieIds();
            string line;
            Dictionary<string, List<int>> provincieNamenEnGemeenteIds = new Dictionary<string, List<int>>();
            using (StreamReader r = new StreamReader(_path + @"\ProvincieInfo.csv"))
            {
                string headerLine = r.ReadLine();
                while ((line = r.ReadLine()) != null)
                {
                    string[] collectie = line.Split(";");
                    int provincieId = int.Parse(collectie[1]);
                    if (collectie[2].Contains("nl") && provincieIDs.Contains(provincieId))
                    {
                        int gemeenteId = int.Parse(collectie[0]);
                        if (!provincieNamenEnGemeenteIds.ContainsKey(collectie[3]))
                            provincieNamenEnGemeenteIds.Add(collectie[3], new List<int>() { gemeenteId });
                        else
                            provincieNamenEnGemeenteIds[collectie[3]].Add(gemeenteId);
                    }
                }
            }
            return provincieNamenEnGemeenteIds;
        }
        private Dictionary<string, List<Gemeente>> MaakGemeentesEnLinkMetProvincie()
        {
            Dictionary<int, List<Straat>> gemeenteIdEnListStraten = MaakStratenEnLinkMetGemeenteId();
            Dictionary<string, List<int>> ProvincieEnListGemeenteId = ProvincieLinkMetGemeente();
            Dictionary<string, List<Gemeente>> provincieEnListGemeentes = new Dictionary<string, List<Gemeente>>();
            string line;
            using (StreamReader r = new StreamReader(_path + @"\WRGemeentenaam.csv"))
            {
                string headerLine = r.ReadLine();
                while ((line = r.ReadLine()) != null)
                {
                    string[] collectie = line.Split(";");
                    int gemeenteId = int.Parse(collectie[1]);
                    string taalCode = collectie[2];

                    if (taalCode == "nl" && gemeenteIdEnListStraten.ContainsKey(gemeenteId))
                    {
                        Gemeente g = new Gemeente(collectie[3], gemeenteIdEnListStraten[gemeenteId]);
                        foreach (var provincieNamenEnGemeenteId in ProvincieEnListGemeenteId)
                        {
                            if (provincieNamenEnGemeenteId.Value.Contains(gemeenteId))
                            {
                                if (!provincieEnListGemeentes.ContainsKey(provincieNamenEnGemeenteId.Key))
                                    provincieEnListGemeentes.Add(provincieNamenEnGemeenteId.Key, new List<Gemeente>() { g });
                                else
                                    provincieEnListGemeentes[provincieNamenEnGemeenteId.Key].Add(g);
                            }
                        }

                    }
                }
            }
            return provincieEnListGemeentes;
        }
        public List<Provincie> MaakData()
        {
            Dictionary<string, List<Gemeente>> provincies = MaakGemeentesEnLinkMetProvincie();
            List<Provincie> uitkomst = new List<Provincie>();
            foreach (var provincie in provincies)
            {
                Provincie p = new Provincie(provincie.Key, provincie.Value);
                uitkomst.Add(p);
            }
            return uitkomst;
        }
    }
}
