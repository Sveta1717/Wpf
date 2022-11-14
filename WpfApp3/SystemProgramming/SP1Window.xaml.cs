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
using System.Windows.Threading;

namespace WpfApp3.SystemProgramming
{
    /// <summary>
    /// Логика взаимодействия для SP1Window.xaml
    /// </summary>
    public partial class SP1Window : Window
    {
        public SP1Window()
        {
            InitializeComponent();
            Messages.Text = "";
        }

        #region Basics

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
            Messages.Text += message + "\n";
        }
        #endregion

        #region Inflation
        //Синхронное решение
        private void StartSync_Click(object sender, RoutedEventArgs e)
        {
            double sum = 100;
            Inflation.Text = "На початок року: " + sum;
            for (int i = 0; i < 12; i++)
            {
                sum += 1.1;  // +10%
                Thread.Sleep(100 + rnd.Next(100));
                Inflation.Text += String.Format(
                    "\nМісяць {0}, Усього {1}",
                    i, sum);
            }
        }

        // Многопоточное решение
        Random rnd = new Random();

        double Sum;                      // "Общая" переменная, изменяемая разными потоками
        readonly object locker = new();  // объект для синхронизации операций с Sum

        int activeThreads;               // счетчик активных потоков
        readonly object locker2 = new(); // объект для синхронизации операций с activeThreads


        // 1. Метод, который бдет работать в потоке
        private void AddMonth(object? data)
        {
            if (data is ThreadData threadData)   // конвертируем object в ThreadData
            {
                // имитируем затрату времени на запрос коэф. инфляции
                double coef;                         // эта часть кода не требует 
                Thread.Sleep(100 + rnd.Next(100));   // блокировки, т.к. не использует
                coef = 1 + rnd.NextDouble() / 5;     // общую переменную
                double sum;
                lock (locker)  // синхро-блок, создающий транзакцию от чтения до записи
                {
                    // метод должен изменять общую сумму, значит переменная должна быть "общей"
                    sum = Sum;         // получаем данные о текущем состоянии                                       
                    sum *= coef;       // расчитываем результат
                    Sum = sum;         // сохраняем расчет в "общем" хранилище                    
                }

                // Напрямую обратится к элементам окна нельзя, т.к. это другой поток
                // Inflation.Text += String.Format("\n Итог {0}", sum );
                Dispatcher.Invoke(   // альтернатива (см. строка 44)
                    () => Inflation.Text += String.Format("\nМесяц {1} Итог {0}", sum, threadData.Month)
                );

                lock (locker2)
                {
                    activeThreads--;
                    // за это время другой поток может еще уменьшить activeThreads
                    if (activeThreads == 0)
                    {
                        Dispatcher.Invoke(InflationComputed);
                    }
                }
            }
        }
        private void InflationComputed()  // завершение - расчет закончен
        {
            Inflation.Text += String.Format("\n--- Итог {0}", Sum);
            StartAsyncButton.IsEnabled = true;  // разблокируем кнопку - работа завершена
        }

        private void StartAsync_Click(object sender, RoutedEventArgs e)
        {
            Sum = 100;  // начальная сумма
            Inflation.Text = "На начало года: " + Sum;
            int monthes = 12;
            activeThreads = monthes;
            for (int i = 0; i < monthes; i++)
            {
                // activeThreads++; опасно - если потоки быстро отрабатывают, до повтора
                //  цикла возможно уменьшение activeThreads-- из потока

                // new Thread(AddMonth).Start();  -- без параметров
                new Thread(AddMonth).Start(           // параметры для метода передаются 
                    new ThreadData { Month = i + 1 }  //  в .Start()
                );
            }
            StartAsyncButton.IsEnabled = false;   // блокируем кнопку до завершения потоков
        }

        #endregion

        Thread worker;
        CancellationTokenSource tokenSource = new();

        private void Start1_Click(object sender, RoutedEventArgs e)
        {
            Progress1.Value = 0;
            worker = new Thread(Worker);
            tokenSource = new();
            CancellationToken token = tokenSource.Token;
            worker.Start(token);
        }

        private void Stop1_Click(object sender, RoutedEventArgs e)
        {
            // worker.Abort(); deprecated, не работает
            tokenSource.Cancel();
        }

        private void Worker(object? pars)
        {
            if (pars is CancellationToken token)
            {
                int i = 0;
                try
                {
                    for (i = 0; i < 100; i++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            //break; - просто остановка работы
                            token.ThrowIfCancellationRequested();
                        }
                        Thread.Sleep(100);
                        Dispatcher.Invoke(() => Progress1.Value++);
                    }
                }
                catch(OperationCanceledException)
                {
                    // поток остановлен - нужны завершающие действия
                    while(i > 0)
                    {
                        Thread.Sleep(100);
                        Dispatcher.Invoke(() => Progress1.Value--);
                        i--;
                    }
                }
            }
        }
    }

    class ThreadData  // для передачи данных в потоковый метод
    {
        public int Month { get; set; }
    }
}
