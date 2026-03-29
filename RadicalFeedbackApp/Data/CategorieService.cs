using RadicalFeedbackApp.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace RadicalFeedbackApp.Data
{
    public class CategorieService
    {
        private Database db = new Database();

        public List<Categorie> GetAllCategories()
        {
            var liste = new List<Categorie>();
            using var conn = db.GetConnection();
            if (conn == null) return liste;

            // Requête SQL Server
            string query = "SELECT ID_CAT, NOM_CAT FROM CATEGORIE";
            using var cmd = new SqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                liste.Add(new Categorie
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID_CAT")),
                    Nom = reader.GetString(reader.GetOrdinal("NOM_CAT"))
                });
            }
            return liste;
        }
    }
}