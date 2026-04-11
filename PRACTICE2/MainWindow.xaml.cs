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

namespace PRACTICE2
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

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            // Show the entered values (email and password)
            string email = nameBox?.Text ?? string.Empty;
            string password = ageBox?.Text ?? string.Empty;
            MessageBox.Show($"Ел. пошта: {email}\nПароль: {password}", "Введені дані", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            // Clear the text boxes
            if (nameBox != null) nameBox.Clear();
            if (ageBox != null) ageBox.Clear();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            // Close the window/application
            this.Close();
        }
    }
}
