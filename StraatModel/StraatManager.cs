using System;
using System.IO;
using System.Collections.Generic;

namespace StraatModel
{
    public class StraatManager
    {
        public Dictionary<int, string> ReadStraatBestand()
        {
            string line;
            Dictionary<int, string> data = new Dictionary<int, string>();
            using (StreamReader r = new StreamReader(@"C:\Users\davy\Documents\data\WRdata-master\WRstraatnamen.csv"))
            {
                string headerline = r.ReadLine();
                while ((line = r.ReadLine()) != null)
                {
                    string[] collectie = line.Split(";");
                    int straatId = int.Parse(collectie[0]);
                    if (straatId != -9)
                        data.Add(straatId, collectie[1]);
                }
            }
            return data;
        }
    }
}
