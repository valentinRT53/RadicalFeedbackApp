using Microsoft.Data.SqlClient;
using RadicalFeedbackApp.Models;
using System;
using System.Collections.Generic;

namespace RadicalFeedbackApp.Data
{
    public class DisponibiliteService
    {
        private Database db = new Database();

        // Récupère les dispos d'un utilisateur pour la semaine en cours
        public List<Disponibilite> GetDisposSemaine(int idUtilisateur, DateTime lundi, DateTime dimanche)
        {
            var liste = new List<Disponibilite>();
            using var conn = db.GetConnection();
            if (conn == null) return liste;

            string query = @"
        SELECT ID_DISPONIBILITE, ID_UTILISATEUR, DATE_DISPO, HEURE_DISPO, PRESENT
        FROM DISPONIBILITE
        WHERE ID_UTILISATEUR = @id
        AND DATE_DISPO BETWEEN @lundi AND @dimanche";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", idUtilisateur);
            cmd.Parameters.AddWithValue("@lundi", lundi);
            cmd.Parameters.AddWithValue("@dimanche", dimanche);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                liste.Add(new Disponibilite
                {
                    Id = Convert.ToInt32(reader["ID_DISPONIBILITE"]),
                    IdUtilisateur = Convert.ToInt32(reader["ID_UTILISATEUR"]),
                    Date = Convert.ToDateTime(reader["DATE_DISPO"]),
                    Heure = Convert.ToInt32(reader["HEURE_DISPO"]),
                    Present = Convert.ToBoolean(reader["PRESENT"])
                });
            }
            return liste;
        }

        // Récupère les dispos de TOUS les utilisateurs (admin)
        public List<Disponibilite> GetDisposSemaineParUtilisateur(int idUtilisateur, DateTime lundi, DateTime dimanche)
        {
            return GetDisposSemaine(idUtilisateur, lundi, dimanche);
        }

        // Coche ou décoche un créneau
        public void ToggleDispo(int idUtilisateur, DateTime date, int heure, bool present)
        {
            using var conn = db.GetConnection();
            if (conn == null) return;

            // Vérifie si le créneau existe déjà
            string checkQuery = @"
                SELECT COUNT(*) FROM DISPONIBILITE
                WHERE ID_UTILISATEUR = @id AND DATE_DISPO = @date AND HEURE_DISPO = @heure";

            using var checkCmd = new SqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@id", idUtilisateur);
            checkCmd.Parameters.AddWithValue("@date", date);
            checkCmd.Parameters.AddWithValue("@heure", heure);

            int count = (int)checkCmd.ExecuteScalar();

            if (count > 0)
            {
                // Met à jour
                string updateQuery = @"
                    UPDATE DISPONIBILITE SET PRESENT = @present
                    WHERE ID_UTILISATEUR = @id AND DATE_DISPO = @date AND HEURE_DISPO = @heure";

                using var updateCmd = new SqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@present", present);
                updateCmd.Parameters.AddWithValue("@id", idUtilisateur);
                updateCmd.Parameters.AddWithValue("@date", date);
                updateCmd.Parameters.AddWithValue("@heure", heure);
                updateCmd.ExecuteNonQuery();
            }
            else
            {
                // Insère
                string insertQuery = @"
                    INSERT INTO DISPONIBILITE (ID_UTILISATEUR, DATE_DISPO, HEURE_DISPO, PRESENT)
                    VALUES (@id, @date, @heure, @present)";

                using var insertCmd = new SqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@id", idUtilisateur);
                insertCmd.Parameters.AddWithValue("@date", date);
                insertCmd.Parameters.AddWithValue("@heure", heure);
                insertCmd.Parameters.AddWithValue("@present", present);
                insertCmd.ExecuteNonQuery();
            }
        }

        // Récupère tous les utilisateurs qui ont des dispos (pour le menu admin)
        public List<(int id, string login)> GetUtilisateursAvecDispos()
        {
            var liste = new List<(int, string)>();
            using var conn = db.GetConnection();
            if (conn == null) return liste;

            string query = @"
                SELECT u.ID_UTILISATEUR, c.LOGIN_CONNEXION
                FROM UTILISATEUR u
                JOIN CONNEXION c ON u.ID_UTILISATEUR = c.ID_UTILISATEUR
                JOIN OBTENIR o ON u.ID_UTILISATEUR = o.ID_UTILISATEUR
                JOIN ROLE r ON o.ID_ROLE = r.ID_ROLE
                WHERE r.NOM_ROLE = 'Expert'";

            using var cmd = new SqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                liste.Add((reader.GetInt32(reader.GetOrdinal("ID_UTILISATEUR")),
                           reader.GetString(reader.GetOrdinal("LOGIN_CONNEXION"))));
            return liste;
        }
    }
}