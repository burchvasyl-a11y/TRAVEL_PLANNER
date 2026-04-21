using System;
using System.Linq;
using System.Windows;

namespace TRAVEL_PLANNER.Services
{
    public static class ThemeService
    {
        private const string LightThemePath = "Resources/Themes/LightTheme.xaml";
        private const string DarkThemePath = "Resources/Themes/DarkTheme.xaml";

        public static void ApplyTheme(bool isDarkTheme)
        {
            var application = Application.Current;
            if (application == null)
            {
                return;
            }

            var mergedDictionaries = application.Resources.MergedDictionaries;
            var existingTheme = mergedDictionaries.FirstOrDefault(dictionary =>
                dictionary.Source != null &&
                (dictionary.Source.OriginalString.EndsWith(LightThemePath, StringComparison.OrdinalIgnoreCase) ||
                 dictionary.Source.OriginalString.EndsWith(DarkThemePath, StringComparison.OrdinalIgnoreCase)));

            if (existingTheme != null)
            {
                mergedDictionaries.Remove(existingTheme);
            }

            mergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(isDarkTheme ? DarkThemePath : LightThemePath, UriKind.Relative)
            });

            AppState.SetTheme(isDarkTheme);
        }
    }
}
