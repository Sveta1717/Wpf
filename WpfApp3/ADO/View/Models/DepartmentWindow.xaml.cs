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
    /// Логика взаимодействия для DepartmentWindow.xaml
    /// Окно - форма для данных из таблицы БД Departments
    /// </summary>
    public partial class DepartmentWindow : Window
    {
        public Entities.Department Department { get; set; }
        public DepartmentWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DepartamentId.Text = Department.Id.ToString();
            //Режим добавления - если передан пустой Id
            if (Department.Id == Guid.Empty)
            {
                Save.Content = "Добавити";
                DepartmentName.Text = "";
                Delete.IsEnabled = false;
            }
            else
            {
                DepartmentName.Text = Department.Name;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Department.Name = DepartmentName.Text;
            DialogResult = true;
            this.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBoxResult.Yes ==
                MessageBox.Show("Ви впевнені?", "Видалення данних",
                MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                Department.Name = String.Empty;
                DialogResult=true;
                this.Close();
            }
        }
    }
}
