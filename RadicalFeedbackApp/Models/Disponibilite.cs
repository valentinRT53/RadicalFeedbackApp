using System;

namespace RadicalFeedbackApp.Models
{
    public class Disponibilite
    {
        public int Id { get; set; }
        public int IdUtilisateur { get; set; }
        public DateTime Date { get; set; }
        public int Heure { get; set; }
        public bool Present { get; set; }
    }
}