using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace StraatModel
{
    public class Bevrager
    {
        private string _connectionString;
        public Bevrager(string connectionstring)
        {
            this._connectionString = connectionstring;
        }
        private SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            return connection;
        }
        public int GeefGemeenteId(string gemeenteNaam)
        {
            SqlConnection connection = GetConnection();
            string query = "SELECT id FROM gemeente WHERE naam=@gemeentenaam";
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("@gemeentenaam", SqlDbType.NVarChar));
                command.Parameters["@gemeentenaam"].Value = gemeenteNaam;
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
        } //OK!
        public Straat GeefStraat(int straatId)
        {
            SqlConnection connection = GetConnection();
            string query = "SELECT * FROM straat WHERE id=@straatid";
            using (SqlCommand command = connection.CreateCommand())
            {
                Graaf graaf = GeefGraaf(straatId);
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
        } //OK!
        public Straat GeefStraat(string straatNaam, string gemeenteNaam)
        {
            int gemeenteId = GeefGemeenteId(gemeenteNaam);
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
                        Knoop k = GeefKnoop((int)reader["knoopid"]);
                        Segment s = GeefSegment((int)reader["segmentid"]);
                        if (!map.ContainsKey(k))
                            map.Add(k, new List<Segment>() { s });
                        else
                            map[k].Add(s);

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
                    Punt p = new Punt((double)reader["x"], (double)reader["y"]);
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
            List<Punt> vertice = GeefVertice(segmentId);
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
                    int beginKNoopId = (int)reader["beginknoopid"];
                    int eindKnoopId = (int)reader["eindknoopid"];
                    reader.Close();
                    Segment segment = new Segment(segmentId, GeefKnoop(beginKNoopId), GeefKnoop(eindKnoopId), vertice);
                    return segment;
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
        public List<Punt> GeefVertice(int segmentId)
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
        public SortedSet<string> GeefStraatNamen(string gemeenteNaam)
        {
            int gemeenteId = GeefGemeenteId(gemeenteNaam);
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
        } //OK!

    }
}
