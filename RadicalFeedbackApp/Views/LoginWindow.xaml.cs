using Microsoft.UI.Xaml;
using RadicalFeedbackApp.Data;
using RadicalFeedbackApp.Helpers;

namespace RadicalFeedbackApp.Views
{
    public sealed partial class LoginWindow : Window
    {
        private ConnexionService _connexionService = new ConnexionService();

        public LoginWindow()
        {
            this.InitializeComponent();
        }

        private void ConnexionBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string mdp = MdpBox.Password.Trim();

            var (succes, idUtilisateur, role) = _connexionService.Authentifier(login, mdp);

            if (succes)
            {
                Session.IdUtilisateur = idUtilisateur;
                Session.Login = login;
                Session.Role = role;

                var mainWindow = new MainWindow();
                mainWindow.Activate();

                // Plein écran
                var appWindow = mainWindow.AppWindow;
                appWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.FullScreen);

                this.Close();
            }
            else
            {
                LoginBox.Text = "";
                MdpBox.Password = "";
                ErreurText.Visibility = Visibility.Visible;
            }
        }
    }
}