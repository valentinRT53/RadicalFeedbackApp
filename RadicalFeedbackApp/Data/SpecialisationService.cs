using Microsoft.Data.SqlClient;
using RadicalFeedbackApp.Models;
using System.Collections.Generic;

namespace RadicalFeedbackApp.Data
{
    public class SpecialisationService
    {
        private Database db = new Database();

        public List<Specialisation> GetAll()
        {
            var liste = new List<Specialisation>();
            using var conn = db.GetConnection();
            if (conn == null) return liste;

            string query = "SELECT ID_SPECIALISATION, NOM_SPECIALISATION, DESCRIPTION_SPECIALISATION FROM SPECIALISATION";
            using var cmd = new SqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                liste.Add(new Specialisation
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID_SPECIALISATION")),
                    Nom = reader.GetString(reader.GetOrdinal("NOM_SPECIALISATION")),
                    Description = reader.IsDBNull(reader.GetOrdinal("DESCRIPTION_SPECIALISATION"))
                        ? "" : reader.GetString(reader.GetOrdinal("DESCRIPTION_SPECIALISATION"))
                });
            }
            return liste;
        }

        public List<Specialisation> GetByExpert(int idExpert)
        {
            var liste = new List<Specialisation>();
            using var conn = db.GetConnection();
            if (conn == null) return liste;

            string query = @"
                SELECT s.ID_SPECIALISATION, s.NOM_SPECIALISATION, s.DESCRIPTION_SPECIALISATION
                FROM SPECIALISATION s
                JOIN EXPERT_SPECIALISATION es ON s.ID_SPECIALISATION = es.ID_SPECIALISATION
                WHERE es.ID_UTILISATEUR = @id";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", idExpert);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                liste.Add(new Specialisation
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID_SPECIALISATION")),
                    Nom = reader.GetString(reader.GetOrdinal("NOM_SPECIALISATION")),
                    Description = reader.IsDBNull(reader.GetOrdinal("DESCRIPTION_SPECIALISATION"))
                        ? "" : reader.GetString(reader.GetOrdinal("DESCRIPTION_SPECIALISATION"))
                });
            }
            return liste;
        }

        public void Ajouter(Specialisation s)
        {
            using var conn = db.GetConnection();
            if (conn == null) return;

            string query = @"INSERT INTO SPECIALISATION (NOM_SPECIALISATION, DESCRIPTION_SPECIALISATION)
                             VALUES (@nom, @desc)";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@nom", s.Nom);
            cmd.Parameters.AddWithValue("@desc", s.Description ?? "");
            cmd.ExecuteNonQuery();
        }

        public void Modifier(Specialisation s)
        {
            using var conn = db.GetConnection();
            if (conn == null) return;

            string query = @"UPDATE SPECIALISATION SET NOM_SPECIALISATION = @nom,
                             DESCRIPTION_SPECIALISATION = @desc
                             WHERE ID_SPECIALISATION = @id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@nom", s.Nom);
            cmd.Parameters.AddWithValue("@desc", s.Description ?? "");
            cmd.Parameters.AddWithValue("@id", s.Id);
            cmd.ExecuteNonQuery();
        }

        public void Supprimer(int id)
        {
            using var conn = db.GetConnection();
            if (conn == null) return;

            // Supprime les liaisons d'abord
            string queryLiaison = "DELETE FROM EXPERT_SPECIALISATION WHERE ID_SPECIALISATION = @id";
            using var cmdLiaison = new SqlCommand(queryLiaison, conn);
            cmdLiaison.Parameters.AddWithValue("@id", id);
            cmdLiaison.ExecuteNonQuery();

            string query = "DELETE FROM SPECIALISATION WHERE ID_SPECIALISATION = @id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public void SetSpecialisationsExpert(int idExpert, List<int> idsSpecialisations)
        {
            using var conn = db.GetConnection();
            if (conn == null) return;

            // Supprime les anciennes liaisons
            string deleteQuery = "DELETE FROM EXPERT_SPECIALISATION WHERE ID_UTILISATEUR = @id";
            using var deleteCmd = new SqlCommand(deleteQuery, conn);
            deleteCmd.Parameters.AddWithValue("@id", idExpert);
            deleteCmd.ExecuteNonQuery();

            // Insère les nouvelles
            foreach (var idSpec in idsSpecialisations)
            {
                string insertQuery = @"INSERT INTO EXPERT_SPECIALISATION (ID_UTILISATEUR, ID_SPECIALISATION)
                                      VALUES (@idUser, @idSpec)";
                using var insertCmd = new SqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@idUser", idExpert);
                insertCmd.Parameters.AddWithValue("@idSpec", idSpec);
                insertCmd.ExecuteNonQuery();
            }
        }
    }
}