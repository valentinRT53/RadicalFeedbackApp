using Microsoft.Data.SqlClient;
using System;

namespace RadicalFeedbackApp.Data
{
    public class Database
    {
        private string connectionString = "Server=192.168.152.253;Database=radical_feedback;User Id=sa;Password=P@ssw0rd2025;TrustServerCertificate=True;";

        public SqlConnection GetConnection()
        {
            try
            {
                var conn = new SqlConnection(connectionString);
                conn.Open();
                return conn;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur de connexion : " + ex.Message);
                return null;
            }
        }
    }
}