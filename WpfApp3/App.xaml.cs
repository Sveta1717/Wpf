using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const int FIELD_SIZE_X = 8;  //  размер поля по горизонтали   
        public const int FIELD_SIZE_Y = 6;  //  размер поля по 
        public const String ConnecTionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\PC\source\repos\WpfApp3\WpfApp3\ADO\ADO121.mdf;Integrated Security=True";
        public const String ConnecTionStringEF = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\PC\source\repos\WpfApp3\WpfApp3\ADO\ADO121.mdf;Integrated Security=True";
    }
}
