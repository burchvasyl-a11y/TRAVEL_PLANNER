using System.Collections.Generic;
using System.Linq;
using TRAVEL_PLANNER.Models;

namespace TRAVEL_PLANNER.Services
{
    public static class AppState
    {
        public static bool IsDarkTheme { get; private set; }

        public static bool IsAuthenticated { get; private set; }

        public static string UserEmail { get; private set; }

        public static string SelectedCountryName { get; private set; }

        public static List<SavedTrip> SavedTrips { get; } = new List<SavedTrip>();

        public static void SignIn(string email)
        {
            IsAuthenticated = true;
            UserEmail = email;
        }

        public static void SignOut()
        {
            IsAuthenticated = false;
            UserEmail = null;
        }

        public static void SetTheme(bool isDarkTheme)
        {
            IsDarkTheme = isDarkTheme;
        }

        public static void SetSelectedCountry(string countryName)
        {
            SelectedCountryName = countryName;
        }

        public static void ClearSelectedCountry()
        {
            SelectedCountryName = null;
        }

        public static void SaveTrip(SavedTrip trip)
        {
            var existingTrip = SavedTrips.FirstOrDefault(item =>
                item.CountryName == trip.CountryName &&
                item.DateFrom == trip.DateFrom &&
                item.DateTo == trip.DateTo);

            if (existingTrip != null)
            {
                existingTrip.Categories = trip.Categories;
                existingTrip.SelectedPlaces = trip.SelectedPlaces;
                return;
            }

            SavedTrips.Add(trip);
        }
    }
}
