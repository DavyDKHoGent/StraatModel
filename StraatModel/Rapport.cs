using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace StraatModel
{
    public class Rapport
    {
        public void AantalStratenPerProvincie()
        {
            Manager mng = new Manager();
            List<Provincie> provincies = mng.MaakProvincies();
            List<int> aantallen = new List<int>();
            int totaal = 0;
            foreach (Provincie prov in provincies)
            {
                int stratenperprovincie = 0;
                foreach (Gemeente gem in prov.Gemeentes)
                {
                    stratenperprovincie += gem.straten.Count();
                }
                aantallen.Add(stratenperprovincie);
                totaal += stratenperprovincie;
            }
            Console.WriteLine($"<{totaal}>\n");
            Console.WriteLine("aantal straten per provincie:\n");
            int i = 0;
            foreach (Provincie prov in provincies)
            {
                Console.WriteLine($"<{prov.Naam}: {aantallen[i]}>");
                i++;
            }
        }
        public void StraatInfo()
        {
            Manager mng = new Manager();
            List<Provincie> provincies = mng.MaakProvincies();
            //provincies[0];
            foreach (Provincie prov in provincies)
            {
                Console.WriteLine($"<straatinfo {prov}>");
                foreach (Gemeente gem in prov.Gemeentes)
                {

                }
            }
        }
    }
}
