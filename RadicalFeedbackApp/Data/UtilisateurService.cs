using Microsoft.Data.SqlClient;
using RadicalFeedbackApp.Models;
using System;
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
SELECT u.ID_UTILISATEUR, u.ID_ABONNEMENT, u.NOM_UTILISATEUR,
       u.PRENOM_UTILISATEUR, u.EMAIL_UTILISATEUR,
       u.VILLE_UTILISATEUR, u.STATUT_UTILISATEUR,
       r.NOM_ROLE
FROM UTILISATEUR u
LEFT JOIN OBTENIR o ON u.ID_UTILISATEUR = o.ID_UTILISATEUR
LEFT JOIN ROLE r ON o.ID_ROLE = r.ID_ROLE";

            using var cmd = new SqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                utilisateurs.Add(new Utilisateur
                {
                    Id = (int)reader["ID_UTILISATEUR"],
                    IdAbonnement = (int)reader["ID_ABONNEMENT"],
                    Nom = reader["NOM_UTILISATEUR"].ToString(),
                    Prenom = reader["PRENOM_UTILISATEUR"].ToString(),
                    Email = reader["EMAIL_UTILISATEUR"].ToString(),
                    Ville = reader["VILLE_UTILISATEUR"].ToString(),
                    Statut = reader["STATUT_UTILISATEUR"].ToString(),
                    Role = reader["NOM_ROLE"] == DBNull.Value ? "Aucun" : reader["NOM_ROLE"].ToString()
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
                (ID_ABONNEMENT, NOM_UTILISATEUR, PRENOM_UTILISATEUR, EMAIL_UTILISATEUR, 
                 VILLE_UTILISATEUR, STATUT_UTILISATEUR, DATECREATION_UTILISATEUR)
                VALUES (@idAbo, @nom, @prenom, @email, @ville, @statut, GETDATE());
                SELECT SCOPE_IDENTITY();"; // récupère l'ID généré

            using var cmdUser = new SqlCommand(queryUser, conn);
            cmdUser.Parameters.AddWithValue("@idAbo", u.IdAbonnement);
            cmdUser.Parameters.AddWithValue("@nom", u.Nom);
            cmdUser.Parameters.AddWithValue("@prenom", u.Prenom);
            cmdUser.Parameters.AddWithValue("@email", u.Email);
            cmdUser.Parameters.AddWithValue("@ville", u.Ville);
            cmdUser.Parameters.AddWithValue("@statut", u.Statut);
            int newId = Convert.ToInt32(cmdUser.ExecuteScalar());

            // Insère la connexion
            string queryConn = @"
                INSERT INTO CONNEXION (ID_UTILISATEUR, LOGIN_CONNEXION, MPD_CONNEXION)
                VALUES (@id, @login, @mdp)";
            using var cmdConn = new SqlCommand(queryConn, conn);
            cmdConn.Parameters.AddWithValue("@id", newId);
            cmdConn.Parameters.AddWithValue("@login", login);
            cmdConn.Parameters.AddWithValue("@mdp", mdp);
            cmdConn.ExecuteNonQuery();

            // Attribue le rôle Utilisateur (ID_ROLE = 2)
            string queryRole = @"INSERT INTO OBTENIR (ID_UTILISATEUR, ID_ROLE) VALUES (@id, 2)";
            using var cmdRole = new SqlCommand(queryRole, conn);
            cmdRole.Parameters.AddWithValue("@id", newId);
            cmdRole.ExecuteNonQuery();
        }

        public void ModifierUtilisateur(Utilisateur u)
        {
            using var conn = db.GetConnection();
            if (conn == null) return;

            string query = @"
                UPDATE UTILISATEUR SET
                    NOM_UTILISATEUR = @nom,
                    PRENOM_UTILISATEUR = @prenom,
                    EMAIL_UTILISATEUR = @email,
                    VILLE_UTILISATEUR = @ville,
                    STATUT_UTILISATEUR = @statut,
                    ID_ABONNEMENT = @idAbo
                WHERE ID_UTILISATEUR = @id";

            using var cmd = new SqlCommand(query, conn);
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
                using var cmd = new SqlCommand(q, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public void ReinitialiseMdp(int id, string nouveauMdp)
        {
            using var conn = db.GetConnection();
            if (conn == null) return;

            string query = "UPDATE CONNEXION SET MPD_CONNEXION = @mdp WHERE ID_UTILISATEUR = @id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@mdp", nouveauMdp);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}