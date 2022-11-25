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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp3.SystemProgramming
{
    /// <summary>
    /// Логика взаимодействия для TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : Window
    {
        public TaskWindow()
        {
            InitializeComponent();
        }

        #region Пул потоків

        CancellationTokenSource cts;

        private void Start1_Click(object sender, RoutedEventArgs e)
        {
            Log1.Text = "Start";
            cts = new();
            for (int i = 0; i < 40; i++)
            {
                ThreadPool.QueueUserWorkItem(PoolWorker, new PoolWorkerData
                { 
                    Num = i,
                    CancellationToken = cts.Token
                });
            }
        }

        private void Stop1_Click(object sender, RoutedEventArgs e)
        {
            Log1.Text += "Stop";
            cts.Cancel();
        }

        private void PoolWorker(object? pars)
        {
            if (pars is PoolWorkerData data)
            {
                if(data.CancellationToken.IsCancellationRequested)
                {
                    return;
                }
                Thread.Sleep(1000);
                Dispatcher.Invoke(() => Log1.Text += $"..{data.Num}\t");
                
                // Log1.Text += $"..{pars}\t"); - как способ проверить , что это другой поток
            }
        }
        class PoolWorkerData
        {
            public int Num { get; set; }
            public CancellationToken CancellationToken { get; set; }
        }
        #endregion

        #region Багатозадачність
        private void Start2_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(Starter);
        }

        private void Stop2_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Starter()
        {
            Task t1 = new Task(() => TaskMethod0(20));  // Создание задачи t1 
            t1.Start();                                 // запуск t1 
                                                        // 
            Task t2 = Task.Run(() => TaskMethod0(10));  // Создание и запуск t2
                                                        // 
            t1.Wait();                                  // Ожидание окончания t1 
                                                        // 
            Task.Run(() => TaskMethod0(30));            // запуск t3 после t1 
        }

        private void TaskMethod0(int num)
        {
            Thread.Sleep(1000);
            Dispatcher.Invoke(() => Log2.Text += $"..{num}\t");
        }

        //-------------------------------------------

        private void Start3_Click(object sender, RoutedEventArgs e)
        {
            Log2.Text = "Start3";
            Task.Run(Starter3);
        }
        private void Starter3_Good()
        {
            Task<String> t1 = Task.Run(() => TaskMethod1(40));
            Task<String> t2 = Task.Run(() => TaskMethod1(50));
            Task<String> t3 = Task.Run(() => TaskMethod1(60));

            Dispatcher.Invoke(() => Log2.Text += t1.Result);
            Dispatcher.Invoke(() => Log2.Text += t2.Result);
            Dispatcher.Invoke(() => Log2.Text += t3.Result);
        }

        private void Starter3()
        {
            Task<String> t = Task.Run(() => TaskMethod1(40));
            String res = t.Result;
            Dispatcher.Invoke(() => Log2.Text += res);     // Такая схема
                                                           // приводит к
            t = Task.Run(() => TaskMethod1(50));           // последовательному
            res = t.Result;                                // выполнению всех
            Dispatcher.Invoke(() => Log2.Text += res);     // задач.
                                                           // res = t.Result; значит
            t = Task.Run(() => TaskMethod1(60));           // ожидание завершения
            res = t.Result;                                // задачи
            Dispatcher.Invoke(() => Log2.Text += res);     // 
        }

        private String TaskMethod1(int num)
        {
            Thread.Sleep(1000);
            //Task.Delay(1000);
            return $"tm1-{num}\t";
        }

        #endregion

        #region Домашне завдання

        private void START1_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(Starter_Consistent);            
        }       

        private void START2_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(Starter_Parallel);           
        }

        private Int32 Go(int num)
        {
            Thread.Sleep(100);
            //Task.Delay(1000);
            return num;
        }

        private void Starter_Parallel()
        {
            int i = 0;
            for (i = 0; i < 100; i++)
            {
                Thread.Sleep(100);                

                Dispatcher.Invoke(() => Progress2.Value++);
                Dispatcher.Invoke(() => Progress1.Value++);
                Dispatcher.Invoke(() => Progress3.Value++);
            }
        }
        private void START3_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(Starter_Mixed);
        }

        private void STOP_Click(object sender, RoutedEventArgs e)
        {            
            cts?.Cancel();           
        }

        private void Starter_Consistent()
        {
            int i = 0;
            for (i = 0; i < 100; i++)
            {
                Task<Int32> t1 = Task.Run(() => Go(0));
                Int32 res = t1.Result;
                Dispatcher.Invoke(() => Progress1.Value++);

                t1 = Task.Run(() => Go(10));
                res = t1.Result;
                Task<Int32> t2 = Task.Run(() => Go(0));
                Dispatcher.Invoke(() => Progress2.Value++);

                t2 = Task.Run(() => Go(20));
                res = t2.Result;
                Dispatcher.Invoke(() => Progress3.Value++);
            }
        }

        private void Starter_Mixed()
        {
            int i = 0;
            for (i = 0; i < 100; i++)
            {
                Task<Int32> t1 = Task.Run(() => Go(0));
                Int32 res = t1.Result;
                Dispatcher.Invoke(() => Progress1.Value++);

                t1 = Task.Run(() => Go(10));
                res = t1.Result;
                Task<Int32> t2 = Task.Run(() => Go(0));
                Dispatcher.Invoke(() => Progress2.Value++);                
            }
            for (i = 0; i < 100; i++)
            {
                Thread.Sleep(100);
                Dispatcher.Invoke(() => Progress3.Value++);
            }
        }
        #endregion        
    }
}
