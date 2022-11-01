using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.EntityFrameworkCore;
using WpfApp3.ADO.DAL;
using WpfApp3.ADO.EF;

namespace WpfApp3.ADO.View
{
    /// <summary>
    /// Логика взаимодействия для AdoEFWindow.xaml
    /// </summary>
    public partial class AdoEFWindow : Window
    {
        private readonly EF.FirmContext FirmContext;
        public ObservableCollection<Entities.Department> Departments { get; set; }
        public ObservableCollection<Entities.Product> Products { get; set; }
        public ObservableCollection<Entities.Manager> Managers { get; set; }

        public AdoEFWindow()
        {
            InitializeComponent();
            FirmContext = new();
            Departments = new();
            this.DataContext = this;
            Products = new();
            Managers = new();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // синтаксис методов
            LabelDepartments.Content =
                FirmContext.Departments.Count();
            //LabelProducts.Content = FirmContext.Products.Count();
            LabelManagers.Content = FirmContext.Managers.Count();

            // синтаксис запросов
            var cntQuery = from p in FirmContext.Products
                           where p.Price > 0
                           select p;
            LabelProducts.Content = cntQuery.Count();

            // Заполняем коллекцию
            var depQuery = from d in FirmContext.Departments
                           where d.Name != null
                           orderby d.Name descending
                           select d;
            foreach(var dep in depQuery)
            {
                Departments.Add(dep);
            }

            // другой синтаксис
            var query =
                FirmContext.Products
                .Where(p => p.Price > 0)
                .OrderBy(p => p.Name);

            //var prodQuery = from prod in FirmContext.Products
            //                orderby prod.Name descending
            //                orderby prod.Price descending
            //                select prod;
            foreach( var prod in query)
            {
                Products.Add(prod);
            }

            var manQuery = from man in FirmContext.Managers
                           orderby man.Name descending
                           orderby man.Surname descending
                           orderby man.SecName descending
                           orderby man.Id_sec_dep
                           orderby man.Id_main_dep
                           orderby man.Id_chief
                           select man;
            foreach(var man in manQuery)
            {
                Managers.Add(man);
            }
        }
    }
}

/*
 Entity Framework и идеология "Code first"
 Идеология основывается на том, что сначала описываются классы,
создается контекст (окружение), а База Данных создается автоматически
из анализа контекста и его классов.
 В противоположность "Data first" начинается с БД, а классы и 
контекст создаются по результатам анализа таблиц БД.
 Работу с данной идеологией обеспечивает Entity Framework (Core)
который нужно установить как дополнительные пакеты NuGet:
 Microsoft.EntityFrameworkCore - основа, набор инструментов
для создания и анализа контекста
 Microsoft.EntityFrameworkCore.SqlServer - драйверы для работы
с MS SQL Server
 Microsoft.EntityFrameworkCore.Tools - инструменты командной строки
для консоли пакетов
 */
