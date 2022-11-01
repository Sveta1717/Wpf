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
        #region
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
        #endregion

        #region Inflation

        //Синхронное решение

        private void StartSync_Click(object sender, RoutedEventArgs e)
        {
            double sum = 100;
            Inflation.Text = "На початок року: " + sum;
            for(int i = 0; i < 12; i++)
            {
                sum += 1.1;  // +10%
                Thread.Sleep(100 + rnd.Next(100));
                Inflation.Text += String.Format(
                    "\nМісяць {0}, Усього {1}",
                    i, sum);
            }
        }

        //-------------- Многопоточное решение---------------

        Random rnd = new Random();

        double Sum;                   // "Общая" переменная, изменяемая разными потоками
        readonly object locker = new(); // объект для синхронизации

        int activeThreads;            // счетчик активных потоков
        readonly object locker2 = new(); // объект для синхронизации операций с activeThreads

        //  Метод, который будет работать в потоке

        private void AddMonth(object? data)
        {
            if (data is ThreadData threadData)
            {
                //имитируем затрату времени на запрос коэффициента инфляции
                double coef;
                Thread.Sleep(100 + rnd.Next(100));
                coef = 1 + rnd.NextDouble() / 5;
                double sum;
                lock (locker)  //синхро-блок, создающий транзакцию от чтения до записи
                {
                    // метод должен изменять общую сумму, значит переменная должна быть "общей"
                    sum = Sum; // получаем данные о текущем состоянии
                    sum *= coef; //рассчитываем результат
                    Sum = sum;  // сохраняем расчет в "общем" хранилищe
                }

                //напрямую обратиться к элементам окна нельзя, т.к. это другой поток 
                //Inflation.Text += String.Format("\n Усього {1}", sum);
                Dispatcher.Invoke(  // альтернатива (см. стр 42 - 44)
                    () => Inflation.Text += String.Format("\nМісяць {1} Усього {0}" , sum, threadData.Month));
                lock (locker2)
                {
                    //  за это время другой поток может еще уменьшить activeThreads
                    if (activeThreads == 0)
                    {
                        activeThreads--;
                        Dispatcher.Invoke(InflationComputed);
                    }
                }
            }
        }

        private void InflationComputed()  // завершение - расчет закончен
        {
            Inflation.Text += String.Format("\n------ Усього {0}", Sum);
            StartAsyncButton.IsEnabled = true; // разблокируем кнопку - работа завершена
        }

        private void StartASync_Click(object sender, RoutedEventArgs e)
        {            
            Sum = 100;     // начальная сумма
            Inflation.Text = "На початок року: " + Sum;
            int monthes = 12;
            activeThreads = monthes;
            for (int i = 0; i < monthes; i++)
            {
                // activeThreads++; опасно - если потоки быстро отрабатывают до повтора цикла
                // возможно уменьшение activeThreads -- из потока

                //  new Thread(AddMonth).Start() - без параметров
                new Thread(AddMonth).Start(       // параметры для метода передаются
                    new ThreadData { Month = i + 1} // в .Start
                    );
            }
            StartAsyncButton.IsEnabled = false; // блокируем кнопку до завершения всех потоков
        }  
        #endregion
    }
    class ThreadData
    {
        public int Month { get; set; }
    }
}


