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

        private void TogglePassword_Click(object sender, MouseButtonEventArgs e)
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
            MessageBox.Show("Відновлення пароля поки не реалізоване", "TRAVEL PLANNER");
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var email = EmailTextBox.Text;
            var password = GetPassword();

            if (!AppState.TrySignIn(email, password, out var errorMessage))
            {
                MessageBox.Show(errorMessage, "TRAVEL PLANNER");
                return;
            }

            NavigationService.Open(this, new MainWindow());
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var email = EmailTextBox.Text;
            var password = GetPassword();

            if (!AppState.TryRegister(email, password, out var errorMessage))
            {
                MessageBox.Show(errorMessage, "TRAVEL PLANNER");
                return;
            }

            MessageBox.Show("Акаунт створено. Ви вже увійшли в систему", "TRAVEL PLANNER");
            NavigationService.Open(this, new MainWindow());
        }

        private void GoogleButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вхід через Google поки не реалізований", "TRAVEL PLANNER");
        }

        private void GitHubButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вхід через GitHub поки не реалізований", "TRAVEL PLANNER");
        }

        private string GetPassword()
        {
            return _isPasswordVisible ? PasswordTextBox.Text : PasswordBox.Password;
        }
    }
}
