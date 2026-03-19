using RadicalFeedbackApp.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace RadicalFeedbackApp.Data
{
    public class UtilisateurService
    {
        private Database db = new Database();

        public List<Utilisateur> GetAllUtilisateurs()
        {
            var utilisateurs = new List<Utilisateur>();
            using var conn = db.GetConnection();
            if (conn == null) return utilisateurs;

            string query = @"
        SELECT u.ID_UTILISATEUR, u.ID_ABONNEMENT, u.NOM_UTILISTEUR, 
               u.PRENOM_UTILISTAUR, u.EMAIL_UTILISATEUR, 
               u.VILLE_UTILISATEUR, u.STATUT_UTILISATEUR,
               r.NOM_ROLE
        FROM UTILISATEUR u
        LEFT JOIN OBTENIR o ON u.ID_UTILISATEUR = o.ID_UTILISATEUR
        LEFT JOIN ROLE r ON o.ID_ROLE = r.ID_ROLE";

            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                utilisateurs.Add(new Utilisateur
                {
                    Id = reader.GetInt32("ID_UTILISATEUR"),
                    IdAbonnement = reader.GetInt32("ID_ABONNEMENT"),
                    Nom = reader.GetString("NOM_UTILISTEUR"),
                    Prenom = reader.GetString("PRENOM_UTILISTAUR"),
                    Email = reader.GetString("EMAIL_UTILISATEUR"),
                    Ville = reader.GetString("VILLE_UTILISATEUR"),
                    Statut = reader.GetString("STATUT_UTILISATEUR"),
                    Role = reader.IsDBNull(reader.GetOrdinal("NOM_ROLE"))
                           ? "Aucun" : reader.GetString("NOM_ROLE")
                });
            }
            return utilisateurs;
        }

        public void AjouterUtilisateur(Utilisateur u, string login, string mdp)
        {
            using var conn = db.GetConnection();
            if (conn == null) return;

            // Insère l'utilisateur
            string queryUser = @"
                INSERT INTO UTILISATEUR 
                (ID_ABONNEMENT, NOM_UTILISTEUR, PRENOM_UTILISTAUR, EMAIL_UTILISATEUR, 
                 VILLE_UTILISATEUR, STATUT_UTILISATEUR, DATECREATION_UTILISATEUR)
                VALUES (@idAbo, @nom, @prenom, @email, @ville, @statut, CURDATE())";

            using var cmdUser = new MySqlCommand(queryUser, conn);
            cmdUser.Parameters.AddWithValue("@idAbo", u.IdAbonnement);
            cmdUser.Parameters.AddWithValue("@nom", u.Nom);
            cmdUser.Parameters.AddWithValue("@prenom", u.Prenom);
            cmdUser.Parameters.AddWithValue("@email", u.Email);
            cmdUser.Parameters.AddWithValue("@ville", u.Ville);
            cmdUser.Parameters.AddWithValue("@statut", u.Statut);
            cmdUser.ExecuteNonQuery();

            // Récupère l'ID généré
            long newId = cmdUser.LastInsertedId;

            // Insère la connexion
            string queryConn = @"
                INSERT INTO CONNEXION (ID_UTILISATEUR, LOGIN_CONNEXION, MPD_CONNEXION)
                VALUES (@id, @login, @mdp)";
            using var cmdConn = new MySqlCommand(queryConn, conn);
            cmdConn.Parameters.AddWithValue("@id", newId);
            cmdConn.Parameters.AddWithValue("@login", login);
            cmdConn.Parameters.AddWithValue("@mdp", mdp);
            cmdConn.ExecuteNonQuery();

            // Attribue le rôle Utilisateur
            string queryRole = @"
                INSERT INTO OBTENIR (ID_UTILISATEUR, ID_ROLE) VALUES (@id, 2)";
            using var cmdRole = new MySqlCommand(queryRole, conn);
            cmdRole.Parameters.AddWithValue("@id", newId);
            cmdRole.ExecuteNonQuery();
        }

        public void ModifierUtilisateur(Utilisateur u)
        {
            using var conn = db.GetConnection();
            if (conn == null) return;

            string query = @"
                UPDATE UTILISATEUR SET
                    NOM_UTILISTEUR = @nom,
                    PRENOM_UTILISTAUR = @prenom,
                    EMAIL_UTILISATEUR = @email,
                    VILLE_UTILISATEUR = @ville,
                    STATUT_UTILISATEUR = @statut,
                    ID_ABONNEMENT = @idAbo
                WHERE ID_UTILISATEUR = @id";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@nom", u.Nom);
            cmd.Parameters.AddWithValue("@prenom", u.Prenom);
            cmd.Parameters.AddWithValue("@email", u.Email);
            cmd.Parameters.AddWithValue("@ville", u.Ville);
            cmd.Parameters.AddWithValue("@statut", u.Statut);
            cmd.Parameters.AddWithValue("@idAbo", u.IdAbonnement);
            cmd.Parameters.AddWithValue("@id", u.Id);
            cmd.ExecuteNonQuery();
        }

        public void SupprimerUtilisateur(int id)
        {
            using var conn = db.GetConnection();
            if (conn == null) return;

            // Supprime dans l'ordre des FK
            foreach (var q in new[]
            {
                "DELETE FROM DISPONIBILITE WHERE ID_UTILISATEUR = @id",
                "DELETE FROM SIGNALEMENT WHERE ID_UTILISATEUR = @id",
                "DELETE FROM AVIS WHERE ID_UTILISATEUR = @id",
                "DELETE FROM OBTENIR WHERE ID_UTILISATEUR = @id",
                "DELETE FROM CONNEXION WHERE ID_UTILISATEUR = @id",
                "DELETE FROM UTILISATEUR WHERE ID_UTILISATEUR = @id"
            })
            {
                using var cmd = new MySqlCommand(q, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public void ReinitialiseMdp(int id, string nouveauMdp)
        {
            using var conn = db.GetConnection();
            if (conn == null) return;

            string query = "UPDATE CONNEXION SET MPD_CONNEXION = @mdp WHERE ID_UTILISATEUR = @id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@mdp", nouveauMdp);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}