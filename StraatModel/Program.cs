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
            //Tool 1
            string pathToRead = @"C:\Users\davy\Documents\data\WRdata";
            Reader reader = new Reader(pathToRead);
            List<Provincie> provincies = reader.MaakData();

            //string pathToWrite = @"C:\Users\davy\Documents\data\StraatModel";
            //Writer writer = new Writer(pathToWrite, provincies);
            //writer.SchrijfData();
            //writer.SchrijfRapport();

            
            ////tool 2
            //string connectionString = @"Data Source=LAPTOP-1U6AQSEQ\SQLEXPRESS;Initial Catalog=StraatModel;Integrated Security=True";
            //DatabaseManager databaseManager = new DatabaseManager(connectionString);
            //databaseManager.FillDatabase(pathToWrite);

            //// tool3
            //Bevrager bevrager = new Bevrager(connectionString);

            //List<int> straatIds = bevrager.GeefStraatIds("Aartselaar");
            //foreach (int straatId in straatIds)
            //    Console.WriteLine(straatId);

            //Straat straat1 = bevrager.GeefStraat(8);
            //Console.WriteLine(straat1);

            //Straat straat2 = bevrager.GeefStraat("Potaerdestraat", "Aartselaar");
            //Console.WriteLine(straat2);

            //SortedSet<string> straatNamen = bevrager.GeefStraatNamen("Aartselaar", "Antwerpen");
            //foreach (string naam in straatNamen)
            //    Console.WriteLine(naam);

        }

    }
}
