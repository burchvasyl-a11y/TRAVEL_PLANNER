using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            return LoadCountryCollection("countries.json", "countries");
        }

        public static List<Country> LoadCities()
        {
            return LoadCountryCollection("cities.json", "country");
        }

        public static Country GetCitiesCountry(string countryName)
        {
            return LoadCountries()
                .FirstOrDefault(item => item.name.Equals(countryName, StringComparison.CurrentCultureIgnoreCase));
        }

        public static bool HasCitiesForCountry(string countryName)
        {
            var country = GetCitiesCountry(countryName);
            return country != null && country.places.Any();
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

        public static ImageSource LoadCityImage(City city)
        {
            if (city == null)
            {
                return null;
            }

            var fileName = !string.IsNullOrWhiteSpace(city.view) ? city.view : city.View;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            return BuildImageSource($"/TRAVEL_PLANNER;component/Resources/CityViews/{fileName}");
        }

        public static ImageSource LoadPlaceImage(Country country, Place place)
        {
            if (country == null || place == null || string.IsNullOrWhiteSpace(place.image))
            {
                return null;
            }

            if (!CountryFolders.TryGetValue(country.name, out var folder))
            {
                return null;
            }

            return BuildImageSource($"/TRAVEL_PLANNER;component/Resources/Images/countries/{folder}/{place.image}");
        }

        private static List<Country> LoadCountryCollection(string fileName, string rootPropertyName)
        {
            var json = File.ReadAllText(BuildDataPath(fileName));
            using (var document = JsonDocument.Parse(json))
            {
                if (!document.RootElement.TryGetProperty(rootPropertyName, out var rootArray))
                {
                    return new List<Country>();
                }

                return JsonSerializer.Deserialize<List<Country>>(rootArray.GetRawText()) ?? new List<Country>();
            }
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
