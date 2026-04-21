using System.Windows;
using System.Windows.Input;
using TRAVEL_PLANNER.Services;

namespace TRAVEL_PLANNER.Views
{
    public partial class LoginWindow : Window
    {
        private bool _isPasswordVisible;

        public LoginWindow()
        {
            InitializeComponent();
        }

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

        private void TogglePassword_Click(object sender, RoutedEventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;

            if (_isPasswordVisible)
            {
                PasswordTextBox.Text = PasswordBox.Password;
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                PasswordBox.Password = PasswordTextBox.Text;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                PasswordBox.Visibility = Visibility.Visible;
            }
        }

        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("У цьому макеті відновлення пароля ще не реалізоване.", "Підказка");
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            SignIn(EmailTextBox.Text);
        }

        private void GoogleButton_Click(object sender, RoutedEventArgs e)
        {
            SignIn("google.user@travelplanner.app");
        }

        private void GitHubButton_Click(object sender, RoutedEventArgs e)
        {
            SignIn("github.user@travelplanner.app");
        }

        private void SignIn(string email)
        {
            var password = _isPasswordVisible ? PasswordTextBox.Text : PasswordBox.Password;
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введіть ел. пошту та пароль, щоб перейти на головну сторінку.", "TRAVEL PLANNER");
                return;
            }

            AppState.SignIn(email.Trim());
            NavigationService.Open(this, new MainWindow());
        }
    }
}
