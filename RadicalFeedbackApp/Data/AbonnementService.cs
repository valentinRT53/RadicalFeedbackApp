using Microsoft.Data.SqlClient;
using RadicalFeedbackApp.Models;
using System;
using System.Collections.Generic;

namespace RadicalFeedbackApp.Data
{
    public class AbonnementService
    {
        private Database db = new Database();

        public List<Abonnement> GetAllAbonnements()
        {
            var liste = new List<Abonnement>();
            using var conn = db.GetConnection();
            if (conn == null) return liste;

            string query = "SELECT ID_ABONNEMENT, DATESOUSCRIPTION_ABONNEMENT, PRIX_ABONNEMENT FROM ABONNEMENT";
            using var cmd = new SqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                liste.Add(new Abonnement
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID_ABONNEMENT")),
                    DateSouscription = reader.GetDateTime(reader.GetOrdinal("DATESOUSCRIPTION_ABONNEMENT")),
                    Prix = Convert.ToDouble(reader["PRIX_ABONNEMENT"])
                });
            }

            reader.Close(); // ← important : fermer le reader avant de refaire des requêtes

            // Charge les utilisateurs pour chaque abonnement
            foreach (var abo in liste)
                abo.Utilisateurs = GetUtilisateursByAbonnement(abo.Id);

            return liste;
        }

        public List<Utilisateur> GetUtilisateursByAbonnement(int idAbonnement)
        {
            var liste = new List<Utilisateur>();
            using var conn = db.GetConnection();
            if (conn == null) return liste;

            string query = @"
                SELECT u.ID_UTILISATEUR, u.NOM_UTILISATEUR, u.PRENOM_UTILISATEUR,
                       u.EMAIL_UTILISATEUR, u.VILLE_UTILISATEUR, u.STATUT_UTILISATEUR,
                       r.NOM_ROLE
                FROM UTILISATEUR u
                LEFT JOIN OBTENIR o ON u.ID_UTILISATEUR = o.ID_UTILISATEUR
                LEFT JOIN ROLE r ON o.ID_ROLE = r.ID_ROLE
                WHERE u.ID_ABONNEMENT = @id";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", idAbonnement);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                liste.Add(new Utilisateur
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID_UTILISATEUR")),
                    Nom = reader["NOM_UTILISATEUR"].ToString(),
                    Prenom = reader["PRENOM_UTILISATEUR"].ToString(),
                    Email = reader["EMAIL_UTILISATEUR"].ToString(),
                    Ville = reader["VILLE_UTILISATEUR"].ToString(),
                    Statut = reader["STATUT_UTILISATEUR"].ToString(),
                    Role = reader["NOM_ROLE"] == DBNull.Value
                           ? "Aucun"
                           : reader["NOM_ROLE"].ToString()
                });
            }
            return liste;
        }

        public void Ajouter(Abonnement a)
        {
            using var conn = db.GetConnection();
            if (conn == null) return;

            string query = @"INSERT INTO ABONNEMENT (DATESOUSCRIPTION_ABONNEMENT, PRIX_ABONNEMENT)
                             VALUES (@date, @prix)";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@date", a.DateSouscription);
            cmd.Parameters.AddWithValue("@prix", a.Prix);
            cmd.ExecuteNonQuery();
        }

        public void Supprimer(int id)
        {
            using var conn = db.GetConnection();
            if (conn == null) return;

            string queryUpdate = "UPDATE UTILISATEUR SET ID_ABONNEMENT = NULL WHERE ID_ABONNEMENT = @id";
            using var cmdUpdate = new SqlCommand(queryUpdate, conn);
            cmdUpdate.Parameters.AddWithValue("@id", id);
            cmdUpdate.ExecuteNonQuery();

            string query = "DELETE FROM ABONNEMENT WHERE ID_ABONNEMENT = @id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}