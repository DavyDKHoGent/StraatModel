using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace StraatModel
{
    public class Program
    {
        static void Main()
        {
            string path = @"C:\Users\davy\Documents\data\StraatModel";

            ////Tool 1
            //Writer writer = new Writer(path);
            //writer.SchrijfData();
            //writer.SchrijfRapport();

            ////tool 2
            //DatabaseManager databaseManager = new DatabaseManager(@"Data Source=LAPTOP-1U6AQSEQ\SQLEXPRESS;Initial Catalog=StraatModel;Integrated Security=True");
            //databaseManager.FillDatabase(path);

            //// tool3
            //Bevrager bevrager = new Bevrager(@"Data Source=LAPTOP-1U6AQSEQ\SQLEXPRESS;Initial Catalog=StraatModel;Integrated Security=True");

            //List<int> straatIds = bevrager.GeefStraatIds("Aartselaar");
            //foreach (int straatId in straatIds)
            //    Console.WriteLine(straatId);

            //Straat straat1 = bevrager.GeefStraat(90000);
            //Console.WriteLine(straat1);

            //Straat straat2 = bevrager.GeefStraat("Potaerdestraat", "Aartselaar");
            //Console.WriteLine(straat2);

            //SortedSet<string> straatNamen = bevrager.GeefStraatNamen("Aartselaar");
            //foreach (string naam in straatNamen)
            //    Console.WriteLine(naam);


        }

    }
}
