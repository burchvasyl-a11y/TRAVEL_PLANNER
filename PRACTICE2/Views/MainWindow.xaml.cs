using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PRACTICE2.Views;

namespace PRACTICE2.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void Main_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Countries_Click(object sender, RoutedEventArgs e) 
        {
            CountriesWindow window = new CountriesWindow();
            window.Show();
            this.Close();
        } 
        
        private void Cities_Click(object sender, RoutedEventArgs e) 
        {
            CitiesWindow window = new CitiesWindow(); 
            window.Show();
            this.Close();
        } 
        
        private void Settings_Click(object sender, RoutedEventArgs e) 
        { 
            SettingsWindow window = new SettingsWindow(); 
            window.Show();
            this.Close();
        }
        
        private void Show_Click(object sender, RoutedEventArgs e)
        {
            // Show the entered values (email and password)
            string email = NameBox?.Text ?? string.Empty;
            string password = AgeBox?.Text ?? string.Empty;
            MessageBox.Show($"Ел. пошта: {email}\nПароль: {password}", "Введені дані", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            // Clear the text boxes
            if (NameBox != null) NameBox.Clear();
            if (AgeBox != null) AgeBox.Clear();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            // Close the window/application
            this.Close();
        }
    }
}