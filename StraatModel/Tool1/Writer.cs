using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace StraatModel
{
    public class Writer
    {
        private List<Provincie> _provincies;
        private DirectoryInfo _mainDirectory;
        public Writer(string pathToWrite, List<Provincie> provincies)
        {
            this._provincies = provincies;
            _mainDirectory = new DirectoryInfo(pathToWrite);
        }
        public void SchrijfData()
        {
            foreach (var prov in _provincies)
            {
                DirectoryInfo dirProvincies = _mainDirectory.CreateSubdirectory(prov.Naam);
                foreach (var gem in prov.Gemeentes)
                {
                    using StreamWriter writer = File.CreateText(Path.Combine(dirProvincies.FullName, gem.Naam + ".txt"));
                    gem.Straten.ForEach(straat => writer.WriteLine(straat));
                }
            }
        }
        public void SchrijfRapport()
        {
            using (StreamWriter writer = File.CreateText(Path.Combine(_mainDirectory.FullName, "Rapport.txt")))
            {
                Dictionary<string, int> stratenPerProvincie = GeefStratenPerProvincie();
                writer.WriteLine($"<Totaal Aantal Straten: {stratenPerProvincie.Values.Sum()}>\n");
                writer.WriteLine("Aantal straten per provincie:\n");
                foreach (var spp in stratenPerProvincie)
                    writer.WriteLine($" <{spp.Key}>: <{spp.Value}>");
                writer.WriteLine("");


                foreach (var provincie in _provincies)
                {
                    writer.WriteLine($"StraatInfo<{provincie.Naam}>:\n");
                    foreach (var gemeente in provincie.Gemeentes)
                    {
                        var orderedStraten = gemeente.Straten.OrderBy(straat => straat.GetLengte());
                        var totaleLengte = Math.Round(orderedStraten.Select(straat => straat.GetLengte()).Sum());
                        writer.WriteLine($" <{gemeente.Naam}>: <{gemeente.Straten.Count()}>, <{totaleLengte}M>");
                        var eerste = orderedStraten.First();
                        var laatste = orderedStraten.Last();
                        writer.WriteLine($"     <{eerste.StraatId}, {eerste.Naam}, {eerste.GetLengte()}M>");
                        writer.WriteLine($"     <{laatste.StraatId}, {laatste.Naam}, {laatste.GetLengte()}M>");
                    }
                }
            }
        }
        public Dictionary<string, int> GeefStratenPerProvincie()
        {
            Dictionary<string, int> uitkomst = new Dictionary<string, int>();
            foreach (Provincie provincie in _provincies)
            {
                var aantalStraten = provincie.Gemeentes.Select(gem => gem.Straten.Count).Sum();
                uitkomst.Add(provincie.Naam, aantalStraten);
            }
            return uitkomst;
        }
    }
}
