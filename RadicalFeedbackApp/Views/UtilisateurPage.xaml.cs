using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RadicalFeedbackApp.Data;
using RadicalFeedbackApp.Models;
using RadicalFeedbackApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RadicalFeedbackApp.Views
{
    public sealed partial class UtilisateurPage : Page
    {
        private UtilisateurViewModel _vm;
        private SpecialisationService _specService = new SpecialisationService();
        private List<int> _specSelectionnees = new();

        private enum ModePanel { Ajouter, Modifier, ReinitialiseMdp }
        private ModePanel _mode;
        private Utilisateur _utilisateurEnCours;

        public UtilisateurPage()
        {
            this.InitializeComponent();
            _vm = (UtilisateurViewModel)this.DataContext;
        }

        // ─── OUVRIR / FERMER ───

        private void OuvrirPanel(int largeur)
        {
            PanelColumn.Width = new GridLength(largeur);
        }

        private void BtnFermerPanel_Click(object sender, RoutedEventArgs e)
        {
            PanelColumn.Width = new GridLength(0);
            PanelErreur.Visibility = Visibility.Collapsed;
        }

        // ─── AJOUTER ───

        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            _mode = ModePanel.Ajouter;
            _utilisateurEnCours = null;
            PanelTitre.Text = "AJOUTER UN UTILISATEUR";
            BtnValider.Content = "CRÉER L'UTILISATEUR";

            ChampLoginPanel.Visibility = Visibility.Visible;
            ChampMdpPanel.Visibility = Visibility.Visible;
            ChampInfosPanel.Visibility = Visibility.Visible;
            ChampSpecialisationsPanel.Visibility = Visibility.Collapsed;

            ChampLogin.Text = "";
            ChampMdp.Password = "";
            ChampMdpConfirm.Password = "";
            ChampNom.Text = "";
            ChampPrenom.Text = "";
            ChampEmail.Text = "";
            ChampVille.Text = "";
            ChampStatut.SelectedIndex = 0;
            PanelErreur.Visibility = Visibility.Collapsed;

            OuvrirPanel(340);
        }

        // ─── MODIFIER ───

        private void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            _mode = ModePanel.Modifier;
            _utilisateurEnCours = (Utilisateur)((Button)sender).Tag;
            PanelTitre.Text = "MODIFIER L'UTILISATEUR";
            BtnValider.Content = "ENREGISTRER";

            ChampLoginPanel.Visibility = Visibility.Collapsed;
            ChampMdpPanel.Visibility = Visibility.Collapsed;
            ChampInfosPanel.Visibility = Visibility.Visible;
            PanelErreur.Visibility = Visibility.Collapsed;

            ChampNom.Text = _utilisateurEnCours.Nom;
            ChampPrenom.Text = _utilisateurEnCours.Prenom;
            ChampEmail.Text = _utilisateurEnCours.Email;
            ChampVille.Text = _utilisateurEnCours.Ville;
            LabelIdUtilisateur.Text = _utilisateurEnCours.Id.ToString();
            LabelIdAbonnement.Text = _utilisateurEnCours.IdAbonnement.ToString();
            ChampStatut.SelectedIndex = _utilisateurEnCours.Statut == "Actif" ? 0 : 1;

            // Spécialisations uniquement pour les experts
            if (_utilisateurEnCours.Role == "Expert")
            {
                ChampSpecialisationsPanel.Visibility = Visibility.Visible;
                ChargerSpecialisations(_utilisateurEnCours.Id);
            }
            else
            {
                ChampSpecialisationsPanel.Visibility = Visibility.Collapsed;
            }

            OuvrirPanel(340);
        }

        private void ChargerSpecialisations(int idExpert)
        {
            var toutes = _specService.GetAll();
            var expertSpecs = _specService.GetByExpert(idExpert)
                .Select(s => s.Id).ToHashSet();

            _specSelectionnees = expertSpecs.ToList();

            // Ajoute les checkboxes
            SpecialisationsList.ItemsSource = null;
            SpecialisationsList.ItemsSource = toutes;

            // Coche les spécialisations existantes après le rendu
            SpecialisationsList.Loaded += (s, e) =>
            {
                var panel = SpecialisationsList.ItemsPanelRoot;
                if (panel == null) return;
                foreach (var item in panel.Children)
                {
                    if (item is CheckBox cb && cb.Tag is Specialisation spec)
                        cb.IsChecked = expertSpecs.Contains(spec.Id);
                }
            };
        }

        private void SpecCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var spec = (Specialisation)cb.Tag;

            if (cb.IsChecked == true)
            {
                if (!_specSelectionnees.Contains(spec.Id))
                    _specSelectionnees.Add(spec.Id);
            }
            else
            {
                _specSelectionnees.Remove(spec.Id);
            }
        }

        // ─── MDP ───

        private void BtnMdp_Click(object sender, RoutedEventArgs e)
        {
            _mode = ModePanel.ReinitialiseMdp;
            _utilisateurEnCours = (Utilisateur)((Button)sender).Tag;
            PanelTitre.Text = "RÉINITIALISER MDP";
            BtnValider.Content = "RÉINITIALISER";

            ChampLoginPanel.Visibility = Visibility.Collapsed;
            ChampMdpPanel.Visibility = Visibility.Visible;
            ChampInfosPanel.Visibility = Visibility.Collapsed;
            ChampSpecialisationsPanel.Visibility = Visibility.Collapsed;

            ChampMdp.Password = "";
            ChampMdpConfirm.Password = "";
            PanelErreur.Visibility = Visibility.Collapsed;

            OuvrirPanel(340);
        }

        // ─── SUPPRIMER ───

        private async void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            var u = (Utilisateur)((Button)sender).Tag;
            var dialog = new ContentDialog
            {
                Title = "Supprimer l'utilisateur",
                Content = $"Supprimer {u.Prenom} {u.Nom} ? Cette action est irréversible.",
                PrimaryButtonText = "Supprimer",
                CloseButtonText = "Annuler",
                XamlRoot = this.XamlRoot
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
                _vm.Supprimer(u.Id);
        }

        // ─── VALIDER ───

        private void BtnValider_Click(object sender, RoutedEventArgs e)
        {
            PanelErreur.Visibility = Visibility.Collapsed;

            if (_mode == ModePanel.Ajouter)
            {
                if (string.IsNullOrWhiteSpace(ChampLogin.Text) ||
                    string.IsNullOrWhiteSpace(ChampMdp.Password) ||
                    string.IsNullOrWhiteSpace(ChampNom.Text) ||
                    string.IsNullOrWhiteSpace(ChampPrenom.Text))
                {
                    PanelErreur.Text = "Veuillez remplir tous les champs obligatoires.";
                    PanelErreur.Visibility = Visibility.Visible;
                    return;
                }

                if (ChampMdp.Password != ChampMdpConfirm.Password)
                {
                    PanelErreur.Text = "Les mots de passe ne correspondent pas.";
                    PanelErreur.Visibility = Visibility.Visible;
                    return;
                }

                var u = new Utilisateur
                {
                    Nom = ChampNom.Text.Trim(),
                    Prenom = ChampPrenom.Text.Trim(),
                    Email = ChampEmail.Text.Trim(),
                    Ville = ChampVille.Text.Trim(),
                    Statut = ChampStatut.SelectedIndex == 0 ? "Actif" : "Inactif",
                    IdAbonnement = 1
                };

                _vm.Ajouter(u, ChampLogin.Text.Trim(), ChampMdp.Password);
                PanelColumn.Width = new GridLength(0);
            }
            else if (_mode == ModePanel.Modifier)
            {
                _utilisateurEnCours.Nom = ChampNom.Text.Trim();
                _utilisateurEnCours.Prenom = ChampPrenom.Text.Trim();
                _utilisateurEnCours.Email = ChampEmail.Text.Trim();
                _utilisateurEnCours.Ville = ChampVille.Text.Trim();
                _utilisateurEnCours.Statut = ChampStatut.SelectedIndex == 0 ? "Actif" : "Inactif";

                _vm.Modifier(_utilisateurEnCours);

                // Sauvegarde les spécialisations si expert
                if (_utilisateurEnCours.Role == "Expert")
                    _specService.SetSpecialisationsExpert(_utilisateurEnCours.Id, _specSelectionnees);

                PanelColumn.Width = new GridLength(0);
            }
            else if (_mode == ModePanel.ReinitialiseMdp)
            {
                if (string.IsNullOrWhiteSpace(ChampMdp.Password))
                {
                    PanelErreur.Text = "Veuillez entrer un nouveau mot de passe.";
                    PanelErreur.Visibility = Visibility.Visible;
                    return;
                }

                if (ChampMdp.Password != ChampMdpConfirm.Password)
                {
                    PanelErreur.Text = "Les mots de passe ne correspondent pas.";
                    PanelErreur.Visibility = Visibility.Visible;
                    return;
                }

                _vm.ReinitialiseMdp(_utilisateurEnCours.Id, ChampMdp.Password);
                PanelColumn.Width = new GridLength(0);
            }
        }
    }
}