using MySql.Data.MySqlClient;
using System;

namespace RadicalFeedbackApp.Data
{
    public class Database
    {
        private string connectionString = "server=localhost;database=radical_feedback;user=root;password=;";

        public MySqlConnection GetConnection()
        {
            try
            {
                var conn = new MySqlConnection(connectionString);
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