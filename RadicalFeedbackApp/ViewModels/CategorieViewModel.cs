using CommunityToolkit.Mvvm.ComponentModel;
using RadicalFeedbackApp.Models;
using RadicalFeedbackApp.Data;
using System.Collections.ObjectModel;

namespace RadicalFeedbackApp.ViewModels
{
    public class CategorieViewModel : ObservableObject
    {
        private CategorieService service = new CategorieService();
        public ObservableCollection<Categorie> Categories { get; set; } = new ObservableCollection<Categorie>();

        public CategorieViewModel()
        {
            LoadCategories();
        }

        private void LoadCategories()
        {
            var list = service.GetAllCategories();
            Categories.Clear();
            foreach (var c in list)
                Categories.Add(c);
        }
    }
}