using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp3.ADO
{
    /// <summary>
    /// Логика взаимодействия для SystemProgrammingWindow.xaml
    /// </summary>
    public partial class SystemProgrammingWindow : Window
    {
        public SystemProgrammingWindow()
        {
            InitializeComponent();
            Messages.Text = "";
        }

        private void StartThread_Click(object sender, RoutedEventArgs e)
        {
           new Thread(ThreadMethod).Start();    
        }

        private void ShowMessage_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Повідомлення");
            Messages.Text += "Повідомлення\n";
        }

        private void ThreadMethod()
        {
            Dispatcher.Invoke(AddMessage, new object[] { "Start" });
            Thread.Sleep(2000);
            Dispatcher.Invoke(AddMessage, new object[] { "Stop" });           
        }

        private void AddMessage(String message)
        {
            Messages.Text +=  message + "\n";
        }
    }
}
