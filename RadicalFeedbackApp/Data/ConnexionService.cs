using Microsoft.Data.SqlClient;
using System;
using BCrypt.Net;

namespace RadicalFeedbackApp.Data
{
    public class ConnexionService
    {
        private Database db = new Database();

        public (bool succes, int idUtilisateur, string role) Authentifier(string login, string mdp)
        {
            using var conn = db.GetConnection();
            if (conn == null) return (false, 0, null);

            string query = @"
                SELECT c.ID_UTILISATEUR, c.MPD_CONNEXION, r.NOM_ROLE 
                FROM CONNEXION c
                JOIN OBTENIR o ON c.ID_UTILISATEUR = o.ID_UTILISATEUR
                JOIN ROLE r ON o.ID_ROLE = r.ID_ROLE
                WHERE c.LOGIN_CONNEXION = @login
                AND r.NOM_ROLE IN ('Admin', 'Expert')";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@login", login);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string hashEnBDD = reader["MPD_CONNEXION"].ToString();

                // 🔥 Vérification du mot de passe
                if (BCrypt.Net.BCrypt.Verify(mdp, hashEnBDD))
                {
                    int idUtilisateur = Convert.ToInt32(reader["ID_UTILISATEUR"]);
                    string role = reader["NOM_ROLE"].ToString();

                    return (true, idUtilisateur, role);
                }
            }

            return (false, 0, null);
        }
    }
}