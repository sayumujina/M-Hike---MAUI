using M_Hike.Hikes;
using Microsoft.Maui.Controls; 

namespace M_Hike
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(HikeEdit), typeof(HikeEdit));
        }
    }
}
