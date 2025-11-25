using M_Hike.Hikes;
using M_Hike.Models;
namespace M_Hike;

public partial class HikeListCollectionViewTemplate : ContentView
{
    public HikeListCollectionViewTemplate()
	{
		InitializeComponent();
    }

    // Use this hike for edit mode
    private async void EditButton_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is Hike hike)
        {
            Console.WriteLine($"Navigating to edit hike with ID: {hike.Id}");
            await Shell.Current.GoToAsync($"{nameof(HikeEdit)}?hikeId={hike.Id}");
        }
    }
}