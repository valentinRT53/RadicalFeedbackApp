using System;

namespace RadicalFeedbackApp.Models
{
    public class Signalement
    {
        public int Id { get; set; }
        public int IdUtilisateur { get; set; }
        public int? IdExpert { get; set; }
        public int? IdConversation { get; set; }
        public string Categorie { get; set; }
        public string Commentaire { get; set; }
        public DateTime DateSignalement { get; set; }

        // Infos jointes
        public string NomUtilisateur { get; set; }
        public string PrenomUtilisateur { get; set; }
        public string NomExpert { get; set; }
        public string PrenomExpert { get; set; }
        public string TitreConversation { get; set; }

        // Helpers
        public bool EstSignalementExpert => IdExpert.HasValue;
        public string Cible => EstSignalementExpert
            ? $"Expert : {PrenomExpert} {NomExpert}"
            : $"Conversation : {TitreConversation}";
    }
}