using RadicalFeedbackApp.Views;
using RadicalFeedbackApp.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace RadicalFeedbackApp
{
    public sealed partial class MainWindow : Window
    {
        private Button _activeNavItem;

        public MainWindow()
        {
            this.InitializeComponent();
            InitialiserUI();
            AppliquerFiltreMenu();

            if (Session.EstAdmin)
                NaviguerVers(NavUtilisateurs);
            else
                NaviguerVers(NavAvis); // Expert atterrit sur ses avis
        }

        private void InitialiserUI()
        {
            RoleLabel.Text = Session.Role.ToUpper();
            UserNameText.Text = Session.Login;
            UserRoleText.Text = Session.Role;
            AvatarText.Text = Session.Login.Length >= 2
                ? Session.Login.Substring(0, 2).ToUpper()
                : Session.Login.ToUpper();
        }

        private void AppliquerFiltreMenu()
        {
            if (!Session.EstAdmin)
            {
                NavUtilisateurs.Visibility = Visibility.Collapsed;
                NavAbonnements.Visibility = Visibility.Collapsed;
                NavSpecialisations.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnQuitter_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void NavItem_Click(object sender, RoutedEventArgs e)
        {
            NaviguerVers(sender as Button);
        }

        private void NaviguerVers(Button btn)
        {
            if (btn == null) return;

            // Reset style ancien item actif
            if (_activeNavItem != null)
            {
                _activeNavItem.Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent);
                _activeNavItem.Foreground = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 160, 160, 176));
            }

            // Appliquer style actif
            btn.Background = new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 24, 24, 29));
            btn.Foreground = new SolidColorBrush(Microsoft.UI.Colors.White);
            _activeNavItem = btn;

            // Navigation
            switch (btn.Tag.ToString())
            {
                case "utilisateurs":
                    ContentFrame.Navigate(typeof(UtilisateurPage));
                    break;
                case "avis":
                    ContentFrame.Navigate(typeof(AvisPage));
                    break;
                case "signalements":
                    ContentFrame.Navigate(typeof(SignalementPage));
                    break;
                case "abonnements":
                    ContentFrame.Navigate(typeof(AbonnementPage));
                    break;
                
                case "agenda":
                    ContentFrame.Navigate(typeof(AgendaPage));
                    break;
                case "specialisations":
                    ContentFrame.Navigate(typeof(SpecialisationPage));
                    break;
            }

        }
    }
}