using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace StraatModel
{
    public class DatabaseManager
    {
        private string _connectionString;
        private int _gemeenteId;
        private int _provincieId;
        // de keys in deze dictionary zijn de oude, meegekregen Ids, de nieuwe Ids die worden aangemaakt door de database zitten in de values.
        private Dictionary<int, Knoop> _knopen = new Dictionary<int, Knoop>();
        private Dictionary<int, Segment> _segmenten = new Dictionary<int, Segment>();
        public DatabaseManager(string connectionstring)
        {
            this._connectionString = connectionstring;
        }
        private SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            return connection;
        }
        public void FillDatabase(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            DirectoryInfo[] directories = dir.GetDirectories();
            Console.WriteLine($"begonnen op {DateTime.Now}");
            foreach (DirectoryInfo dirinfo in directories)
            {
                Console.WriteLine($"begonnen met {dirinfo.Name} op {DateTime.Now}");
                AddProvincie(dirinfo.Name);
                FileInfo[] files = dirinfo.GetFiles();
                foreach (FileInfo file in files)
                {
                    string[] gemeentenaam = file.Name.Split(".");
                    AddGemeente(gemeentenaam[0]);
                    ReadGemeenteTxtFile(file.FullName);
                    Console.WriteLine($"    gedaan met {gemeentenaam[0]} op {DateTime.Now}");
                }
                Console.WriteLine($"{dirinfo.Name} gedaan op {DateTime.Now}");
            }
        }
        public void AddProvincie(string provincie)
        {
            SqlConnection connection = GetConnection();
            string query = "INSERT INTO provincie (naam) output INSERTED.ID VALUES( @naam)";
            using (SqlCommand command = connection.CreateCommand())
            {
                connection.Open();
                try
                {
                    command.Parameters.Add(new SqlParameter("@naam", SqlDbType.NVarChar));
                    command.CommandText = query;
                    command.Parameters["@naam"].Value = provincie;
                    _provincieId = (int)command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public void AddGemeente(string gemeente)
        {
            SqlConnection connection = GetConnection();
            string query = "INSERT INTO gemeente (naam, provincieid) output INSERTED.ID VALUES( @naam, @provincieid)";
            using (SqlCommand command = connection.CreateCommand())
            {
                connection.Open();
                try
                {
                    command.Parameters.Add(new SqlParameter("@naam", SqlDbType.NVarChar));
                    command.Parameters.Add(new SqlParameter("@provincieid", SqlDbType.Int));
                    command.CommandText = query;
                    command.Parameters["@naam"].Value = gemeente;
                    command.Parameters["@provincieid"].Value = _provincieId;
                    _gemeenteId = (int)command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        private void ReadGemeenteTxtFile(string path)
        {
            List<string> straat = new List<string>();
            string line;
            using (StreamReader r = new StreamReader(path))
            {
                while ((line = r.ReadLine()) != null)
                {
                    if (!line.Contains("</Straat>"))
                        straat.Add(line);
                    else
                    {
                        MaakStraat(straat);
                        straat.Clear();
                    }
                }
            }
        }
        private void MaakStraat(List<string> collectie)
        {
            Dictionary<int, List<int>> map = new Dictionary<int, List<int>>();
            for (int i = 6; i < collectie.Count; i++)
            {
                if (collectie[i].Contains("<entry>"))
                {
                    i += 2;
                    int oudKnoopId = int.Parse(Isoleer(collectie[i]));
                    if (!_knopen.ContainsKey(oudKnoopId))
                        AddKnoop(new Knoop(oudKnoopId, MaakPunt(collectie[i + 1])));
                    i += 3;
                    List<int> segmentIds = new List<int>();
                    while (!collectie[i].Contains("</entry>"))
                    {
                        List<string> segment = new List<string>();
                        while (!collectie[i].Contains("</Punten>") && i < collectie.Count - 4)
                        {
                            segment.Add(collectie[i]);
                            i++;
                        }
                        int oudSegmentId = int.Parse(Isoleer(segment[1]));
                        
                        if (!_segmenten.ContainsKey(oudSegmentId))
                            MaakSegment(oudSegmentId, segment);
                        
                        segmentIds.Add(_segmenten[oudSegmentId].SegmentID);
                        i++;
                    }
                    map.Add(_knopen[oudKnoopId].KnoopId, segmentIds);
                }
            }
            AddStraatEnGraaf(Isoleer(collectie[2]), map);
        }
        private string Isoleer(string value)
        {
            int begin = value.IndexOf(">") + 1;
            int eind = value.LastIndexOf("<");
            return value.Substring(begin, (eind - begin));
        }
        public void AddKnoop(Knoop knoop)
        {
            SqlConnection connection = GetConnection();
            string queryPunt = "INSERT INTO punt(x, y) output INSERTED.ID VALUES(@x, @y)";
            string queryKnoop = "INSERT INTO knoop(puntid) output INSERTED.ID VALUES(@puntid)";

            using (SqlCommand command1 = connection.CreateCommand())
            using (SqlCommand command2 = connection.CreateCommand())
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                command1.Transaction = transaction;
                command2.Transaction = transaction;
                try
                {
                    // punt toevoegen
                    command1.Parameters.Add(new SqlParameter("@x", SqlDbType.Float));
                    command1.Parameters.Add(new SqlParameter("@y", SqlDbType.Float));
                    command1.CommandText = queryPunt;
                    command1.Parameters["@x"].Value = knoop.Punt.X;
                    command1.Parameters["@y"].Value = knoop.Punt.Y;
                    int puntId = (int)command1.ExecuteScalar();

                    // knoop toevoegen
                    command2.Parameters.Add(new SqlParameter("@puntid", SqlDbType.Int));
                    command2.CommandText = queryKnoop;
                    command2.Parameters["@puntid"].Value = puntId;
                    int knoopId = (int)command2.ExecuteScalar();

                    transaction.Commit();
                    _knopen.Add(knoop.KnoopId, new Knoop(knoopId, new Punt(puntId, knoop.Punt.X, knoop.Punt.Y)));
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        private void MaakSegment(int segmentId, List<string> segment)
        {
            int beginKnoopId = int.Parse(Isoleer(segment[2]));
            if (!_knopen.ContainsKey(beginKnoopId))
                AddKnoop(new Knoop(beginKnoopId, MaakPunt(segment[6])));

            int eindKnoopId = int.Parse(Isoleer(segment[3]));
            if (!_knopen.ContainsKey(eindKnoopId))
                AddKnoop(new Knoop(eindKnoopId, MaakPunt(segment[segment.Count - 1])));

            List<Punt> vertices = new List<Punt>();
            vertices.Add(_knopen[beginKnoopId].Punt);
            for (int i = 7; i < segment.Count - 1; i++)
            {
                Punt p = MaakPunt(segment[i]);
                Punt punt = AddPunt(p);
                vertices.Add(punt);
            }
            vertices.Add(_knopen[eindKnoopId].Punt);

            AddSegment(new Segment(segmentId, _knopen[beginKnoopId], _knopen[eindKnoopId], vertices));
        }
        private Punt MaakPunt(string value)
        {
            string[] punten = Isoleer(value).Split(" ");
            Punt punt = new Punt(double.Parse(punten[0]), double.Parse(punten[1]));
            return punt;
        }
        public Punt AddPunt(Punt punt)
        {
            SqlConnection connection = GetConnection();
            string query = "INSERT INTO punt(x, y) output INSERTED.ID VALUES(@x, @y)";
            using (SqlCommand command = connection.CreateCommand())
            {
                connection.Open();
                try
                {
                    command.Parameters.Add(new SqlParameter("@x", SqlDbType.Float));
                    command.Parameters.Add(new SqlParameter("@y", SqlDbType.Float));
                    command.CommandText = query;
                    command.Parameters["@x"].Value = punt.X;
                    command.Parameters["@y"].Value = punt.Y;
                    int puntId = (int)command.ExecuteScalar();
                    return new Punt(puntId, punt.X, punt.Y);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public void AddSegment(Segment segment)
        {
            SqlConnection connection = GetConnection();
            string querySegment = "INSERT INTO segment(beginknoopid, eindknoopid) output INSERTED.ID VALUES(@beginknoopid, @eindknoopid)";
            string queryVertice = "INSERT INTO segment_punt(segmentid, puntid) VALUES(@segmentid, @puntid)";

            using (SqlCommand command1 = connection.CreateCommand())
            using (SqlCommand command2 = connection.CreateCommand())
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                command1.Transaction = transaction;
                command2.Transaction = transaction;
                try
                {
                    command1.Parameters.Add(new SqlParameter("@beginknoopid", SqlDbType.Int));
                    command1.Parameters.Add(new SqlParameter("@eindknoopid", SqlDbType.Int));
                    command1.CommandText = querySegment;
                    command1.Parameters["@beginknoopid"].Value = segment.BeginKnoop.KnoopId;
                    command1.Parameters["@eindknoopid"].Value = segment.EindKnoop.KnoopId;
                    int segmentId = (int)command1.ExecuteScalar();

                    command2.Parameters.Add(new SqlParameter("@segmentid", SqlDbType.Int));
                    command2.Parameters.Add(new SqlParameter("@puntid", SqlDbType.Int));
                    command2.CommandText = queryVertice;
                    command2.Parameters["@segmentid"].Value = segmentId;

                    foreach (var punt in segment.Vertices)
                    {
                        command2.Parameters["@puntid"].Value = punt.Id;
                        command2.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    _segmenten.Add(segment.SegmentID, new Segment(segmentId, segment.BeginKnoop, segment.EindKnoop, segment.Vertices));
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public void AddStraatEnGraaf(string straatnaam, Dictionary<int, List<int>> map)
        {
            SqlConnection connection = GetConnection();
            string queryStraat = "INSERT INTO straat(naam, gemeenteid) output INSERTED.ID VALUES(@naam, @gemeenteid)";
            string queryGraaf = "INSERT INTO graaf(knoopid, segmentid, straatid) VALUES(@knoopid, @segmentid, @straatid)";

            using (SqlCommand command1 = connection.CreateCommand())
            using (SqlCommand command2 = connection.CreateCommand())
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                command1.Transaction = transaction;
                command2.Transaction = transaction;
                try
                {
                    command1.Parameters.Add(new SqlParameter("@naam", SqlDbType.NVarChar));
                    command1.Parameters.Add(new SqlParameter("@gemeenteid", SqlDbType.Int));
                    command1.CommandText = queryStraat;
                    command1.Parameters["@naam"].Value = straatnaam;
                    command1.Parameters["@gemeenteid"].Value = _gemeenteId;
                    int straatId = (int)command1.ExecuteScalar();

                    // knoop toevoegen
                    command2.Parameters.Add(new SqlParameter("@knoopid", SqlDbType.Int));
                    command2.Parameters.Add(new SqlParameter("@segmentid", SqlDbType.Int));
                    command2.Parameters.Add(new SqlParameter("@straatid", SqlDbType.Int));
                    command2.CommandText = queryGraaf;

                    command2.Parameters["@straatid"].Value = straatId;
                    foreach (var knoopid in map)
                    {
                        command2.Parameters["@knoopid"].Value = knoopid.Key;
                        foreach (var segmentid in knoopid.Value)
                        {
                            command2.Parameters["@segmentid"].Value = segmentid;
                            command2.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
