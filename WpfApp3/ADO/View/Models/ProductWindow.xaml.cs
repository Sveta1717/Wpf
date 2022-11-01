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
using WpfApp3.ADO.Entities;

namespace WpfApp3.ADO.View.Models
{
    /// <summary>
    /// Логика взаимодействия для ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        public Entities.Product Product { get; set; }
        public ProductWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            //ProductId.Text = Product.Id.ToString();
            //Режим добавления - если передан пустой Id
            if (Product.Id == Guid.Empty)
            {
                Save.Content = "Добавити";
                ProductName.Text = "";
                Delete.IsEnabled = false;
            }
            else
            {
                ProductName.Text = Product.Name;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Product.Name = ProductName.Text;
            DialogResult = true;
            this.Close();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes ==
               MessageBox.Show("Ви впевнені?", "Видалення данних",
               MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                Product.Name = String.Empty;
                DialogResult = true;
                this.Close();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
