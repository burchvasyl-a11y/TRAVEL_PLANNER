using System.Windows;

namespace TRAVEL_PLANNER.Services
{
    public static class NavigationService
    {
        public static void Open(Window currentWindow, Window nextWindow)
        {
            nextWindow.Show();
            currentWindow?.Close();
        }
    }
}
