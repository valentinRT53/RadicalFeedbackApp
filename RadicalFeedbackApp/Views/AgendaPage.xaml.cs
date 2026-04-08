using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using RadicalFeedbackApp.Helpers;
using RadicalFeedbackApp.ViewModels;
using System;
using System.Linq;
using Windows.UI;

namespace RadicalFeedbackApp.Views
{
    public sealed partial class AgendaPage : Page
    {
        private AgendaViewModel _vm;

        public AgendaPage()
        {
            this.InitializeComponent();
            _vm = (AgendaViewModel)this.DataContext;

            LabelSemaine.Text = _vm.LabelSemaine;

            if (Session.EstAdmin)
            {
                UtilisateurCombo.Visibility = Visibility.Visible;
                foreach (var u in _vm.Utilisateurs)
                    UtilisateurCombo.Items.Add(u.login);

                if (UtilisateurCombo.Items.Count > 0)
                    UtilisateurCombo.SelectedIndex = 0;
            }

            BuildAgendaGrid();
        }

        private void BtnPrecedent_Click(object sender, RoutedEventArgs e)
        {
            _vm.SemainePrecedente();
            LabelSemaine.Text = _vm.LabelSemaine;
            BuildAgendaGrid();
        }

        private void BtnSuivant_Click(object sender, RoutedEventArgs e)
        {
            _vm.SemaineSuivante();
            LabelSemaine.Text = _vm.LabelSemaine;
            BuildAgendaGrid();
        }

        private void UtilisateurCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _vm.SelectedUtilisateurIndex = UtilisateurCombo.SelectedIndex;
            BuildAgendaGrid();
        }

        private void BuildAgendaGrid()
        {
            AgendaGrid.Children.Clear();
            AgendaGrid.ColumnDefinitions.Clear();
            AgendaGrid.RowDefinitions.Clear();

            AgendaGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
            for (int j = 0; j < 7; j++)
                AgendaGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            AgendaGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            for (int h = 0; h < _vm.Heures.Count; h++)
                AgendaGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Header des jours
            for (int j = 0; j < _vm.Jours.Count; j++)
            {
                var jour = _vm.Jours[j];
                bool estAujourdhui = jour.Date == DateTime.Today;

                var header = new Border
                {
                    Padding = new Thickness(8, 10, 8, 10),
                    BorderBrush = new SolidColorBrush(Color.FromArgb(255, 42, 42, 50)),
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    Background = estAujourdhui
                        ? new SolidColorBrush(Color.FromArgb(30, 232, 25, 44))
                        : new SolidColorBrush(Color.FromArgb(255, 17, 17, 20))
                };

                var sp = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
                sp.Children.Add(new TextBlock
                {
                    Text = jour.ToString("ddd").ToUpper(),
                    FontSize = 9,
                    CharacterSpacing = 200,
                    Foreground = estAujourdhui
                        ? new SolidColorBrush(Color.FromArgb(255, 232, 25, 44))
                        : new SolidColorBrush(Color.FromArgb(255, 107, 107, 122))
                });
                sp.Children.Add(new TextBlock
                {
                    Text = jour.ToString("dd/MM"),
                    FontSize = 13,
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 240, 242)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 4, 0, 0)
                });

                header.Child = sp;
                Grid.SetColumn(header, j + 1);
                Grid.SetRow(header, 0);
                AgendaGrid.Children.Add(header);
            }

            // Lignes d'heures + cases
            for (int h = 0; h < _vm.Heures.Count; h++)
            {
                int heure = _vm.Heures[h];

                var heureLabel = new Border
                {
                    Padding = new Thickness(8, 12, 8, 12),
                    BorderBrush = new SolidColorBrush(Color.FromArgb(255, 42, 42, 50)),
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    Background = new SolidColorBrush(Color.FromArgb(255, 17, 17, 20))
                };
                heureLabel.Child = new TextBlock
                {
                    Text = $"{heure}h",
                    FontSize = 11,
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 107, 107, 122)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(heureLabel, 0);
                Grid.SetRow(heureLabel, h + 1);
                AgendaGrid.Children.Add(heureLabel);

                for (int j = 0; j < _vm.Jours.Count; j++)
                {
                    var jour = _vm.Jours[j];
                    var creneau = _vm.Creneaux.FirstOrDefault(c =>
                        c.Date.Date == jour.Date && c.Heure == heure);

                    bool present = creneau?.Present ?? false;
                    bool readOnly = Session.EstAdmin;

                    var cell = new Border
                    {
                        BorderBrush = new SolidColorBrush(Color.FromArgb(255, 42, 42, 50)),
                        BorderThickness = new Thickness(0, 0, 1, 1),
                        Background = present
                            ? new SolidColorBrush(Color.FromArgb(255, 30, 8, 12))
                            : new SolidColorBrush(Color.FromArgb(255, 10, 10, 11)),
                        Padding = new Thickness(8, 12, 8, 12),
                        Tag = (jour, heure)
                    };

                    if (present)
                    {
                        cell.Child = new Border
                        {
                            Width = 10,
                            Height = 10,
                            CornerRadius = new CornerRadius(5),
                            Background = new SolidColorBrush(Color.FromArgb(255, 232, 25, 44)),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                    }

                    if (!readOnly)
                    {
                        cell.PointerPressed += (s, e) =>
                        {
                            var b = (Border)s;
                            var (d, hr) = ((DateTime, int))b.Tag;
                            bool newPresent = !(creneau?.Present ?? false);
                            _vm.ToggleCreneau(d, hr, newPresent);
                            BuildAgendaGrid();
                        };

                        cell.PointerEntered += (s, e) =>
                        {
                            var b = (Border)s;
                            if (b.Background is SolidColorBrush brush &&
                                brush.Color == Color.FromArgb(255, 10, 10, 11))
                                b.Background = new SolidColorBrush(Color.FromArgb(255, 20, 20, 26));
                        };

                        cell.PointerExited += (s, e) =>
                        {
                            var b = (Border)s;
                            bool isPresent = creneau?.Present ?? false;
                            b.Background = isPresent
                                ? new SolidColorBrush(Color.FromArgb(255, 30, 8, 12))
                                : new SolidColorBrush(Color.FromArgb(255, 10, 10, 11));
                        };
                    }

                    Grid.SetColumn(cell, j + 1);
                    Grid.SetRow(cell, h + 1);
                    AgendaGrid.Children.Add(cell);
                }
            }
        }
    }
}