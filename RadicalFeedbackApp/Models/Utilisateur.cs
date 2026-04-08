namespace RadicalFeedbackApp.Models
{
    public class Utilisateur
    {
        public int Id { get; set; }
        public int IdAbonnement { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Ville { get; set; }
        public string Statut { get; set; }
        public string Role { get; set; }
        public string Login { get; set; } // ← AJOUTER
    }
}