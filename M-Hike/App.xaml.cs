using M_Hike.Database;
using M_Hike.Models;

namespace M_Hike
{
    public partial class App : Application
    {
        public Hike? selectedHikes;
        private static HikeSQLiteDatabase? hikeDb;

        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        public static HikeSQLiteDatabase hikeDatabase
        {
            get
            {
                if (hikeDb == null)
                {
                    hikeDb = new HikeSQLiteDatabase();
                }
                return hikeDb;
            }
        }
    }
}