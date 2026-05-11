using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RadicalFeedbackApp.Helpers;
using RadicalFeedbackApp.Models;
using RadicalFeedbackApp.ViewModels;
using System;

namespace RadicalFeedbackApp.Views
{
    public sealed partial class AvisPage : Page
    {
        private AvisViewModel _vm;

        public AvisPage()
        {
            this.InitializeComponent();
            _vm = (AvisViewModel)this.DataContext;

            if (Session.EstAdmin)
            {
                ExpertCombo.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                foreach (var e in _vm.Experts)
                    ExpertCombo.Items.Add($"{e.prenom} {e.nom}");

                if (ExpertCombo.Items.Count > 0)
                    ExpertCombo.SelectedIndex = 0;
            }
        }

        private void ExpertCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _vm.ExpertSelectionneIndex = ExpertCombo.SelectedIndex;
        }

        private async void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            var avis = (Avis)((Button)sender).Tag;
            var dialog = new ContentDialog
            {
                Title = "Supprimer l'avis",
                Content = $"Supprimer l'avis \"{avis.Titre}\" ? Cette action est irréversible.",
                PrimaryButtonText = "Supprimer",
                CloseButtonText = "Annuler",
                XamlRoot = this.XamlRoot
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
                _vm.Supprimer(avis.Id);
        }
    }
}