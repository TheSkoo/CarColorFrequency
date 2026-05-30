using CarColorFrequencyApi.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CarColorFrequencyApi.DataLayer
{
    public class DBAccess : IDisposable
    {
        private readonly string _connectionString = "Server=localhost;Database=CarColorFrequency;Trusted_Connection=True;TrustServerCertificate=True;";

        private SqlConnection _conn = null;

        public DBAccess()
        {
            _conn = new SqlConnection(_connectionString);
            _conn.Open();
        }

        public void Dispose()
        {
            _conn.Close();
        }

        public List<ColorData> GetColors()
        {
            var result = new List<ColorData>();
            SqlCommand cmd = _conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                "SELECT [ColorDictPK], [Color], [BackgroundColorRGB], [ForegroundColorRGB], [Count]"
                + "FROM [ColorDict]";
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new ColorData
                {
                    ColorDictId = reader.GetInt32(0),
                    Color = reader.GetString(1),
                    BackgroundColorRGB = reader.GetInt32(2),
                    ForegroundColorRGB = reader.GetInt32(3),
                    Count = reader.GetInt32(4)
                });
            }

            return result;
        }
    }
}
