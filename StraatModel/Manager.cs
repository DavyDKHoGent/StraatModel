using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace StraatModel
{
    public class Manager
    {
        private Dictionary<int, List<Segment>> OrderSegmentenPerStraat()
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

            Dictionary<int, List<Segment>> straatIdEnListSegmenten = new Dictionary<int, List<Segment>>();
            List<string[]> data = ReadDataBestand();
            foreach (string[] collectie in data)
            {
                // controle of de straat een naam heeft.
                int linksStraatNaamId = int.Parse(collectie[6]);
                int rechtsStraatNaamId = int.Parse(collectie[7]);
                if (!(linksStraatNaamId == -9 && rechtsStraatNaamId == -9))
                {
                    // segment aanmaken
                    Segment segment = MaakSegment(collectie);

                    // segment toevoegen per straatId in bovenstaande dictionary
                    if (linksStraatNaamId == rechtsStraatNaamId)
                    {
                        if (!straatIdEnListSegmenten.ContainsKey(linksStraatNaamId))
                            straatIdEnListSegmenten.Add(linksStraatNaamId, new List<Segment>() { segment });
                        else
                            straatIdEnListSegmenten[linksStraatNaamId].Add(segment);
                    }
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
            return straatIdEnListSegmenten;
        }
        private Segment MaakSegment(string[] collectie)
        {
            // lijst met punten maken.
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
        private List<Punt> MaakPunten(string punten)
        {
            List<Punt> vertices = new List<Punt>();
            // lijst van 2 doubles met spatie gesplitst en komma's tussen de doubles (double double, double double, ...)
            string[] nummers = punten.Split(", ");
            nummers[nummers.Length - 1] = nummers[nummers.Length - 1].Remove(nummers[nummers.Length - 1].Length - 1);
            nummers[0] = nummers[0].Remove(0, 12);
            // nog 2 doubles per index

            // doubles splitsen, in punten zetten en toevoegen aan de lijst punten.
            for (int i = 0; i < nummers.Length; i++)
            {
                string[] doubles = nummers[i].Split(" ");
                double.TryParse(doubles[0], out double X);
                double.TryParse(doubles[1], out double Y);
                vertices.Add(new Punt(X, Y));
            }
            return vertices;
        }
        private Dictionary<int, int> straatIdMetGemeenteId()
        {
            string line;
            Dictionary<int, int> straatIdEnGemeenteId = new Dictionary<int, int>();
            using (StreamReader r = new StreamReader(@"C:\Users\davy\Documents\data\FileIO\FileIOData\StraatnaamID_gemeenteID.csv"))
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
        private Dictionary<int, List<Straat>> MaakStratenEnLinkMetGemeenteId()   // Klopt dit? Dic dat hier wordt gemaakt heeft minder gemeentes.
        {                                                                       // WRGemeenteID bevate niet alle straatdIDs die je in WRStraatnamen vindt
            Dictionary<int, List<Segment>> straatIdEnListSegmenten = OrderSegmentenPerStraat();
            Dictionary<int, string> straatIdEnNaam = ReadStraatBestand();
            Dictionary<int, int> straatIdEnGemeenteId = straatIdMetGemeenteId();
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

                if (straatIdEnGemeenteId.ContainsKey(straatId))         // voorlopige oplossing tot ik meer weet.
                {                                                       // als de straatId niet in WRgemeenteID zit negeer ik die straat voorlopig
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
        private List<string[]> ReadDataBestand()
        {
            List<string[]> data = new List<string[]>();
            string line;
            using (StreamReader r = new StreamReader(@"C:\Users\davy\Documents\data\WRdata\WRdata.csv"))
            {
                string headerline = r.ReadLine();
                while ((line = r.ReadLine()) != null)
                {
                    string[] collectie = line.Split(";");
                    if (collectie.Length != 8)
                        throw new ArgumentException($"De string is niet correct ingevuld");
                    else
                        data.Add(collectie);
                }
            }
            return data;
        }
        private Dictionary<int, string> ReadStraatBestand()
        {
            Dictionary<int, string> straatData = new Dictionary<int, string>();
            string line;
            using (StreamReader r = new StreamReader(@"C:\Users\davy\Documents\data\WRdata\WRstraatnamen.csv"))
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
                            straatData.Add(straatId, collectie[1].Trim());
                    }
                }
            }
            return straatData;
        }
        private List<int> ProvincieIds()
        {
            // lijst met provincieIDs aanmaken.
            List<int> provincieIDs = new List<int>();
            using (StreamReader r = new StreamReader(@"C:\Users\davy\Documents\data\WRdata\ProvincieIDsVlaanderen.csv"))
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
            using (StreamReader r = new StreamReader(@"C:\Users\davy\Documents\data\WRdata\ProvincieInfo.csv"))
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
            int nieuwId = 1;
            using (StreamReader r = new StreamReader(@"C:\Users\davy\Documents\data\WRdata\WRGemeentenaam.csv"))
            {
                string headerLine = r.ReadLine();
                while ((line = r.ReadLine()) != null)
                {
                    string[] collectie = line.Split(";");
                    int gemeenteId = int.Parse(collectie[1]);
                    string taalCode = collectie[2];

                    if (taalCode == "nl" && gemeenteIdEnListStraten.ContainsKey(gemeenteId)) // 2de voorlopige oplossing. controleren of de gemeenteId bestaat.
                    {
                        Gemeente g = new Gemeente(nieuwId, collectie[3], gemeenteIdEnListStraten[gemeenteId]);
                        nieuwId++;
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
        public List<Provincie> MaakProvincies()
        {
            Dictionary<string, List<Gemeente>> provincies = MaakGemeentesEnLinkMetProvincie();
            List<Provincie> uitkomst = new List<Provincie>();
            int id = 1;
            foreach (var provincie in provincies)
            {
                Provincie p = new Provincie(id, provincie.Key, provincie.Value);
                id++;
                uitkomst.Add(p);
            }
            return uitkomst;
        }
    }
}
