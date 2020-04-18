using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Linq;

namespace StraatModel
{
    public class Bevrager
    {
        private Dictionary<int, Knoop> _knopen;
        private Dictionary<int, Segment> _segmenten;
        private string _connectionString;
        public Bevrager(string connectionstring)
        {
            this._connectionString = connectionstring;
            _knopen = new Dictionary<int, Knoop>();
            _segmenten = new Dictionary<int, Segment>();
        }
        private SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            return connection;
        }
        public int GeefGemeenteId(string gemeenteNaam, int provincieId)
        {
            SqlConnection connection = GetConnection();
            string query = "SELECT id FROM gemeente WHERE naam=@gemeentenaam AND provincieId=@provincieId";
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("@gemeentenaam", SqlDbType.NVarChar));
                command.Parameters["@gemeentenaam"].Value = gemeenteNaam;
                command.Parameters.Add(new SqlParameter("@provincieId", SqlDbType.Int));
                command.Parameters["@provincieId"].Value = provincieId;
                connection.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    int id = (int)reader["id"];
                    reader.Close();
                    return id;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return 0;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public List<int> GeefStraatIds(string gemeenteNaam)
        {
            List<int> straatIds = new List<int>();
            SqlConnection connection = GetConnection();
            string queryS = "SELECT id FROM gemeente WHERE naam=@gemeentenaam";
            string querySC = "SELECT id FROM straat WHERE gemeenteid=@gemeenteid";
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = queryS;
                command.Parameters.Add(new SqlParameter("@gemeentenaam", SqlDbType.NVarChar));
                command.Parameters["@gemeentenaam"].Value = gemeenteNaam;
                connection.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    int gemeenteId = (int)reader["id"];
                    reader.Close();

                    command.CommandText = querySC;
                    command.Parameters.Add(new SqlParameter("@gemeenteid", SqlDbType.Int));
                    command.Parameters["@gemeenteid"].Value = gemeenteId;
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int straatId = (int)reader["id"];
                        straatIds.Add(straatId);
                    }
                    reader.Close();
                    return straatIds;
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
        public Straat GeefStraat(int straatId)
        {
            SqlConnection connection = GetConnection();
            string query = "SELECT * FROM straat WHERE id=@straatid";
            Graaf graaf = GeefGraaf(straatId);
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                SqlParameter paramNaam = command.Parameters.Add(new SqlParameter("@straatid", SqlDbType.Int));
                command.Parameters["@straatid"].Value = straatId;
                connection.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    Straat straat = new Straat(straatId, (string)reader["naam"], graaf);
                    reader.Close();
                    return straat;
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
        public Straat GeefStraat(string straatNaam, string gemeenteNaam)
        {
            int provincieId = GeefProvincieIdGemeenteNaam(gemeenteNaam);
            int gemeenteId = GeefGemeenteId(gemeenteNaam, provincieId);
            SqlConnection connection = GetConnection();
            string query = "SELECT * FROM straat WHERE naam=@straatnaam AND gemeenteid=@gemeenteid";
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("@straatnaam", SqlDbType.NVarChar));
                command.Parameters["@straatnaam"].Value = straatNaam;
                command.Parameters.Add(new SqlParameter("@gemeenteid", SqlDbType.Int));
                command.Parameters["@gemeenteid"].Value = gemeenteId;
                connection.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    int straatId = (int)reader["id"];
                    reader.Close();
                    return new Straat(straatId, straatNaam, GeefGraaf(straatId));
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
        public Graaf GeefGraaf(int straatId)
        {
            SqlConnection connection = GetConnection();
            Dictionary<Knoop, List<Segment>> map = new Dictionary<Knoop, List<Segment>>();
            string query = "SELECT knoopid, segmentid FROM graaf WHERE straatid=@straatId";
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("@straatId", SqlDbType.Int));
                command.Parameters["@straatId"].Value = straatId;
                connection.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int knoopId = (int)reader["knoopid"];
                        if (!_knopen.ContainsKey(knoopId))
                            _knopen.Add(knoopId, GeefKnoop(knoopId));
                        int segmentId = (int)reader["segmentid"];
                        if (!_segmenten.ContainsKey(segmentId))
                            _segmenten.Add(segmentId, GeefSegment(segmentId));

                        if (!map.ContainsKey(_knopen[knoopId]))
                            map.Add(_knopen[knoopId], new List<Segment>() { _segmenten[segmentId] });
                        else
                            map[_knopen[knoopId]].Add(_segmenten[segmentId]);

                    }
                    reader.Close();
                    return new Graaf(straatId, map);
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
        public Knoop GeefKnoop(int knoopId)
        {
            SqlConnection connection = GetConnection();
            string queryS = "SELECT * FROM knoop WHERE id=@knoopId";
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = queryS;
                command.Parameters.Add(new SqlParameter("@knoopId", SqlDbType.Int));
                command.Parameters["@knoopId"].Value = knoopId;
                connection.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    int puntId = (int)reader["puntid"];
                    reader.Close();
                    return new Knoop(knoopId, GeefPunt(puntId));
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
        public Punt GeefPunt(int puntId)
        {
            SqlConnection connection = GetConnection();
            string query = "SELECT x,y FROM punt WHERE id=@puntId";
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("@puntId", SqlDbType.Int));
                command.Parameters["@puntId"].Value = puntId;
                connection.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    Punt p = new Punt(puntId, (double)reader["x"], (double)reader["y"]);
                    reader.Close();
                    return p;
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
        public Segment GeefSegment(int segmentId)
        {
            SqlConnection connection = GetConnection();
            string query = "SELECT * FROM segment WHERE id=@segmentId";
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("@segmentId", SqlDbType.Int));
                command.Parameters["@segmentId"].Value = segmentId;
                connection.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    int beginKnoopId = (int)reader["beginknoopid"];
                    int eindKnoopId = (int)reader["eindknoopid"];
                    reader.Close();

                    if (!_knopen.ContainsKey(beginKnoopId))
                        _knopen.Add(beginKnoopId, GeefKnoop(beginKnoopId));
                    if (!_knopen.ContainsKey(eindKnoopId))
                        _knopen.Add(eindKnoopId, GeefKnoop(eindKnoopId));

                    Knoop beginKnoop = _knopen[beginKnoopId];
                    Knoop eindKnoop = _knopen[eindKnoopId];
                    List<Punt> vertice = GeefVertice(segmentId, beginKnoop.Punt.Id, eindKnoop.Punt.Id);
                    vertice.Insert(0, beginKnoop.Punt);
                    vertice.Add(eindKnoop.Punt);

                    return new Segment(segmentId, beginKnoop, eindKnoop, vertice);
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
        public List<Punt> GeefVertice(int segmentId, int beginPuntId, int eindPuntId)
        {
            SqlConnection connection = GetConnection();
            string query = "SELECT * FROM segment_punt WHERE segmentid=@segmentId";
            List<Punt> vertices = new List<Punt>();
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("@segmentId", SqlDbType.Int));
                command.Parameters["@segmentId"].Value = segmentId;
                connection.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int puntId = (int)reader["puntid"];
                        if (puntId != beginPuntId && puntId != eindPuntId)
                            vertices.Add(GeefPunt(puntId));
                    }
                    reader.Close();
                    return vertices;
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
        public SortedSet<string> GeefStraatNamen(string gemeenteNaam, string provincieNaam)
        {
            int provincieId = GeefProvincieIdProvincieNaam(provincieNaam);
            int gemeenteId = GeefGemeenteId(gemeenteNaam, provincieId);
            SqlConnection connection = GetConnection();
            string query = "SELECT naam FROM straat WHERE gemeenteid=@gemeenteId";
            SortedSet<string> straatNamen = new SortedSet<string>();
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("@gemeenteId", SqlDbType.Int));
                command.Parameters["@gemeenteId"].Value = gemeenteId;
                connection.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string straatNaam = (string)reader["naam"];
                        straatNamen.Add(straatNaam);
                    }
                    return straatNamen;
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
        public int GeefProvincieIdProvincieNaam(string provincieNaam)
        {
            SqlConnection connection = GetConnection();
            string query = "SELECT id FROM provincie WHERE naam=@provincieNaam";
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("@provincieNaam", SqlDbType.NVarChar));
                command.Parameters["@provincieNaam"].Value = provincieNaam;
                connection.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    int provincieId = (int)reader["id"];
                    reader.Close();
                    return provincieId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return 0;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public int GeefProvincieIdGemeenteNaam(string gemeenteNaam)
        {
            SqlConnection connection = GetConnection();
            string query = "SELECT provincieid FROM gemeente WHERE naam=@gemeenteNaam";
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("@gemeenteNaam", SqlDbType.NVarChar));
                command.Parameters["@gemeenteNaam"].Value = gemeenteNaam;
                connection.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    int provincieId = (int)reader["provincieid"];
                    reader.Close();
                    return provincieId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return 0;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public void GeefProvincieRapport(string provincieNaam)
        {
            int provincieId = GeefProvincieIdProvincieNaam(provincieNaam);
            SortedDictionary<string, List<int>> gemeenteData = GeefGemeenteData(provincieId);
            List<Gemeente> gemeentes = new List<Gemeente>();
            foreach (var gemeente in gemeenteData)
            {
                List<Straat> straten = new List<Straat>();
                foreach(var straatId in gemeente.Value)
                {
                    straten.Add(GeefStraat(straatId));
                }
                gemeentes.Add(new Gemeente(gemeente.Key, SorteerStraten(straten)));
            }
        }
        public List<Straat> SorteerStraten(List<Straat> straten)
        {
            return straten.OrderBy(str => str.GetLengte()).ToList();
        }
        public SortedDictionary<string, List<int>> GeefGemeenteData(int provincieId)
        {
            SqlConnection connection = GetConnection();
            string query = "SELECT * FROM gemeente WHERE provincieid=@provincieId";
            using (SqlCommand command = connection.CreateCommand())
            {
                SortedDictionary<string, List<int>> gemeentes = new SortedDictionary<string, List<int>>();
                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("@provincieId", SqlDbType.Int));
                command.Parameters["@provincieId"].Value = provincieId;
                connection.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string gemeenteNaam = (string)reader["naam"];
                        List<int> straten = GeefStraatIds((string)reader["naam"]);
                        gemeentes.Add(gemeenteNaam, straten);
                    }
                    reader.Close();
                    return gemeentes;
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
    }
}
