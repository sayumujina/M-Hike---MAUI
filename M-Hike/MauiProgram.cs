using CommunityToolkit.Maui;
using M_Hike.Database;
using M_Hike.Hikes;
using Microsoft.Extensions.Logging;

namespace M_Hike
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<HikeSQLiteDatabase>(); // Run once
            builder.Services.AddTransient<HikeList>(); // Run per view load
            builder.Services.AddTransient<HikeEdit>(); // Run per view load

            return builder.Build();
        }
    }
}
