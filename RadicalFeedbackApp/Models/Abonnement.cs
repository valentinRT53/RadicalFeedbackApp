using System;
using System.Collections.Generic;

namespace RadicalFeedbackApp.Models
{
    public class Abonnement
    {
        public int Id { get; set; }
        public DateTime DateSouscription { get; set; }
        public double Prix { get; set; }
        public List<Utilisateur> Utilisateurs { get; set; } = new();
        public int NbUtilisateurs => Utilisateurs.Count;
    }
}