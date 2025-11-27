using M_Hike.Database;
using M_Hike.Models;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace M_Hike.Hikes;

[QueryProperty(nameof(HikeId), "hikeId")]
public partial class HikeEdit : ContentPage
{
    private readonly HikeSQLiteDatabase _databaseHelper;

    // Load hike details for editing if applicable
    private Hike _hike;
    public string HikeId
    {
        set
        {
            LoadHike(value);
        }
    }

    // Initialise members and gears
    public ObservableCollection<StringItem> HikeMembers { get; set; }
    public ObservableCollection<StringItem> HikeGears { get; set; }

    public HikeEdit(HikeSQLiteDatabase databaseHelper)
    {
        InitializeComponent();

        _databaseHelper = databaseHelper;
        _hike = new Hike();
        BindingContext = _hike;

        // Set default values
        hikeParkingAvailableButton.IsChecked = true;

        HikeMembers = new ObservableCollection<StringItem>();
        HikeGears = new ObservableCollection<StringItem>();

        BindableLayout.SetItemsSource(hikeMembersContainer, HikeMembers);
        BindableLayout.SetItemsSource(gearsContainer, HikeGears);
    }

    // Load hike details from the database if in edit mode
    private void LoadHike(string hikeId)
    {
        if (int.TryParse(hikeId, out int id))
        {
            _hike = _databaseHelper.GetHikeById(id);
                Console.WriteLine("Loaded Hike: " + _hike.Id);
            if (_hike != null)
            {
                BindingContext = _hike;
                
                // For some reason binding this to the radio buttons 
                // is more complicated than I thought, maybe I'm missing something
                if (_hike.ParkingAvailability)
                {
                    hikeParkingAvailableButton.IsChecked = true;
                }
                else
                {
                    hikeParkingUnavailableButton.IsChecked = true;
                }


                // Populate Members
                HikeMembers.Clear();
                if (!string.IsNullOrEmpty(_hike.Members))
                {
                    var members = _hike.Members.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var member in members)
                    {
                        HikeMembers.Add(new StringItem { Value = member.Trim() });
                    }
                }

                // Populate Gear
                HikeGears.Clear();
                if (!string.IsNullOrEmpty(_hike.Gear))
                {
                    var gears = _hike.Gear.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var gear in gears)
                    {
                        HikeGears.Add(new StringItem { Value = gear.Trim() });
                    }
                }
            }
        }
    }

    private void AddHikeMember_Clicked(object sender, EventArgs e)
    {
        HikeMembers.Add(new StringItem { Value = "" });
    }

    private void DeleteHikeMember_Clicked(object sender, EventArgs e)
    {
        if (((ImageButton)sender).CommandParameter is StringItem member)
        {
            HikeMembers.Remove(member);
        }
    }

    private void AddGear_Clicked(object sender, EventArgs e)
    {
        HikeGears.Add(new StringItem { Value = "" });
    }

    private void DeleteGear_Clicked(object sender, EventArgs e)
    {
        if (((ImageButton)sender).CommandParameter is StringItem gear)
        {
            HikeGears.Remove(gear);
        }
    }

    // Save the hike details
    private void SaveButton_Clicked(object sender, EventArgs e)
    {
        if (ValidateFields())
        {
            _ = PerformActualSave();
        }
       
    }

    // Validate fields
    // Check for required fields and invalid data
    private bool ValidateFields()
    {
        // Check if name is alpha-numeric
        if (string.IsNullOrWhiteSpace(editName.Text))
        {
            DisplayAlert("Validation Error", "Name of hike is required.", "OK");
            return false;
        }
        if (!Regex.IsMatch(editName.Text, @"^[a-zA-Z0-9 ]+$"))
        {
            DisplayAlert("Validation Error", "Name of hike must be alphanumeric.", "OK");
            return false;
        }

        if (string.IsNullOrWhiteSpace(editLocation.Text))
        {
            DisplayAlert("Validation Error", "Location is required.", "OK");
            return false;
        }

        if (string.IsNullOrWhiteSpace(editLength.Text))
        {
            DisplayAlert("Validation Error", "Length of the hike is required.", "OK");
            return false;
        }

        return true;
    }

    // Displays all the hike details for the user to check once again before 
    private async Task PerformActualSave()
    {
        bool saveConfirmed = await DisplayAlert("Are you sure you want to save this hike?",
            "Name: " + editName.Text + "\n" +
            "Location: " + editLocation.Text + "\n" +
            "Length: " + editLength.Text + "\n" +
            "Date: " + editDate.Date.ToString("d") + "\n" +
            "Difficulty: " + Math.Floor(difficultySlider.Value) + "\n" +
            "Parking Available: " + (hikeParkingAvailableButton.IsChecked ? "Yes" : "No") + "\n" +
            "Members: " + string.Join(", ", HikeMembers.Select(m => m.Value)) + "\n" +
            "Gears: " + string.Join(", ", HikeGears.Select(g => g.Value)) + "\n"+
            "Description: " + editDescription.Text + "\n",
            "Save", "Cancel");

        // Insert the hike details if confirmed by the user
        if (saveConfirmed)
        {
            Hike newHike = new Hike
            {
                Name = editName.Text,
                Location = editLocation.Text,
                Length = double.Parse(editLength.Text),
                Date = editDate.Date,
                Difficulty = (int)(Math.Floor(difficultySlider.Value)),
                ParkingAvailability = hikeParkingAvailableButton.IsChecked,
                Members = string.Join(", ", HikeMembers.Select(m => m.Value)),
                Gear = string.Join(", ", HikeGears.Select(g => g.Value)),
                Description = editDescription.Text
            };
            if (_hike.Id == 0)
            {
                _databaseHelper.InsertHike(newHike);
            }
            else
            {
                newHike.Id = _hike.Id;
                _databaseHelper.UpdateHike(newHike);
            }
            await DisplayAlert("Success", "Hike details saved successfully!", "OK");

            // Go back to the previous page
            await Navigation.PopAsync();
        }
    }
}

// Helper class to allow binding to a string in a collection.
public class StringItem
{
    public string? Value { get; set; }
}