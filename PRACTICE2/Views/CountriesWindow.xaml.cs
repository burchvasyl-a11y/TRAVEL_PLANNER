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
using System.Windows.Shapes;

namespace PRACTICE2.Views
{
    /// <summary>
    /// Interaction logic for CountriesWindow.xaml
    /// </summary>
    public partial class CountriesWindow : Window
    {
        public CountriesWindow()
        {
            InitializeComponent();
        }

        private void Main_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
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

        
    }
}
