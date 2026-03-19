using System;

namespace RadicalFeedbackApp.Models
{
    public class CreneauAgenda
    {
        public DateTime Date { get; set; }
        public int Heure { get; set; }
        public bool Present { get; set; }
        public string Label => $"{Heure}h";
        public string DateLabel => Date.ToString("ddd dd/MM");
    }
}