using System.Windows;
using TRAVEL_PLANNER.Services;
using TRAVEL_PLANNER.Views;

namespace TRAVEL_PLANNER
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppState.Initialize();
            ThemeService.ApplyTheme(false);

            Window startupWindow = AppState.IsAuthenticated
                ? (Window)new MainWindow()
                : new LoginWindow();

            startupWindow.Show();
        }
    }
}
