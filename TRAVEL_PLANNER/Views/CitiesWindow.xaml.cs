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
        private List<City> _availableCities = new List<City>();
        private City _selectedCity;

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
            _selectedCity = CityComboBox.SelectedItem as City;
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
            RefreshAttractions();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!AppState.IsAuthenticated)
            {
                MessageBox.Show("Незареєстрований користувач не може зберегти подорож. Спочатку увійдіть в акаунт", "TRAVEL PLANNER");
                return;
            }

            if (_selectedCity == null)
            {
                MessageBox.Show("Оберіть місто зі списку", "TRAVEL PLANNER");
                return;
            }

            if (!DatePickerFrom.SelectedDate.HasValue || !DatePickerTo.SelectedDate.HasValue)
            {
                MessageBox.Show("Оберіть дату початку та завершення подорожі", "TRAVEL PLANNER");
                return;
            }

            var cards = AttractionsItemsControl.ItemsSource as IEnumerable<PlaceCardItem>;
            var selectedPlaces = cards?
                .Where(item => item.IsAdded)
                .Select(item => item.Place)
                .ToList() ?? new List<string>();

            var categories = cards?
                .Where(item => item.IsAdded)
                .Select(item => item.Category)
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Distinct()
                .ToList() ?? new List<string>();

            if (categories.Count == 0)
            {
                categories = GetSelectedCategories().ToList();
            }

            if (categories.Count == 0)
            {
                categories.Add("Індивідуальний маршрут");
            }

            AppState.SaveTrip(new SavedTrip
            {
                CountryName = $"{_selectedCity.country}: {_selectedCity.name}",
                DateFrom = DatePickerFrom.SelectedDate.Value,
                DateTo = DatePickerTo.SelectedDate.Value,
                Categories = categories,
                SelectedPlaces = selectedPlaces
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

            _availableCities = JsonService.GetCitiesForCountry(AppState.SelectedCountryName);
            if (!_availableCities.Any())
            {
                MessageBox.Show("Введена країна не має записів у файлі cities.json, тому сторінка \"Міста\" залишається заблокованою", "TRAVEL PLANNER");
                NavigationService.Open(this, new CountriesWindow());
                return;
            }

            CountryNameTextBlock.Text = AppState.SelectedCountryName;
            CityComboBox.ItemsSource = _availableCities;
            CityComboBox.SelectedIndex = 0;
        }

        private void RefreshSelectedCity()
        {
            if (_selectedCity == null)
            {
                ClearCitySelection();
                return;
            }

            CityViewImage.Source = JsonService.LoadCityView(_selectedCity);
            SelectedCityNameTextBlock.Text = _selectedCity.name;
            SelectedCityShortInfoTextBlock.Text = BuildCityShortInfo(_selectedCity);
            TextBox_Info.Text = BuildCityInfo(_selectedCity);
            RefreshAttractions();
        }

        private void RefreshAttractions()
        {
            if (_selectedCity == null)
            {
                AttractionsItemsControl.ItemsSource = null;
                return;
            }

            var filters = GetSelectedCategories().ToList();
            var cards = _selectedCity.attractions
                .Where(item => filters.Count == 0 || filters.Any(filter => MatchesCategory(filter, item.category)))
                .Select(item => new PlaceCardItem
                {
                    Place = item.attraction,
                    Category = item.category,
                    Description = string.IsNullOrWhiteSpace(item.description)
                        ? ""
                        : item.description,
                    ImagePath = item.image,
                    PreViewImage = JsonService.LoadAttractionImage(_selectedCity, item)
                })
                .ToList();

            AttractionsItemsControl.ItemsSource = cards;
        }

        private void ClearCitySelection()
        {
            CityViewImage.Source = null;
            SelectedCityNameTextBlock.Text = string.Empty;
            SelectedCityShortInfoTextBlock.Text = string.Empty;
            TextBox_Info.Text = string.Empty;
            AttractionsItemsControl.ItemsSource = null;
        }

        private IEnumerable<string> GetSelectedCategories()
        {
            if (CheckBoxHeart.IsChecked == true)
            {
                yield return CheckBoxHeart.Content.ToString();
            }

            if (CheckBoxPark.IsChecked == true)
            {
                yield return CheckBoxPark.Content.ToString();
            }

            if (CheckBoxCulture.IsChecked == true)
            {
                yield return CheckBoxCulture.Content.ToString();
            }

            if (CheckBoxHistoric.IsChecked == true)
            {
                yield return CheckBoxHistoric.Content.ToString();
            }

            if (CheckBoxReligious.IsChecked == true)
            {
                yield return CheckBoxReligious.Content.ToString();
            }

            if (CheckBoxArchitecture.IsChecked == true)
            {
                yield return CheckBoxArchitecture.Content.ToString();
            }
        }

        private static bool MatchesCategory(string selectedCategory, string actualCategory)
        {
            if (string.IsNullOrWhiteSpace(selectedCategory) || string.IsNullOrWhiteSpace(actualCategory))
            {
                return false;
            }

            return actualCategory.IndexOf(selectedCategory, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private static string BuildCityShortInfo(City city)
        {
            if (!string.IsNullOrWhiteSpace(city.info))
            {
                return city.info;
            }

            var attractions = city.attractions
                .Take(3)
                .Select(item => item.attraction)
                .ToList();

            return $"Рекомендовані місця для знайомства з містом: {string.Join(", ", attractions)}";
        }

        private static string BuildCityInfo(City city)
        {
            var baseInfo = string.IsNullOrWhiteSpace(city.info)
                ? ""
                : city.info;

            var attractions = city.attractions
                .Take(4)
                .Select(item => item.attraction)
                .ToList();

            return $"{baseInfo}{Environment.NewLine}{Environment.NewLine}Рекомендовані місця для старту маршруту: {string.Join(", ", attractions)}";
        }
    }
}
