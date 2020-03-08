using System;
using System.Collections.Generic;
using System.Text;

namespace StraatModel
{
    public class Program
    {
        static void Main()
        {
            //var s = new StraatManager();
            //Dictionary<int, string> straatNamen = s.MaakDictionary();
            //foreach (var straatNaam in straatNamen)
            //    Console.WriteLine($"{straatNaam.Key}, {straatNaam.Value}");
            
            var k = new DataManager();
            k.MaakAlles();
            //k.check();
        }
    }
}
