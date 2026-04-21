using System.Windows;
using TRAVEL_PLANNER.Services;

namespace TRAVEL_PLANNER.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            Loaded += (_, __) => RefreshState();
        }

        private void Main_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Open(this, AppState.IsAuthenticated ? (Window)new MainWindow() : new LoginWindow());
        }

        private void Countries_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Open(this, new CountriesWindow());
        }

        private void Cities_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Open(this, new CitiesWindow());
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ApplyTheme_Click(object sender, RoutedEventArgs e)
        {
            ThemeService.ApplyTheme(DarkThemeRadio.IsChecked == true);
            RefreshState();
        }

        private void SignOut_Click(object sender, RoutedEventArgs e)
        {
            AppState.SignOut();
            NavigationService.Open(this, new LoginWindow());
        }

        private void RefreshState()
        {
            LightThemeRadio.IsChecked = !AppState.IsDarkTheme;
            DarkThemeRadio.IsChecked = AppState.IsDarkTheme;
            CurrentUserText.Text = AppState.IsAuthenticated
                ? $"Ви увійшли як: {AppState.UserEmail}"
                : "Користувач не авторизований. Можна повернутися на сторінку входу.";
        }
    }
}
