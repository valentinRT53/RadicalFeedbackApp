using Microsoft.UI.Xaml.Controls;
using RadicalFeedbackApp.Helpers;
using RadicalFeedbackApp.ViewModels;

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
    }
}