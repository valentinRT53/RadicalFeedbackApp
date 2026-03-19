using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RadicalFeedbackApp.Data;
using RadicalFeedbackApp.Models;
using RadicalFeedbackApp.ViewModels;
using System;

namespace RadicalFeedbackApp.Views
{
    public sealed partial class AbonnementPage : Page
    {
        private AbonnementViewModel _vm;
        private AbonnementService _service = new AbonnementService();

        public AbonnementPage()
        {
            this.InitializeComponent();
            _vm = (AbonnementViewModel)this.DataContext;
        }

        // ─── AJOUTER ───

        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            PanelTitre.Text = "NOUVEL ABONNEMENT";
            FormulairePanel.Visibility = Visibility.Visible;
            AbonnesPanel.Visibility = Visibility.Collapsed;
            BtnValiderBorder.Visibility = Visibility.Visible;
            ChampPrix.Text = "";
            ChampDate.Date = DateTime.Today;
            PanelErreur.Visibility = Visibility.Collapsed;
            PanelColumn.Width = new GridLength(340);
        }

        // ─── VOIR ABONNÉS ───

        private void BtnVoirAbonnes_Click(object sender, RoutedEventArgs e)
        {
            var abo = (Abonnement)((Button)sender).Tag;
            PanelTitre.Text = $"ABONNEMENT {abo.Prix}€";
            FormulairePanel.Visibility = Visibility.Collapsed;
            AbonnesPanel.Visibility = Visibility.Visible;
            BtnValiderBorder.Visibility = Visibility.Collapsed;

            var users = _service.GetUtilisateursByAbonnement(abo.Id);
            AbonnesList.ItemsSource = users;
            AbonnesCount.Text = $"{users.Count} abonné(s)";

            PanelColumn.Width = new GridLength(380);
        }

        // ─── SUPPRIMER ───

        private async void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            var abo = (Abonnement)((Button)sender).Tag;
            var dialog = new ContentDialog
            {
                Title = "Supprimer l'abonnement",
                Content = $"Supprimer l'abonnement à {abo.Prix}€ ? Les utilisateurs liés seront désassociés.",
                PrimaryButtonText = "Supprimer",
                CloseButtonText = "Annuler",
                XamlRoot = this.XamlRoot
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                _vm.Supprimer(abo.Id);
                PanelColumn.Width = new GridLength(0);
            }
        }

        // ─── FERMER ───

        private void BtnFermer_Click(object sender, RoutedEventArgs e)
        {
            PanelColumn.Width = new GridLength(0);
        }

        // ─── VALIDER ───

        private void BtnValider_Click(object sender, RoutedEventArgs e)
        {
            PanelErreur.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(ChampPrix.Text) || ChampDate.Date == null)
            {
                PanelErreur.Text = "Veuillez remplir tous les champs.";
                PanelErreur.Visibility = Visibility.Visible;
                return;
            }

            if (!double.TryParse(ChampPrix.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out double prix))
            {
                PanelErreur.Text = "Le prix doit être un nombre valide.";
                PanelErreur.Visibility = Visibility.Visible;
                return;
            }

            _vm.Ajouter(new Abonnement
            {
                Prix = prix,
                DateSouscription = ChampDate.Date.Value.DateTime
            });

            PanelColumn.Width = new GridLength(0);
        }
    }
}