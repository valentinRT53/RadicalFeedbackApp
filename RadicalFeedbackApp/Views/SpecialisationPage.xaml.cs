using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RadicalFeedbackApp.Models;
using RadicalFeedbackApp.ViewModels;
using System;

namespace RadicalFeedbackApp.Views
{
    public sealed partial class SpecialisationPage : Page
    {
        private SpecialisationViewModel _vm;
        private bool _modeAjout;
        private Specialisation _enCours;

        public SpecialisationPage()
        {
            this.InitializeComponent();
            _vm = (SpecialisationViewModel)this.DataContext;
        }

        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            _modeAjout = true;
            _enCours = null;
            PanelTitre.Text = "AJOUTER";
            BtnValider.Content = "CRÉER";
            ChampNom.Text = "";
            ChampDescription.Text = "";
            PanelErreur.Visibility = Visibility.Collapsed;
            PanelColumn.Width = new GridLength(340);
        }

        private void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            _modeAjout = false;
            _enCours = (Specialisation)((Button)sender).Tag;
            PanelTitre.Text = "MODIFIER";
            BtnValider.Content = "ENREGISTRER";
            ChampNom.Text = _enCours.Nom;
            ChampDescription.Text = _enCours.Description;
            PanelErreur.Visibility = Visibility.Collapsed;
            PanelColumn.Width = new GridLength(340);
        }

        private async void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            var s = (Specialisation)((Button)sender).Tag;
            var dialog = new ContentDialog
            {
                Title = "Supprimer la spécialisation",
                Content = $"Supprimer '{s.Nom}' ? Les experts liés perdront cette spécialisation.",
                PrimaryButtonText = "Supprimer",
                CloseButtonText = "Annuler",
                XamlRoot = this.XamlRoot
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
                _vm.Supprimer(s.Id);
        }

        private void BtnFermer_Click(object sender, RoutedEventArgs e)
        {
            PanelColumn.Width = new GridLength(0);
        }

        private void BtnValider_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ChampNom.Text))
            {
                PanelErreur.Text = "Le nom est obligatoire.";
                PanelErreur.Visibility = Visibility.Visible;
                return;
            }

            if (_modeAjout)
            {
                _vm.Ajouter(new Specialisation
                {
                    Nom = ChampNom.Text.Trim(),
                    Description = ChampDescription.Text.Trim()
                });
            }
            else
            {
                _enCours.Nom = ChampNom.Text.Trim();
                _enCours.Description = ChampDescription.Text.Trim();
                _vm.Modifier(_enCours);
            }

            PanelColumn.Width = new GridLength(0);
        }
    }
}