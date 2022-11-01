using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace WpfApp3
{
    /// <summary>
    /// Логика взаимодействия для StylesWindow.xaml
    /// </summary>
    public partial class StylesWindow : Window
        
    {
       
        public StylesWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = new Button() { Content = "New Button" };
            Field.Children.Add(button); // новые элементы появляются со стилями,
                                        // определенными в ресурсах окна, но только
                                        // тот, который для всех кнопок 
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Найти ресурс окна и использовать его в новом элементе
            Style? style = this.FindResource("BgCentered") as Style;
            var label = new Label()
            {
                Content = "New Label",
            Style = style
            };          
            Field.Children.Add(label);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Style? style = this.FindResource("CenteredContent") as Style;
            var label = new Label()
            {
                Content = "New Label3",
                Style = style                
            };
            Field.Children.Add(label);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            FontFamily? fontFamily = this.FindResource("Comic") as FontFamily;
            var label = new Label()
            {
                Content = "New Label4",
               FontFamily = fontFamily
            };
            Field.Children.Add(label);
        }
    }
}
