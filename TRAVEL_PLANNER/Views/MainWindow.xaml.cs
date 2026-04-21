using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using TRAVEL_PLANNER.Models;
using TRAVEL_PLANNER.Services;

namespace TRAVEL_PLANNER.Views
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _userEmailText;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += (_, __) => RefreshState();
        }

        public string UserEmailText
        {
            get => _userEmailText;
            set
            {
                _userEmailText = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Main_Click(object sender, RoutedEventArgs e)
        {
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
            NavigationService.Open(this, new SettingsWindow());
        }

        private void AddTrip_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Open(this, new CountriesWindow());
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is FrameworkElement element) || !(element.Tag is SavedTrip trip))
            {
                return;
            }

            MessageBox.Show(
                $"Країна: {trip.CountryName}\n" +
                $"Дати: {trip.DisplayDateRange}\n" +
                $"Категорії: {string.Join(", ", trip.Categories)}\n" +
                $"Місця: {string.Join(", ", trip.SelectedPlaces)}",
                "Деталі подорожі");
        }

        private void RefreshState()
        {
            if (!AppState.IsAuthenticated)
            {
                NavigationService.Open(this, new LoginWindow());
                return;
            }

            UserEmailText = AppState.UserEmail;
            var trips = new List<SavedTrip>(AppState.SavedTrips);
            TripsItemsControl.ItemsSource = trips;

            var hasTrips = trips.Count > 0;
            EmptyStateBorder.Visibility = hasTrips ? Visibility.Collapsed : Visibility.Visible;
            TripsScrollViewer.Visibility = hasTrips ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
