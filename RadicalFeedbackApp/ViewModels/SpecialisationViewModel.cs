using CommunityToolkit.Mvvm.ComponentModel;
using RadicalFeedbackApp.Data;
using RadicalFeedbackApp.Models;
using System.Collections.ObjectModel;

namespace RadicalFeedbackApp.ViewModels
{
    public class SpecialisationViewModel : ObservableObject
    {
        private SpecialisationService _service = new SpecialisationService();
        public ObservableCollection<Specialisation> Specialisations { get; set; } = new();

        public SpecialisationViewModel()
        {
            Load();
        }

        public void Load()
        {
            var list = _service.GetAll();
            Specialisations.Clear();
            foreach (var s in list)
                Specialisations.Add(s);
        }

        public void Ajouter(Specialisation s)
        {
            _service.Ajouter(s);
            Load();
        }

        public void Modifier(Specialisation s)
        {
            _service.Modifier(s);
            Load();
        }

        public void Supprimer(int id)
        {
            _service.Supprimer(id);
            Load();
        }
    }
}