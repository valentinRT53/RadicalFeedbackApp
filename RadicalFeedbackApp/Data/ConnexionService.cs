using MySql.Data.MySqlClient;

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
                SELECT c.ID_UTILISATEUR, r.NOM_ROLE 
                FROM CONNEXION c
                JOIN OBTENIR o ON c.ID_UTILISATEUR = o.ID_UTILISATEUR
                JOIN ROLE r ON o.ID_ROLE = r.ID_ROLE
                WHERE c.LOGIN_CONNEXION = @login AND c.MPD_CONNEXION = @mdp
                AND r.NOM_ROLE IN ('Admin', 'Expert')";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@login", login);
            cmd.Parameters.AddWithValue("@mdp", mdp);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int idUtilisateur = reader.GetInt32("ID_UTILISATEUR");
                string role = reader.GetString("NOM_ROLE");
                return (true, idUtilisateur, role);
            }

            return (false, 0, null);
        }
    }
}