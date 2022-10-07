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

namespace WpfApp3
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

        private void ButtonConteiners_Click(object sender, RoutedEventArgs e)
        {
            new ConteinersWindow().ShowDialog();
        }

        private void ButtonMines_Click(object sender, RoutedEventArgs e)
        {
            new MinesWindow().ShowDialog();
        }

        private void ButtonsCanvas_Click(object sender, RoutedEventArgs e)
        {
            new CanvasWindow().ShowDialog();
        }

        private void ButtonStyles_Click(object sender, RoutedEventArgs e)
        {
            new StylesWindow().ShowDialog();
        }

        private void ButtonTriggers_Click(object sender, RoutedEventArgs e)
        {
            new TriggersWindow().ShowDialog();
        }

        private void ButtonRegistration_Click(object sender, RoutedEventArgs e)
        {
            new RegistrationFformWindow().ShowDialog();
        }
    }
}
