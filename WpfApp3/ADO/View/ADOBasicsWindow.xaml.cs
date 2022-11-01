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
using System.Data;
/*  NuGet Manager (Tools - NuGet - Manage )
 *  Поиск - SqlClient
 *  Выбираем - System.Data.SqlClient - Устанавливаем
 */
using System.Data.SqlClient;

namespace WpfApp3.ADO
{
    /// <summary>
    /// Логика взаимодействия для ADOBasicsWindow.xaml
    /// </summary>
    public partial class ADOBasicsWindow : Window
    {
        private const String ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\PC\source\repos\WpfApp3\WpfApp3\ADO\ADO121.mdf;Integrated Security=True";
        private SqlConnection connection; // объект - подключение

        public ADOBasicsWindow()
        {
            InitializeComponent();
            connection = new SqlConnection(ConnectionString);
            // !! в ADO создание объекта не открывает подключение
        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                connection.Open();
                MessageBox.Show("Підключення відкрито");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                connection.Close();
                MessageBox.Show("Підключення закрито");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonTimestamp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT CURRENT_TIMESTAMP", connection))
                {
                    MessageBox.Show(
                        cmd.ExecuteScalar()  // исполнение команды и возврат "скаляра" - одного рез-та
                        .ToString());
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
