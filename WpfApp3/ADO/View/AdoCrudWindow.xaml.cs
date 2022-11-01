using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;

namespace WpfApp3.ADO.View
{
    /// <summary>
    /// Логика взаимодействия для AdoCrudWindow.xaml
    /// </summary>
    public partial class AdoCrudWindow : Window
    {
        // ObservableCollection - коллекция, уведомлящая контейнер о своих изменениях
        public ObservableCollection <Entities.Department> Departments { get; set; } // для связывания 
        public ObservableCollection<Entities.Product> Products { get; set; }
        private readonly SqlConnection _connection;
        private readonly DAL.Departments _departments;
        private readonly DAL.Products _products;
        private DispatcherTimer timer = new DispatcherTimer();
        int n = 0;
        public AdoCrudWindow()
        {
            InitializeComponent();
            _connection = new SqlConnection(App.ConnecTionString);
            ConnectDb();
            _departments = new DAL.Departments(_connection);  
            _products = new DAL.Products(_connection);
            Departments = new(_departments.GetList())
            { 
                new Entities.Department
                {
                Id = Guid.Empty,
                Name = "Добавити новий відділ"
                }
            };
            Products = new (_products.GetList())            
            {
                new Entities.Product
                {
                Id = Guid.Empty,
                Name = "Добавити новий товар"
                }
            };
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
            // связывание данных (1) указываем контекст - откуда берутся
            // имена ресурсов
            this.DataContext = this;
        }

        private void ConnectDb()
        {
            try
            {
                _connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connection error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            Departments.Add(_departments.GetList()[n]);
            n++;
            if (n == _departments.GetList().Count)
            {
                timer.Stop();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {           
            //timer.Start();            
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            _connection?.Close();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // двойной щелчок мыши на элементе (строке) списка
            if (sender is ListViewItem item)
            {
                // в тело заходим, если sender - это ListViewItem
                // извлекаем ссылку на объект данных (Department)
                if (item.Content is Entities.Department department)
                {
                    // department - ссылка на элемент коллекции Departments,
                    // на котором сработало событие
                    // MessageBox.Show(department.ToString());
                    var editWindow = new View.Models.DepartmentWindow()
                    {
                        Department = department
                    };

                    if(editWindow.ShowDialog() == true)
                    {
                        if (department.Id == Guid.Empty)         // дОБАВЛЕНИЕ
                        {
                            if (department.Name != string.Empty)
                            {
                                Guid id = _departments.Create(department);
                                Departments.Remove(department);
                                department.Id = id;
                                Departments.Add(department);
                                Departments.Add(new Entities.Department
                                {
                                    Id = Guid.Empty,
                                    Name = "Добавити новий відділ"
                                });
                            }
                            else
                            {
                                MessageBox.Show("Будь ласка вкажите назву відділу!");
                            }
                        }
                    
                        else                                     // Изменение, удаление
                        {
                            if (department.Name == String.Empty) // удаление
                            {
                                Departments.Remove(department);
                                _departments.Delete(department);
                                MessageBox.Show("Відділ видалено");
                            }

                            else                                   //изменение
                            {
                                // Коллекция не отслеживает внутри элементов
                                // поэтому создаем эффект состава коллекции
                                int index = Departments.IndexOf(department);
                                Departments.Remove(department);
                                Departments.Insert(index, department);
                                // Пока обновлен только список, вносим изменения в БД
                                _departments.Update(department);
                            }
                        }
                    }
                   
                    //MessageBox.Show(department.ToString());
                }
                
            }
        }

        private void ListViewItem_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {                
                if (item.Content is Entities.Product product)
                {                    
                    var editWindow = new View.Models.ProductWindow()
                    {
                        Product = product
                    };

                    if (editWindow.ShowDialog() == true)
                    {
                        if (product.Id == Guid.Empty)        
                        {
                            if (product.Name != string.Empty)
                            {
                                Guid id = _products.Create(product);
                                Products.Remove(product);
                                product.Id = id;
                                Products.Add(product);
                                Products.Add(new Entities.Product
                                {
                                    Id = Guid.Empty,
                                    Name = "Добавити новий товар",
                                    Price = product.Price,  
                                });
                            }
                            else
                            {
                                MessageBox.Show("Будь ласка вкажите назву товару!");
                            }
                        }

                        else                                     // Изменение, удаление
                        {
                            if (product.Name == String.Empty) // удаление
                            {
                                Products.Remove(product);
                                _products.Delete(product);
                                MessageBox.Show("Товар видалено");
                            }

                            else                                   //изменение
                            {
                                
                                int index = Products.IndexOf(product);
                                Products.Remove(product);
                                Products.Insert(index, product);
                                // Пока обновлен только список, вносим изменения в БД
                                _products.Update(product);
                            }
                        }
                    }

                    //MessageBox.Show(department.ToString());
                }

            }
        }
    }
}

/*
 CRUD - (Create Read Update Delete) - концепция, согласно которой
 информационная система должна обеспечить эти 4 операции по отношению
 ко всем своим данным.
Create - добавление данных (Add, Insert) - создание новых инфо-единиц
Read   - отображение, извлечение данных из БД
Update - внесение изменений в уже существующие данные
Delete - удаление данных из БД. Особенность БД еще и в том, что
          удаление нельзя откатить (отменить). Поэтому одной из традиций
          является замена настоящего удаления введением дополнительного
          поля "deleted" (либо признак, либо дата удаления)
         Как вариант, ведется отдельная таблица удалений, в которой
          кроме даты отмечается кто удалил, причина удаления и т.п.
Задание: ограничить возможность введения пустого названия для
нового отдела
Д.З. Реализовать концепцию CRUD для работы с таблицей товаров (Products)
По аналогии с рассмотренными задачами с таблицей отделов (Departments)
 */
