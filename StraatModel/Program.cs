using System;
using System.Collections.Generic;
using System.Text;

namespace StraatModel
{
    public class Program
    {
        static void Main()
        {
            Manager mng = new Manager();
            //Dictionary<string, Dictionary<string, List<Straat>>> t = stm.Uitkomst();
            //mng.MaakProvincies();
            Rapport rpt = new Rapport();
            rpt.AantalStratenPerProvincie();
            //rpt.StraatInfo();
        }
    }
}
