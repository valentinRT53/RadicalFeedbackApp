using MySql.Data.MySqlClient;
using RadicalFeedbackApp.Helpers;
using RadicalFeedbackApp.Models;
using System;
using System.Collections.Generic;

namespace RadicalFeedbackApp.Data
{
    public class SignalementService
    {
        private Database db = new Database();

        public List<Signalement> GetAllSignalements()
        {
            var liste = new List<Signalement>();
            using var conn = db.GetConnection();
            if (conn == null) return liste;

            string query = Session.EstAdmin ? @"
                SELECT s.ID_SIGNALEMENT, s.ID_UTILISATEUR, s.ID_EXPERT, s.ID__CONV,
                       s.CATEGORIE_SIGNALEMENT, s.COMMENTAIRE, s.DATE_SIGNALEMENT,
                       u.NOM_UTILISTEUR, u.PRENOM_UTILISTAUR,
                       e.NOM_UTILISTEUR AS NOM_EXPERT, e.PRENOM_UTILISTAUR AS PRENOM_EXPERT,
                       c.TITRE_CONV
                FROM SIGNALEMENT s
                JOIN UTILISATEUR u ON s.ID_UTILISATEUR = u.ID_UTILISATEUR
                LEFT JOIN UTILISATEUR e ON s.ID_EXPERT = e.ID_UTILISATEUR
                LEFT JOIN CONVERSATION c ON s.ID__CONV = c.ID__CONV
                ORDER BY s.DATE_SIGNALEMENT DESC"
            : @"
                SELECT s.ID_SIGNALEMENT, s.ID_UTILISATEUR, s.ID_EXPERT, s.ID__CONV,
                       s.CATEGORIE_SIGNALEMENT, s.COMMENTAIRE, s.DATE_SIGNALEMENT,
                       u.NOM_UTILISTEUR, u.PRENOM_UTILISTAUR,
                       e.NOM_UTILISTEUR AS NOM_EXPERT, e.PRENOM_UTILISTAUR AS PRENOM_EXPERT,
                       c.TITRE_CONV
                FROM SIGNALEMENT s
                JOIN UTILISATEUR u ON s.ID_UTILISATEUR = u.ID_UTILISATEUR
                LEFT JOIN UTILISATEUR e ON s.ID_EXPERT = e.ID_UTILISATEUR
                LEFT JOIN CONVERSATION c ON s.ID__CONV = c.ID__CONV
                WHERE s.ID_EXPERT = @id
                ORDER BY s.DATE_SIGNALEMENT DESC";

            using var cmd = new MySqlCommand(query, conn);
            if (!Session.EstAdmin)
                cmd.Parameters.AddWithValue("@id", Session.IdUtilisateur);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                liste.Add(new Signalement
                {
                    Id = reader.GetInt32("ID_SIGNALEMENT"),
                    IdUtilisateur = reader.GetInt32("ID_UTILISATEUR"),
                    IdExpert = reader.IsDBNull(reader.GetOrdinal("ID_EXPERT"))
                        ? null : reader.GetInt32("ID_EXPERT"),
                    IdConversation = reader.IsDBNull(reader.GetOrdinal("ID__CONV"))
                        ? null : reader.GetInt32("ID__CONV"),
                    Categorie = reader.GetString("CATEGORIE_SIGNALEMENT"),
                    Commentaire = reader.IsDBNull(reader.GetOrdinal("COMMENTAIRE"))
                        ? "" : reader.GetString("COMMENTAIRE"),
                    DateSignalement = reader.GetDateTime("DATE_SIGNALEMENT"),
                    NomUtilisateur = reader.GetString("NOM_UTILISTEUR"),
                    PrenomUtilisateur = reader.GetString("PRENOM_UTILISTAUR"),
                    NomExpert = reader.IsDBNull(reader.GetOrdinal("NOM_EXPERT"))
                        ? "" : reader.GetString("NOM_EXPERT"),
                    PrenomExpert = reader.IsDBNull(reader.GetOrdinal("PRENOM_EXPERT"))
                        ? "" : reader.GetString("PRENOM_EXPERT"),
                    TitreConversation = reader.IsDBNull(reader.GetOrdinal("TITRE_CONV"))
                        ? "" : reader.GetString("TITRE_CONV")
                });
            }
            return liste;
        }

        public void Supprimer(int id)
        {
            using var conn = db.GetConnection();
            if (conn == null) return;

            string query = "DELETE FROM SIGNALEMENT WHERE ID_SIGNALEMENT = @id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public List<string> GetMessagesConversation(int idConv)
        {
            var messages = new List<string>();
            using var conn = db.GetConnection();
            if (conn == null) return messages;

            string query = "SELECT TEXT_MESSAGE FROM MESSAGE WHERE ID__CONV = @id ORDER BY ID_MESSAGE";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", idConv);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                messages.Add(reader.GetString("TEXT_MESSAGE"));
            return messages;
        }
    }
}