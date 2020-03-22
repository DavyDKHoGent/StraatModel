//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace StraatModel
//{
//    class Dump
//    {
//                public void AantalStratenPerProvincie()
//        {
//            Manager mng = new Manager();
//            Dictionary<string, Dictionary<string, List<Straat>>> uitkomst = mng.MaakGemeentes();
//            List<int> aantalStraten = new List<int>();

//            int totaleAantalStraten = 0;
//            foreach (var provincie in uitkomst)
//            {
//                int straten = 0;
//                foreach (var gemeente in provincie.Value)
//                    straten += gemeente.Value.Count;
//                aantalStraten.Add(straten);
//                totaleAantalStraten += straten;
//            }
//            Console.WriteLine($"Aantal Straten: [{totaleAantalStraten}]\n");
//            foreach (var provincie in uitkomst)
//            {
//                int i = 0;
//                Console.WriteLine($"[{provincie.Key}]: [{aantalStraten[i]}].");
//                i++;
//            }
//        }
//        public void StraatInfo()
//        {
//            Manager mng = new Manager();
//            Dictionary<string, Dictionary<string, List<Straat>>> uitkomst = mng.MaakGemeentes();

//        }
//    }
//}
