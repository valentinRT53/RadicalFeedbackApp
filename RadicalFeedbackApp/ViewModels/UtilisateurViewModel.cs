using CommunityToolkit.Mvvm.ComponentModel;
using RadicalFeedbackApp.Models;
using RadicalFeedbackApp.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace RadicalFeedbackApp.ViewModels
{
    public class UtilisateurViewModel : ObservableObject
    {
        private UtilisateurService _service = new UtilisateurService();
        private List<Utilisateur> _tousLesUtilisateurs = new();

        public ObservableCollection<Utilisateur> Utilisateurs { get; set; } = new();

        private Utilisateur _utilisateurSelectionne;
        public Utilisateur UtilisateurSelectionne
        {
            get => _utilisateurSelectionne;
            set => SetProperty(ref _utilisateurSelectionne, value);
        }

        private string _recherche = "";
        public string Recherche
        {
            get => _recherche;
            set
            {
                SetProperty(ref _recherche, value);
                AppliquerFiltres();
            }
        }

        private int _triSelectionne = 0;
        public int TriSelectionne
        {
            get => _triSelectionne;
            set
            {
                SetProperty(ref _triSelectionne, value);
                AppliquerFiltres();
            }
        }

        private int _roleFiltre = 0;
        public int RoleFiltre
        {
            get => _roleFiltre;
            set
            {
                SetProperty(ref _roleFiltre, value);
                AppliquerFiltres();
            }
        }

        public UtilisateurViewModel()
        {
            LoadUtilisateurs();
        }

        public void LoadUtilisateurs()
        {
            _tousLesUtilisateurs = _service.GetAllUtilisateurs();
            AppliquerFiltres();
        }

        private void AppliquerFiltres()
        {
            var filtre = _tousLesUtilisateurs.AsEnumerable();

            // Filtre recherche
            if (!string.IsNullOrWhiteSpace(_recherche))
            {
                string r = _recherche.ToLower();
                filtre = filtre.Where(u =>
                    u.Nom.ToLower().Contains(r) ||
                    u.Prenom.ToLower().Contains(r) ||
                    u.Email.ToLower().Contains(r) ||
                    u.Ville.ToLower().Contains(r));
            }

            // Filtre rôle
            filtre = _roleFiltre switch
            {
                1 => filtre.Where(u => u.Role == "Admin"),
                2 => filtre.Where(u => u.Role == "Expert"),
                3 => filtre.Where(u => u.Role == "Utilisateur"),
                _ => filtre
            };

            // Tri
            filtre = _triSelectionne switch
            {
                1 => filtre.OrderBy(u => u.Nom),
                2 => filtre.OrderBy(u => u.Prenom),
                3 => filtre.OrderBy(u => u.Role),
                4 => filtre.OrderBy(u => u.Ville),
                _ => filtre.OrderBy(u => u.Id)
            };

            Utilisateurs.Clear();
            foreach (var u in filtre)
                Utilisateurs.Add(u);
        }

        public void Ajouter(Utilisateur u, string login, string mdp, int idRole)
        {
            _service.AjouterUtilisateur(u, login, mdp, idRole);
            LoadUtilisateurs();
        }

        public void Modifier(Utilisateur u)
        {
            _service.ModifierUtilisateur(u);
            LoadUtilisateurs();
        }

        public void Supprimer(int id)
        {
            _service.SupprimerUtilisateur(id);
            LoadUtilisateurs();
        }

        public void ReinitialiseMdp(int id, string nouveauMdp)
        {
            _service.ReinitialiseMdp(id, nouveauMdp);
        }
    }
}