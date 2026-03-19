using CommunityToolkit.Mvvm.ComponentModel;
using RadicalFeedbackApp.Data;
using RadicalFeedbackApp.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RadicalFeedbackApp.ViewModels
{
    public class SignalementViewModel : ObservableObject
    {
        private SignalementService _service = new SignalementService();
        private List<Signalement> _tousLesSignalements = new();

        public ObservableCollection<Signalement> Signalements { get; set; } = new();
        public ObservableCollection<string> Experts { get; set; } = new();

        private int _filtreExpertIndex = 0;
        public int FiltreExpertIndex
        {
            get => _filtreExpertIndex;
            set
            {
                SetProperty(ref _filtreExpertIndex, value);
                AppliquerFiltres();
            }
        }

        private int _filtreCategorie = 0;
        public int FiltreCategorie
        {
            get => _filtreCategorie;
            set
            {
                SetProperty(ref _filtreCategorie, value);
                AppliquerFiltres();
            }
        }

        public SignalementViewModel()
        {
            LoadSignalements();
        }
        public void Supprimer(int id)
        {
            _service.Supprimer(id);
            LoadSignalements();
        }
        public void LoadSignalements()
        {
            _tousLesSignalements = _service.GetAllSignalements();

            // Construit la liste des experts distincts pour le filtre
            Experts.Clear();
            Experts.Add("Tous les experts");
            var experts = _tousLesSignalements
                .Where(s => s.EstSignalementExpert)
                .Select(s => $"{s.PrenomExpert} {s.NomExpert}")
                .Distinct()
                .OrderBy(e => e);
            foreach (var e in experts)
                Experts.Add(e);

            AppliquerFiltres();
        }

        private void AppliquerFiltres()
        {
            var filtre = _tousLesSignalements.AsEnumerable();

            // Filtre expert
            if (_filtreExpertIndex > 0 && _filtreExpertIndex < Experts.Count)
            {
                string expertChoisi = Experts[_filtreExpertIndex];
                filtre = filtre.Where(s =>
                    $"{s.PrenomExpert} {s.NomExpert}" == expertChoisi ||
                    (!s.EstSignalementExpert && expertChoisi == Experts[_filtreExpertIndex]));
            }

            // Filtre catégorie
            filtre = _filtreCategorie switch
            {
                1 => filtre.Where(s => s.Categorie == "Comportement inapproprié"),
                2 => filtre.Where(s => s.Categorie == "Contenu incorrect"),
                _ => filtre
            };

            Signalements.Clear();
            foreach (var s in filtre)
                Signalements.Add(s);
        }
    }
}