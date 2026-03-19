using Microsoft.UI.Xaml;
using RadicalFeedbackApp.Views;

namespace RadicalFeedbackApp
{
    public partial class App : Application
    {
        private Window? _window;

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new LoginWindow();
            _window.Activate();

            // Plein écran
            var appWindow = _window.AppWindow;
            appWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.FullScreen);
        }
    }
}