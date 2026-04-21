using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using TRAVEL_PLANNER.Models;
using TRAVEL_PLANNER.Services;

namespace TRAVEL_PLANNER.Views
{
    public partial class CountriesWindow : Window, INotifyPropertyChanged
    {
        private readonly List<Country> _countries;
        private Country _selectedCountry;
        private string _selectedCountryName;
        private string _selectedCountryShortInfo;

        public CountriesWindow()
        {
            InitializeComponent();
            DataContext = this;

            _countries = JsonService.LoadCountries();
            DatePickerFrom.SelectedDate = DateTime.Today;
            DatePickerTo.SelectedDate = DateTime.Today.AddDays(6);
            ClearCountrySelection();
        }

        public string SelectedCountryName
        {
            get => _selectedCountryName;
            set
            {
                _selectedCountryName = value;
                OnPropertyChanged();
            }
        }

        public string SelectedCountryShortInfo
        {
            get => _selectedCountryShortInfo;
            set
            {
                _selectedCountryShortInfo = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Main_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Open(this, AppState.IsAuthenticated ? (Window)new MainWindow() : new LoginWindow());
        }

        private void Countries_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Cities_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCountry == null)
            {
                MessageBox.Show("Спочатку введіть назву країни на сторінці \"Країни\". Сторінка \"Міста\" поки заблокована без вибраної країни.", "TRAVEL PLANNER");
                return;
            }

            if (!JsonService.HasCitiesForCountry(_selectedCountry.name))
            {
                MessageBox.Show("Введена країна не має записів у файлі cities.json, тому сторінка \"Міста\" залишається заблокованою.", "TRAVEL PLANNER");
                return;
            }

            NavigationService.Open(this, new CitiesWindow());
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Open(this, new SettingsWindow());
        }

        private void CountryTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var query = TextBox_Country.Text?.Trim();
            if (string.IsNullOrWhiteSpace(query))
            {
                ClearCountrySelection(clearText: false);
                return;
            }

            var country = _countries.FirstOrDefault(item =>
                item.name.Equals(query, StringComparison.CurrentCultureIgnoreCase));

            if (country == null)
            {
                ClearCountrySelection(clearText: false);
                return;
            }

            SetSelectedCountry(country);
        }

        private void DatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
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
            RefreshPlaces();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!AppState.IsAuthenticated)
            {
                MessageBox.Show("Незареєстрований користувач не може зберегти подорож. Спочатку увійдіть в акаунт.", "TRAVEL PLANNER");
                return;
            }

            if (_selectedCountry == null)
            {
                MessageBox.Show("Введіть країну.", "TRAVEL PLANNER");
                return;
            }

            if (!DatePickerFrom.SelectedDate.HasValue || !DatePickerTo.SelectedDate.HasValue)
            {
                MessageBox.Show("Оберіть дату початку та завершення подорожі.", "TRAVEL PLANNER");
                return;
            }

            var placeCards = PlacesItemsControl.ItemsSource as IEnumerable<PlaceCardItem>;
            var selectedPlaces = placeCards?
                .Where(item => item.IsAdded)
                .Select(item => item.Place)
                .ToList() ?? new List<string>();

            var categories = GetSelectedCategories().ToList();
            if (categories.Count == 0 && placeCards != null)
            {
                categories = placeCards
                    .Where(item => item.IsAdded)
                    .Select(item => item.Category)
                    .Where(category => !string.IsNullOrWhiteSpace(category))
                    .Distinct()
                    .ToList();
            }

            if (categories.Count == 0)
            {
                categories.Add("Індивідуальний маршрут");
            }

            AppState.SaveTrip(new SavedTrip
            {
                CountryName = _selectedCountry.name,
                DateFrom = DatePickerFrom.SelectedDate.Value,
                DateTo = DatePickerTo.SelectedDate.Value,
                Categories = categories,
                SelectedPlaces = selectedPlaces
            });

            MessageBox.Show("Подорож збережено на головній сторінці.", "TRAVEL PLANNER");
        }

        private void SetSelectedCountry(Country country)
        {
            _selectedCountry = country;
            SelectedCountryName = _selectedCountry.name;
            SelectedCountryShortInfo = BuildCountryInfo(_selectedCountry);
            TextBox_Info.Text = BuildCountryInfo(_selectedCountry, includePlaces: true);
            CountryViewImage.Source = JsonService.LoadCountryMap(_selectedCountry);
            AppState.SetSelectedCountry(_selectedCountry.name);
            RefreshPlaces();
        }

        private void ClearCountrySelection(bool clearText = true)
        {
            _selectedCountry = null;
            SelectedCountryName = string.Empty;
            SelectedCountryShortInfo = string.Empty;
            TextBox_Info.Text = string.Empty;
            CountryViewImage.Source = null;
            PlacesItemsControl.ItemsSource = null;
            AppState.ClearSelectedCountry();

            if (!clearText)
            {
                return;
            }

            TextBox_Country.TextChanged -= CountryTextBox_TextChanged;
            TextBox_Country.Text = string.Empty;
            TextBox_Country.TextChanged += CountryTextBox_TextChanged;
        }

        private void RefreshPlaces()
        {
            if (_selectedCountry == null)
            {
                PlacesItemsControl.ItemsSource = null;
                return;
            }

            var filters = GetSelectedCategories().ToList();
            var places = _selectedCountry.places
                .Where(place => filters.Count == 0 || filters.Any(filter => MatchesCategory(filter, place.category)))
                .Select(place => new PlaceCardItem
                {
                    Place = place.place,
                    Category = place.category,
                    Description = string.IsNullOrWhiteSpace(place.description) ? string.Empty : place.description,
                    ImagePath = place.image,
                    PreViewImage = JsonService.LoadPlaceImage(_selectedCountry, place)
                })
                .ToList();

            PlacesItemsControl.ItemsSource = places;
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

            if (selectedCategory == "Відпочинок на морі" &&
                actualCategory.IndexOf("Відпочинок", StringComparison.CurrentCultureIgnoreCase) >= 0)
            {
                return true;
            }

            if (selectedCategory == "Відпочинок на природі" &&
                actualCategory.IndexOf("Відпочинок", StringComparison.CurrentCultureIgnoreCase) >= 0)
            {
                return true;
            }

            return false;
        }

        private static string BuildCountryInfo(Country country, bool includePlaces = false)
        {
            var baseInfo = string.IsNullOrWhiteSpace(country.info)
                ? $"{country.name} підходить для гнучкого планування подорожі з акцентом на природу, пам'ятки та цікаві локації."
                : country.info;

            if (!includePlaces)
            {
                return baseInfo;
            }

            var highlightedPlaces = string.Join(", ", country.places.Take(3).Select(place => place.place));
            return $"{baseInfo}\n\nРекомендовані місця для старту маршруту: {highlightedPlaces}.";
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
