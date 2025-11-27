using M_Hike.Database;
using M_Hike.Hikes;
using M_Hike.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace M_Hike
{
    public partial class HikeList : ContentPage
    {
        private readonly HikeSQLiteDatabase _databaseHelper;
        public ObservableCollection<Hike> Hikes { get; set; } = [];

        public HikeList(HikeSQLiteDatabase databaseHelper)
        {
            InitializeComponent();
            _databaseHelper = databaseHelper;
            hikeCollectionView.ItemsSource = Hikes;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            PopulateHikesData();
        }

        // Populate the ObservableCollection with hikes from the database
        private void PopulateHikesData()
        {
            foreach (var hike in Hikes)
            {
                hike.PropertyChanged -= Hike_PropertyChanged;
            }
            Hikes.Clear();
            var hikesFromDb = _databaseHelper.GetAllHikes();
            foreach (var hike in hikesFromDb)
            {
                hike.PropertyChanged += Hike_PropertyChanged;
                Hikes.Add(hike);
            }
            UpdateSelectLabel();
        }

        private void Hike_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Hike.IsSelected))
            {
                UpdateSelectLabel();
            }
        }

        // Update the selection label
        private void UpdateSelectLabel()
        {
            // Disable the button if there are no hikes
            if (Hikes.Count == 0)
            {
                selectAllButton.Text = "Select All";
                selectAllButton.IsEnabled = false;
                return;
            }

            // Dynamically switches between selection types
            if (Hikes.All(h => h.IsSelected))
            {
                selectAllButton.Text = "Deselect All";
            }
            else
            {
                selectAllButton.Text = "Select All";
            }
        }

        // Switch to the hike edit menu
        private void AddButton_Clicked(object sender, System.EventArgs e)
        {
            Shell.Current.GoToAsync(nameof(HikeEdit));
        }

        // Select all checkboxes
        private void SelectAll_Clicked(object sender, EventArgs e)
        {
            bool shouldSelectAll = Hikes.Any(h => !h.IsSelected);
            foreach (var hike in Hikes)
            {
                hike.IsSelected = shouldSelectAll;
            }
        }

        // Delete selected hikes
        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            // Do nothing if no hikes are selected
            if (!Hikes.Any(h => h.IsSelected))
            {
                return;
            }

            // Display a confirmation dialog first
            bool confirm = await DisplayAlert("Confirm Deletion", "Are you sure you want to delete the selected hikes?", "Yes", "No");
            if (!confirm)
            {
                return;
            }

            var selectedHikes = Hikes.Where(h => h.IsSelected).ToList();
            foreach (var hike in selectedHikes)
            {
                _databaseHelper.DeleteHikeById(hike.Id);
                Hikes.Remove(hike);
            }

            UpdateSelectLabel();
        }
    }
}
