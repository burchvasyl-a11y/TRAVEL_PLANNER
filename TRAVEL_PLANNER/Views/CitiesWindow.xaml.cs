using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TRAVEL_PLANNER.Models;
using TRAVEL_PLANNER.Services;

namespace TRAVEL_PLANNER.Views
{
    public partial class CitiesWindow : Window
    {
        private Country _selectedCountry;
        private List<Place> _availableCities = new List<Place>();

        public CitiesWindow()
        {
            InitializeComponent();
            DatePickerFrom.SelectedDate = DateTime.Today;
            DatePickerTo.SelectedDate = DateTime.Today.AddDays(3);
            LoadCities();
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
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Open(this, new SettingsWindow());
        }

        private void CityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshSelectedCity();
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DatePickerFrom.SelectedDate.HasValue &&
                DatePickerTo.SelectedDate.HasValue &&
                DatePickerTo.SelectedDate < DatePickerFrom.SelectedDate)
            {
                DatePickerTo.SelectedDate = DatePickerFrom.SelectedDate;
            }
        }

        private void Filters_Changed(object sender, RoutedEventArgs e)
        {
            RefreshCityList();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!AppState.IsAuthenticated)
            {
                MessageBox.Show("Незареєстрований користувач не може зберегти подорож. Спочатку увійдіть в акаунт", "TRAVEL PLANNER");
                return;
            }

            if (!(CityComboBox.SelectedItem is Place city))
            {
                MessageBox.Show("Оберіть місто зі списку", "TRAVEL PLANNER");
                return;
            }

            if (!DatePickerFrom.SelectedDate.HasValue || !DatePickerTo.SelectedDate.HasValue)
            {
                MessageBox.Show("Оберіть дату початку та завершення подорожі", "TRAVEL PLANNER");
                return;
            }

            var categories = GetSelectedCategories().ToList();
            if (categories.Count == 0 && !string.IsNullOrWhiteSpace(city.category))
            {
                categories.Add(city.category);
            }

            if (categories.Count == 0)
            {
                categories.Add("Індивідуальний маршрут");
            }

            AppState.SaveTrip(new SavedTrip
            {
                CountryName = $"{_selectedCountry.name}: {city.place}",
                DateFrom = DatePickerFrom.SelectedDate.Value,
                DateTo = DatePickerTo.SelectedDate.Value,
                Categories = categories,
                SelectedPlaces = new List<string> { city.place }
            });

            MessageBox.Show("Подорож збережено на головній сторінці", "TRAVEL PLANNER");
        }

        private void LoadCities()
        {
            if (string.IsNullOrWhiteSpace(AppState.SelectedCountryName))
            {
                MessageBox.Show("Сторінка \"Міста\" поки недоступна. Спочатку введіть країну на сторінці \"Країни\"", "TRAVEL PLANNER");
                NavigationService.Open(this, new CountriesWindow());
                return;
            }

            _selectedCountry = JsonService.GetCitiesCountry(AppState.SelectedCountryName);
            if (_selectedCountry == null || !_selectedCountry.places.Any())
            {
                MessageBox.Show("Для вибраної країни немає записів", "TRAVEL PLANNER");
                NavigationService.Open(this, new CountriesWindow());
                return;
            }

            CountryNameTextBlock.Text = _selectedCountry.name;
            _availableCities = _selectedCountry.places.ToList();
            RefreshCityList();
        }

        private void RefreshCityList()
        {
            var filters = GetSelectedCategories().ToList();
            var filteredCities = _availableCities
                .Where(city => filters.Count == 0 || filters.Any(filter => MatchesCategory(filter, city.category)))
                .ToList();

            CityComboBox.ItemsSource = filteredCities;

            if (filteredCities.Count == 0)
            {
                ClearSelectedCity();
                return;
            }

            if (!filteredCities.Contains(CityComboBox.SelectedItem as Place))
            {
                CityComboBox.SelectedIndex = 0;
            }
            else
            {
                RefreshSelectedCity();
            }
        }

        private void RefreshSelectedCity()
        {
            if (!(CityComboBox.SelectedItem is Place city))
            {
                ClearSelectedCity();
                return;
            }

            CityImage.Source = JsonService.LoadCityMap(_selectedCountry);
            SelectedCityImage.Source = JsonService.LoadPlaceImage(_selectedCountry, city);
            SelectedCityNameTextBlock.Text = city.place;
            SelectedCityShortInfoTextBlock.Text = string.IsNullOrWhiteSpace(city.description)
                ? ""
                : city.description;
            SelectedCityCategoryTextBlock.Text = city.category;
            SelectedCityDescriptionTextBlock.Text = string.IsNullOrWhiteSpace(city.description)
                ? ""
                : city.description;
            TextBox_Info.Text = BuildCityInfo(city);
        }

        private void ClearSelectedCity()
        {
            CityMapImage.Source = JsonService.LoadCountryMap(_selectedCountry);
            SelectedCityImage.Source = null;
            SelectedCityNameTextBlock.Text = string.Empty;
            SelectedCityShortInfoTextBlock.Text = string.Empty;
            SelectedCityCategoryTextBlock.Text = string.Empty;
            SelectedCityDescriptionTextBlock.Text = string.Empty;
            TextBox_Info.Text = string.Empty;
        }

        private IEnumerable<string> GetSelectedCategories()
        {
            if (CheckBoxSea.IsChecked == true)
            {
                yield return CheckBoxSea.Content.ToString();
            }

            if (CheckBoxNature.IsChecked == true)
            {
                yield return CheckBoxNature.Content.ToString();
            }

            if (CheckBoxActivities.IsChecked == true)
            {
                yield return CheckBoxActivities.Content.ToString();
            }

            if (CheckBoxHistoric.IsChecked == true)
            {
                yield return CheckBoxHistoric.Content.ToString();
            }

            if (CheckBoxShopping.IsChecked == true)
            {
                yield return CheckBoxShopping.Content.ToString();
            }
        }

        private static bool MatchesCategory(string selectedCategory, string actualCategory)
        {
            if (string.IsNullOrWhiteSpace(selectedCategory) || string.IsNullOrWhiteSpace(actualCategory))
            {
                return false;
            }

            if (actualCategory.IndexOf(selectedCategory, StringComparison.CurrentCultureIgnoreCase) >= 0)
            {
                return true;
            }

                       return false;
        }

        private static string BuildCityInfo(Place city)
        {
            var description = string.IsNullOrWhiteSpace(city.description)
                ? ""
                : city.description;

            return $"Місто: {city.place}\nКатегорія: {city.category}\n\n{description}";
        }
    }
}
