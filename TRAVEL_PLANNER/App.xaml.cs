using System.Windows;
using TRAVEL_PLANNER.Services;

namespace TRAVEL_PLANNER
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ThemeService.ApplyTheme(false);
        }
    }
}
