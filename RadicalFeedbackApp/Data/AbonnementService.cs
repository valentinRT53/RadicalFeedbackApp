using MySql.Data.MySqlClient;
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
            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                liste.Add(new Abonnement
                {
                    Id = reader.GetInt32("ID_ABONNEMENT"),
                    DateSouscription = reader.GetDateTime("DATESOUSCRIPTION_ABONNEMENT"),
                    Prix = reader.GetDouble("PRIX_ABONNEMENT")
                });
            }
            return liste;
        }

        public List<Utilisateur> GetUtilisateursByAbonnement(int idAbonnement)
        {
            var liste = new List<Utilisateur>();
            using var conn = db.GetConnection();
            if (conn == null) return liste;

            string query = @"
                SELECT u.ID_UTILISATEUR, u.NOM_UTILISTEUR, u.PRENOM_UTILISTAUR,
                       u.EMAIL_UTILISATEUR, u.VILLE_UTILISATEUR, u.STATUT_UTILISATEUR,
                       r.NOM_ROLE
                FROM UTILISATEUR u
                LEFT JOIN OBTENIR o ON u.ID_UTILISATEUR = o.ID_UTILISATEUR
                LEFT JOIN ROLE r ON o.ID_ROLE = r.ID_ROLE
                WHERE u.ID_ABONNEMENT = @id";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", idAbonnement);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                liste.Add(new Utilisateur
                {
                    Id = reader.GetInt32("ID_UTILISATEUR"),
                    Nom = reader.GetString("NOM_UTILISTEUR"),
                    Prenom = reader.GetString("PRENOM_UTILISTAUR"),
                    Email = reader.GetString("EMAIL_UTILISATEUR"),
                    Ville = reader.GetString("VILLE_UTILISATEUR"),
                    Statut = reader.GetString("STATUT_UTILISATEUR"),
                    Role = reader.IsDBNull(reader.GetOrdinal("NOM_ROLE"))
                           ? "Aucun" : reader.GetString("NOM_ROLE")
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
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@date", a.DateSouscription);
            cmd.Parameters.AddWithValue("@prix", a.Prix);
            cmd.ExecuteNonQuery();
        }

        public void Supprimer(int id)
        {
            using var conn = db.GetConnection();
            if (conn == null) return;

            // Réassigne les utilisateurs à aucun abonnement avant suppression
            string queryUpdate = "UPDATE UTILISATEUR SET ID_ABONNEMENT = NULL WHERE ID_ABONNEMENT = @id";
            using var cmdUpdate = new MySqlCommand(queryUpdate, conn);
            cmdUpdate.Parameters.AddWithValue("@id", id);
            cmdUpdate.ExecuteNonQuery();

            string query = "DELETE FROM ABONNEMENT WHERE ID_ABONNEMENT = @id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}