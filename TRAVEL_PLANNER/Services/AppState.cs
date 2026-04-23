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

        public static void Initialize()
        {
            AuthService.EnsureStorage();

            var restoredUser = AuthService.RestoreSession();
            if (string.IsNullOrWhiteSpace(restoredUser))
            {
                IsAuthenticated = false;
                UserEmail = null;
                SavedTrips.Clear();
                return;
            }

            ApplyAuthenticatedUser(restoredUser);
        }

        public static bool TryRegister(string email, string password, out string errorMessage)
        {
            if (!AuthService.TryRegister(email, password, out errorMessage))
            {
                return false;
            }

            ApplyAuthenticatedUser(email);
            return true;
        }

        public static bool TrySignIn(string email, string password, out string errorMessage)
        {
            if (!AuthService.TrySignIn(email, password, out errorMessage))
            {
                return false;
            }

            ApplyAuthenticatedUser(email);
            return true;
        }

        public static void SignOut()
        {
            AuthService.SignOut();
            IsAuthenticated = false;
            UserEmail = null;
            SavedTrips.Clear();
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
            }
            else
            {
                SavedTrips.Add(trip);
            }

            PersistTrips();
        }

        private static void ApplyAuthenticatedUser(string email)
        {
            IsAuthenticated = true;
            UserEmail = email?.Trim().ToLowerInvariant();
            SavedTrips.Clear();
            SavedTrips.AddRange(AuthService.LoadTrips(UserEmail));
        }

        private static void PersistTrips()
        {
            if (!IsAuthenticated || string.IsNullOrWhiteSpace(UserEmail))
            {
                return;
            }

            AuthService.SaveTrips(UserEmail, SavedTrips);
        }
    }
}
