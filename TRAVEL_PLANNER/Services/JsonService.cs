using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TRAVEL_PLANNER.Models;

namespace TRAVEL_PLANNER.Services
{
    public static class JsonService
    {
        private static readonly Dictionary<string, string> CountryFolders = new Dictionary<string, string>
        {
            ["Україна"] = "ukr",
            ["Румунія"] = "rom",
            ["Словенія"] = "slo",
            ["Італія"] = "ita",
            ["Мавританія"] = "mrt",
            ["Йорданія"] = "jor",
            ["Австралія"] = "aus",
            ["Перу"] = "per",
            ["Сенегал"] = "sen"
        };

        public static List<Country> LoadCountries()
        {
            return LoadCollection<Country>("countries.json", "countries");
        }

        public static List<City> LoadCities()
        {
            return LoadCollection<City>("cities.json", "cities");
        }

        public static List<City> GetCitiesForCountry(string countryName)
        {
            var normalizedCountryName = NormalizeText(countryName);
            return LoadCities()
                .Where(item => string.Equals(NormalizeText(item.country), normalizedCountryName, StringComparison.CurrentCultureIgnoreCase))
                .ToList();
        }

        public static bool HasCitiesForCountry(string countryName)
        {
            return GetCitiesForCountry(countryName).Any();
        }

        public static ImageSource LoadCountryMap(Country country)
        {
            if (country == null)
            {
                return null;
            }

            var fileName = !string.IsNullOrWhiteSpace(country.map) ? country.map : country.View;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            return BuildImageSource($"/TRAVEL_PLANNER;component/Resources/Maps/{fileName}");
        }

        public static ImageSource LoadPlaceImage(Country country, Place place)
        {
            if (country == null || place == null || string.IsNullOrWhiteSpace(place.image))
            {
                return null;
            }

            if (!CountryFolders.TryGetValue(NormalizeText(country.name), out var folder))
            {
                return null;
            }

            return BuildImageSource($"/TRAVEL_PLANNER;component/Resources/Images/countries/{folder}/{place.image}");
        }

        public static ImageSource LoadCityView(City city)
        {
            if (city == null || string.IsNullOrWhiteSpace(city.view))
            {
                return null;
            }

            return BuildImageSource($"/TRAVEL_PLANNER;component/Resources/CityViews/{city.view}");
        }

        public static ImageSource LoadAttractionImage(City city, Attraction attraction)
        {
            if (city == null || attraction == null || string.IsNullOrWhiteSpace(city.view) || string.IsNullOrWhiteSpace(attraction.image))
            {
                return null;
            }

            var cityFolder = Path.GetFileNameWithoutExtension(city.view);
            if (string.IsNullOrWhiteSpace(cityFolder))
            {
                return null;
            }

            return BuildImageSource($"/TRAVEL_PLANNER;component/Resources/Images/cities/{cityFolder}/{attraction.image}");
        }

        private static List<T> LoadCollection<T>(string fileName, string rootPropertyName)
        {
            var json = File.ReadAllText(BuildDataPath(fileName), Encoding.UTF8);
            using (var document = JsonDocument.Parse(json))
            {
                if (!document.RootElement.TryGetProperty(rootPropertyName, out var rootArray))
                {
                    return new List<T>();
                }

                return JsonSerializer.Deserialize<List<T>>(rootArray.GetRawText()) ?? new List<T>();
            }
        }

        private static string NormalizeText(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? string.Empty : text.Trim();
        }

        private static string BuildDataPath(string fileName)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Data", fileName);
        }

        private static ImageSource BuildImageSource(string uri)
        {
            try
            {
                return new BitmapImage(new Uri(uri, UriKind.Relative));
            }
            catch
            {
                return null;
            }
        }
    }
}
