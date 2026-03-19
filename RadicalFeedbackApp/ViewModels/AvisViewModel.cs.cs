using CommunityToolkit.Mvvm.ComponentModel;
using RadicalFeedbackApp.Models;
using RadicalFeedbackApp.Data;
using RadicalFeedbackApp.Helpers;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace RadicalFeedbackApp.ViewModels
{
    public class AvisViewModel : ObservableObject
    {
        private AvisService _service = new AvisService();

        public ObservableCollection<Avis> AvisList { get; set; } = new();
        public ObservableCollection<(int id, string prenom, string nom)> Experts { get; set; } = new();

        private double _noteMoyenne;
        public double NoteMoyenne
        {
            get => _noteMoyenne;
            set => SetProperty(ref _noteMoyenne, value);
        }

        private int _expertSelectionneIndex;
        public int ExpertSelectionneIndex
        {
            get => _expertSelectionneIndex;
            set
            {
                SetProperty(ref _expertSelectionneIndex, value);
                if (value >= 0 && value < Experts.Count)
                    ChargerAvis(Experts[value].id);
            }
        }

        public AvisViewModel()
        {
            if (Session.EstAdmin)
            {
                var experts = _service.GetTousLesExperts();
                foreach (var e in experts)
                    Experts.Add(e);

                if (Experts.Count > 0)
                {
                    _expertSelectionneIndex = 0;
                    ChargerAvis(Experts[0].id);
                }
            }
            else
            {
                ChargerAvis(Session.IdUtilisateur);
            }
        }

        public void ChargerAvis(int idExpert)
        {
            AvisList.Clear();
            var list = _service.GetAvisParExpert(idExpert);
            foreach (var a in list)
                AvisList.Add(a);

            NoteMoyenne = _service.GetNoteMoyenne(idExpert);
        }
    }
}