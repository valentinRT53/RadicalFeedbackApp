using CommunityToolkit.Mvvm.ComponentModel;
using RadicalFeedbackApp.Data;
using RadicalFeedbackApp.Models;
using System.Collections.ObjectModel;

namespace RadicalFeedbackApp.ViewModels
{
    public class AbonnementViewModel : ObservableObject
    {
        private AbonnementService _service = new AbonnementService();
        public ObservableCollection<Abonnement> Abonnements { get; set; } = new();

        public AbonnementViewModel()
        {
            Load();
        }

        public void Load()
        {
            var list = _service.GetAllAbonnements();
            Abonnements.Clear();
            foreach (var a in list)
                Abonnements.Add(a);
        }

        public void Ajouter(Abonnement a)
        {
            _service.Ajouter(a);
            Load();
        }

        public void Supprimer(int id)
        {
            _service.Supprimer(id);
            Load();
        }
    }
}