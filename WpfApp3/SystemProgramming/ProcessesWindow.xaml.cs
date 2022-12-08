using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.IO.Enumeration;
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
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace WpfApp3.SystemProgramming
{
    /// <summary>
    /// Логика взаимодействия для ProcessesWindow.xaml
    /// </summary>
    public partial class ProcessesWindow : Window
    {
        public ObservableCollection<Process> Processes { get; set; }       

        public ProcessesWindow()
        {
            Processes = new();
            InitializeComponent();
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Process[] proc = Process.GetProcesses();  // получаем все системные процессы
            Processes.Clear();
            foreach (Process process in proc)        // переносим их в наблюдаемую коллекцию
            {
                Processes.Add(process);   
                //process.TotalProcessorTime
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if(item.Content is Process process)
                {
                    //MessageBox.Show(process.ProcessName);
                    try
                    {                      
                        ThreadsBlock.Text = String.Empty;
                        foreach (ProcessThread thread in process.Threads)
                        {
                            ThreadsBlock.Text += thread.Id + " " + thread.TotalProcessorTime + "\n";
                        }
                    }
                    catch
                    {
                        ThreadsBlock.Text = "Відмовлено у доступі";
                    }
                }
            }
        }

        private void NottepadeButton_Click(object sender, RoutedEventArgs e)
        {
            var notepad = Process.Start("notepad.exe");
           
        }

        private void ExplorerButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer", System.AppDomain.CurrentDomain.BaseDirectory);
        }

        private void StepButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("MicrosoftEdge.exe", "http://itstep.org");
        }       

        private void FileButton_Click(object sender, RoutedEventArgs e) 
        {            
           //Process.Start(new ProcessStartInfo { FileName = @"D:\", UseShellExecute = true });
            
            OpenFileDialog openFileDialog = new OpenFileDialog();
           
            if(openFileDialog.ShowDialog() == true) 
            {
                //Process.Start("notepad.exe", File.ReadAllText(openFileDialog.FileName));
                Process.Start("notepad.exe", @"C:\Users\");
            }            
            
        }
        

        private void NottepadeCloseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (Process proc in Process.GetProcessesByName("notepad"))
                {
                    proc.Kill();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        } 


        private void StartURL_Click(object sender, RoutedEventArgs e)
        {
            string name = URLEnter.Text;                               
            TextWriter textWriter = new StringWriter();
            textWriter.Write(name);
            Process.Start("MicrosoftEdge.exe", name);            
        }
    }
}
