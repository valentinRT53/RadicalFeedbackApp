using CommunityToolkit.Mvvm.ComponentModel;
using RadicalFeedbackApp.Data;
using RadicalFeedbackApp.Helpers;
using RadicalFeedbackApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RadicalFeedbackApp.ViewModels
{
    public class AgendaViewModel : ObservableObject
    {
        private DisponibiliteService _service = new DisponibiliteService();

        public ObservableCollection<CreneauAgenda> Creneaux { get; set; } = new();
        public ObservableCollection<(int id, string login)> Utilisateurs { get; set; } = new();
        public List<DateTime> Jours { get; set; } = new();
        public List<int> Heures { get; } = new() { 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };

        public string LabelSemaine { get; set; } = "";

        private int _semaineOffset = 0;
        public int SemaineOffset
        {
            get => _semaineOffset;
            set
            {
                SetProperty(ref _semaineOffset, value);
                RecalculerJours();
            }
        }

        private int _idUtilisateurSelectionne;
        public int IdUtilisateurSelectionne
        {
            get => _idUtilisateurSelectionne;
            set
            {
                SetProperty(ref _idUtilisateurSelectionne, value);
                ChargerAgenda(value);
            }
        }

        private int _selectedUtilisateurIndex;
        public int SelectedUtilisateurIndex
        {
            get => _selectedUtilisateurIndex;
            set
            {
                SetProperty(ref _selectedUtilisateurIndex, value);
                if (value >= 0 && value < Utilisateurs.Count)
                    IdUtilisateurSelectionne = Utilisateurs[value].id;
            }
        }

        public AgendaViewModel()
        {
            RecalculerJours();

            if (Session.EstAdmin)
            {
                var users = _service.GetUtilisateursAvecDispos();
                foreach (var u in users)
                    Utilisateurs.Add(u);

                if (Utilisateurs.Count > 0)
                {
                    _selectedUtilisateurIndex = 0;
                    ChargerAgenda(Utilisateurs[0].id);
                }
            }
            else
            {
                ChargerAgenda(Session.IdUtilisateur);
            }
        }

        private void RecalculerJours()
        {
            Jours.Clear();
            DateTime aujourd_hui = DateTime.Today;
            int diff = (7 + (int)aujourd_hui.DayOfWeek - (int)DayOfWeek.Monday) % 7;
            DateTime lundi = aujourd_hui.AddDays(-diff + (_semaineOffset * 7));
            for (int i = 0; i < 7; i++)
                Jours.Add(lundi.AddDays(i));

            LabelSemaine = $"{Jours[0]:dd/MM} – {Jours[6]:dd/MM/yyyy}";
            OnPropertyChanged(nameof(LabelSemaine));
            OnPropertyChanged(nameof(Jours));
        }

        public void SemainePrecedente()
        {
            SemaineOffset--;
            int id = Session.EstAdmin ? IdUtilisateurSelectionne : Session.IdUtilisateur;
            ChargerAgenda(id);
        }

        public void SemaineSuivante()
        {
            SemaineOffset++;
            int id = Session.EstAdmin ? IdUtilisateurSelectionne : Session.IdUtilisateur;
            ChargerAgenda(id);
        }

        public void ChargerAgenda(int idUtilisateur)
        {
            var dispos = _service.GetDisposSemaine(idUtilisateur, Jours[0], Jours[6]);
            Creneaux.Clear();

            foreach (var jour in Jours)
            {
                foreach (var heure in Heures)
                {
                    var dispo = dispos.FirstOrDefault(d =>
                        d.Date.Date == jour.Date && d.Heure == heure);

                    Creneaux.Add(new CreneauAgenda
                    {
                        Date = jour,
                        Heure = heure,
                        Present = dispo?.Present ?? false
                    });
                }
            }
        }

        public void ToggleCreneau(DateTime date, int heure, bool present)
        {
            int idUser = Session.EstAdmin ? IdUtilisateurSelectionne : Session.IdUtilisateur;
            _service.ToggleDispo(idUser, date, heure, present);

            var creneau = Creneaux.FirstOrDefault(c =>
                c.Date.Date == date.Date && c.Heure == heure);
            if (creneau != null)
                creneau.Present = present;
        }
    }
}