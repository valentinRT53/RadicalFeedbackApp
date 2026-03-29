using Microsoft.Data.SqlClient;
using RadicalFeedbackApp.Models;
using System;
using System.Collections.Generic;

namespace RadicalFeedbackApp.Data
{
    public class AvisService
    {
        private Database db = new Database();

        public List<Avis> GetAvisParExpert(int idExpert)
        {
            var liste = new List<Avis>();
            using var conn = db.GetConnection();
            if (conn == null) return liste;

            string query = @"
                SELECT a.ID_AVIS, a.ID__CONV, a.ID_UTILISATEUR, a.ID_EXPERT,
                       a.TITRE_AVIS, a.TEXTE_AVIS, a.NOTE_AVIS,
                       u.NOM_UTILISTEUR, u.PRENOM_UTILISTAUR
                FROM AVIS a
                JOIN UTILISATEUR u ON a.ID_UTILISATEUR = u.ID_UTILISATEUR
                WHERE a.ID_EXPERT = @idExpert
                ORDER BY a.ID_AVIS DESC";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@idExpert", idExpert);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                liste.Add(new Avis
                {
                    Id = Convert.ToInt32(reader["ID_AVIS"]),
                    IdConversation = Convert.ToInt32(reader["ID__CONV"]),
                    IdUtilisateur = Convert.ToInt32(reader["ID_UTILISATEUR"]),
                    IdExpert = Convert.ToInt32(reader["ID_EXPERT"]),
                    Titre = reader["TITRE_AVIS"]?.ToString(),
                    Texte = reader["TEXTE_AVIS"]?.ToString(),
                    Note = Convert.ToDouble(reader["NOTE_AVIS"]),
                    NomUtilisateur = reader["NOM_UTILISTEUR"]?.ToString(),
                    PrenomUtilisateur = reader["PRENOM_UTILISTAUR"]?.ToString()
                });
            }
            return liste;
        }

        public double GetNoteMoyenne(int idExpert)
        {
            using var conn = db.GetConnection();
            if (conn == null) return 0;

            string query = "SELECT AVG(NOTE_AVIS) FROM AVIS WHERE ID_EXPERT = @id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", idExpert);

            var result = cmd.ExecuteScalar();
            if (result == null || result == DBNull.Value) return 0;

            return Math.Round(Convert.ToDouble(result), 1);
        }

        public List<(int id, string prenom, string nom)> GetTousLesExperts()
        {
            var liste = new List<(int, string, string)>();
            using var conn = db.GetConnection();
            if (conn == null) return liste;

            string query = @"
                SELECT u.ID_UTILISATEUR, u.PRENOM_UTILISTAUR, u.NOM_UTILISTEUR
                FROM UTILISATEUR u
                JOIN OBTENIR o ON u.ID_UTILISATEUR = o.ID_UTILISATEUR
                JOIN ROLE r ON o.ID_ROLE = r.ID_ROLE
                WHERE r.NOM_ROLE = 'Expert'";

            using var cmd = new SqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                liste.Add((
                    Convert.ToInt32(reader["ID_UTILISATEUR"]),
                    reader["PRENOM_UTILISTAUR"].ToString(),
                    reader["NOM_UTILISTEUR"].ToString()
                ));
            }
            return liste;
        }
    }
}