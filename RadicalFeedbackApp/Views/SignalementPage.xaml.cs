using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RadicalFeedbackApp.Data;
using RadicalFeedbackApp.Models;
using RadicalFeedbackApp.ViewModels;
using System;

namespace RadicalFeedbackApp.Views
{
    public sealed partial class SignalementPage : Page
    {
        private SignalementService _service = new SignalementService();

        public SignalementPage()
        {
            this.InitializeComponent();
        }

        private void BtnDetail_Click(object sender, RoutedEventArgs e)
        {
            var s = (Signalement)((Button)sender).Tag;

            DetailTitre.Text = s.EstSignalementExpert ? "EXPERT SIGNALÉ" : "CONVERSATION SIGNALÉE";
            DetailCategorie.Text = s.Categorie;
            DetailAuteur.Text = $"{s.PrenomUtilisateur} {s.NomUtilisateur}";
            DetailCible.Text = s.Cible;
            DetailCommentaire.Text = string.IsNullOrEmpty(s.Commentaire)
                ? "Aucun commentaire." : s.Commentaire;

            // Si c'est une conversation, charge les messages
            if (!s.EstSignalementExpert && s.IdConversation.HasValue)
            {
                var messages = _service.GetMessagesConversation(s.IdConversation.Value);
                MessagesList.ItemsSource = messages;
                ConversationPanel.Visibility = Visibility.Visible;
            }
            else
            {
                ConversationPanel.Visibility = Visibility.Collapsed;
            }

            DetailColumn.Width = new GridLength(380);
        }

        private void BtnFermerDetail_Click(object sender, RoutedEventArgs e)
        {
            DetailColumn.Width = new GridLength(0);
        }

        private async void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            var s = (Signalement)((Button)sender).Tag;
            var dialog = new ContentDialog
            {
                Title = "Supprimer le signalement",
                Content = $"Supprimer ce signalement de {s.PrenomUtilisateur} {s.NomUtilisateur} ?",
                PrimaryButtonText = "Supprimer",
                CloseButtonText = "Annuler",
                XamlRoot = this.XamlRoot
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var vm = (SignalementViewModel)this.DataContext;
                vm.Supprimer(s.Id);
                DetailColumn.Width = new GridLength(0);
            }
        }
    }
}