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
using System.Data.SqlClient;
using WpfApp3.ADO.Entities;
using WpfApp3.ADO.DAL;

namespace WpfApp3.ADO
{
    /// <summary>
    /// Логика взаимодействия для SalesWindow.xaml
    /// </summary>
    public partial class SalesWindow : Window
    {
        private readonly SqlConnection _connection;
        private readonly DAL.Departments _departments;  // "инструментарий" работы с таблицей
        private readonly DAL.Products _products;
        private readonly DAL.Managers _managers;
        private readonly DAL.Departments _countManagers;
        public SalesWindow()
        {
            InitializeComponent();
            _connection = new SqlConnection(App.ConnecTionString);
            _departments = new DAL.Departments(_connection);
            _products = new DAL.Products(_connection);
            _managers = new DAL.Managers(_connection);
            _countManagers = new DAL.Departments(_connection);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           ShowMonitor();
           ShowDepartments();
           ShowProducts();
           ShowManagers();
           ShowDepManagers();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            _connection.Close();
        }

        private void ShowProducts()
        {
            ProductsGrid.ItemsSource = _products.GetList();
            ProductsGrid.Columns[0].Visibility = Visibility.Collapsed;
        }

        private void ShowDepartments()
        {
            // ORM - Object Relation Mapping - Отображение реляционных данных на объекты
            // "Слой" программы, отвечающий за преобразование данных в объекты и их коллекции
            // Детали - см. в папке Entities
            StringBuilder sb = new();
            foreach (Entities.Department department in _departments.GetList())
            {
                sb.AppendLine(department.ToString());
            }
            DepartmentsInfo.Text = sb.ToString();           
        }

        private void ShowManagers()
        {
            ManagersGrid.ItemsSource = _managers.GetList();
            ManagersGrid.Columns[0].Visibility = Visibility.Collapsed;            
        }

        private void ShowDepManagers()
        {
            CountManagerGrid.ItemsSource = _departments.GetList();         
            CountManagerGrid.Columns[0].Visibility = Visibility.Collapsed;
           
        }

        private void ShowMonitor()
        {
            try
            {
                _connection.Open();
                MonitorDbLabel.Content = "Підключено";
                MonitorDbLabel.Foreground = Brushes.Green;
            }
            catch
            {
                MonitorDbLabel.Content = "Відключено";
                MonitorDbLabel.Foreground = Brushes.Red;
            }
            String sql = "SELECT COUNT(*) FROM Departments";
            using (SqlCommand cmd = new(sql, _connection))
            {
                try
                {
                    MonitorDepartmentsLabel.Content = cmd.ExecuteScalar();
                    MonitorDepartmentsLabel.Foreground = Brushes.Green;
                }
                catch
                {
                    MonitorDepartmentsLabel.Content = "--";
                    MonitorDepartmentsLabel.Foreground = Brushes.Red;
                }
            }
             sql = "SELECT COUNT(*) FROM Managers";
            using (SqlCommand cmd = new(sql, _connection))
            {
                try
                {
                    MonitorManagersLabel.Content = cmd.ExecuteScalar();
                    MonitorManagersLabel.Foreground = Brushes.Green;
                }
                catch
                {
                    MonitorManagersLabel.Content = "--";
                    MonitorManagersLabel.Foreground = Brushes.Red;
                }
            }
            sql = "SELECT COUNT(*) FROM Products";
            using (SqlCommand cmd = new(sql, _connection))
            {
                try
                {
                    MonitorProductsLabel.Content = cmd.ExecuteScalar();
                    MonitorProductsLabel.Foreground = Brushes.Green;
                }
                catch
                {
                    MonitorProductsLabel.Content = "--";
                    MonitorProductsLabel.Foreground = Brushes.Red;
                }
            }
            sql = "SELECT COUNT(*) FROM Sales";
            using (SqlCommand cmd = new(sql, _connection))
            {
                try
                {
                    MonitorSalesLabel.Content = cmd.ExecuteScalar();
                    MonitorSalesLabel.Foreground = Brushes.Green;
                }
                catch
                {
                    MonitorSalesLabel.Content = "--";
                    MonitorSalesLabel.Foreground = Brushes.Red;
                }
            }
        }
    }
}
/* При работе с БД разделяют два режима работы - присоединенный и отсоединенный
 * 
 * В присоединенном режиме подключение к БД не закрывается и каждое обращение
 * к данным является запросом к БД. Режим используется если к БД есть другие
 * обращения (от других программ), режим обеспечивает актуальность данных.
 * 
 * В отсоединенном режиме при первом запросе ВСЕ данные из БД копируются
 * в приложение, соединение закрывается. Программа работает с локальной 
 * копией и при завершении работы сохраняет изменения в БД. Режим используется
 * когда БД использует только одна программа, других участников нет.
 * Работает быстрее, т.к. нет дополнительных запросов.
 */