namespace RadicalFeedbackApp.Models
{
    public class Avis
    {
        public int Id { get; set; }
        public int IdConversation { get; set; }
        public int IdUtilisateur { get; set; }
        public int IdExpert { get; set; }
        public string Titre { get; set; }
        public string Texte { get; set; }
        public double Note { get; set; }
        public string NomUtilisateur { get; set; }
        public string PrenomUtilisateur { get; set; }
    }
}