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
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace WpfApp3
{
    /// <summary>
    /// Логика взаимодействия для CanvasWindow.xaml
    /// </summary>
    public partial class CanvasWindow : Window
    {  
        // Таймер - инструмент создания хроно-событий или
        // периодического запуска функции/метода
        private DispatcherTimer Timer;
        private DispatcherTimer Clock;

        private int time;               // время игры
        private int Count_bonuses;      // кол-во бонусов

        private bool LeftKeyHold;       // признак удержания кнопки "влево"
        private bool RightKeyHold;      // признак удержания кнопки "вправо"
        private double ShipVelocity;

        private List<Rectangle> Bricks; // коллекция блоков
        private List<Rectangle> Bonuses;

       

        public CanvasWindow()
        {
            InitializeComponent();
            Timer = new() { Interval = new TimeSpan(0, 0, 0, 0, 15) };
            Timer.Tick += this.TimerTick;

            Clock = new() { Interval = new TimeSpan(0, 0, 0, 1) };
            Clock.Tick += this.ClockTick;


            Bricks = new();
            Bonuses = new();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // событие "готовности" окна. Работу с элементами UI (User Interfacce)
            // желательно реализовывать в этом событии
            Timer.Start();
            time = 0;
            Clock.Start();
            // создаем объект с данными (BallData), ссылку на него помещаем
            // в поле Tag объекта-шарика Ball ( <Ellipse x:Name="Ball"  )
            // Tag - специальное "резервное" поле для добавления своих данных
            Ball.Tag = new BallData { Vx = 2, Vy = -2 };
            ShipVelocity = 5;

            foreach (var child in Field.Children)  // обходим элементы Canvas
            {
                if (child is Rectangle rect)  // отбираем Прямоугольники
                {
                    if (rect != Ship)  // Ship - прямоугольник, не включаем в блоки
                    {
                        Bricks.Add(rect);  // добавляем блок в коллекцию
                    }
                }
            }
        }

        // Функция "часов" - отображает время
        private void ClockTick(object? sender, EventArgs e)
        {
            ++time;
            int h = time / 3600;         // часы
            int m = (time % 3600) / 60;  // минуты
            int s = time % 60;           // секунды
            String t = h.ToString("00") + ":" +
                m.ToString("00") + ":" + s.ToString("00");
            ClockLabel.Content = t;
        }

        // метод, который периодически запускается таймером
        private void TimerTick(object? sender, EventArgs e)
        {
            #region Движение шарика
            if (Ball.Tag is BallData ballData)
            {
                double ballX = Canvas.GetLeft(Ball);  // определяем координаты
                double ballY = Canvas.GetTop(Ball);   // шарика на холсте (Canvas)

                ballX += ballData.Vx;    // Движение - изменение координат
                ballY += ballData.Vy;    // 
                // ограничения движения
                if (ballX <= 0)  // выход за левую грань
                {
                    ballData.Vx = -ballData.Vx;  // меняем скорость по Х
                    ballX = 0;  // "подравниваем" если вышел за холст
                }
                if (ballY <= 0)
                {
                    ballData.Vy = -ballData.Vy;
                    ballY = 0;
                }
                if (ballX >= Field.ActualWidth - Ball.ActualWidth)
                {
                    ballData.Vx = -ballData.Vx;
                    ballX = Field.ActualWidth - Ball.ActualWidth;
                }
                // Окончательное падение
                if (ballY >= Field.ActualHeight - Ball.ActualHeight / 2)
                {
                    MessageBox.Show("Game Over");
                    this.Close();
                }
                // "Зона" ракетки, откуда возможно отбитие
                if (ballY >= Canvas.GetTop(Ship) - Ball.ActualHeight)
                {
                    double shipX = Canvas.GetLeft(Ship);
                    // находится ли шарик над ракеткой?
                    if (ballX + Ball.ActualWidth / 2 >= shipX
                     && ballX + Ball.ActualWidth / 2 <= shipX + Ship.ActualWidth)
                    {
                        ballData.Vy = -ballData.Vy;
                        ballY = Canvas.GetTop(Ship) - Ball.ActualHeight;
                    }
                }

                // проблема удаления - см. комментарии ниже
                Rectangle? removed = null;  // ссылка на удаляемый эл-т
                foreach (var brick in Bricks)
                {
                    #region Шарик - brick соударение
                    // Шарик - brick соударение
                    double brickX = Canvas.GetLeft(brick);
                    double brickY = Canvas.GetTop(brick);
                    if (ballX + Ball.ActualWidth / 2 >= brickX
                     && ballX + Ball.ActualWidth / 2 <= brickX + brick.ActualWidth)
                    {
                        if (ballY + Ball.ActualHeight >= brickY
                         && ballY + Ball.ActualHeight <= brickY + 2 * Math.Abs(ballData.Vy))
                        {
                            // Cверху
                            ballData.Vy = -ballData.Vy;
                            removed = brick;                                
                        }
                        if (ballY <= brickY + brick.ActualHeight
                         && ballY >= brickY + brick.ActualHeight - 2 * Math.Abs(ballData.Vy))
                        {
                            // Снизу
                            ballData.Vy = -ballData.Vy;
                            removed = brick;                           
                        }
                    }
                    else if (ballY + Ball.ActualHeight / 2 >= brickY
                          && ballY + Ball.ActualHeight / 2 <= brickY + brick.ActualHeight)
                    {
                        if (ballX + Ball.ActualWidth >= brickX
                         && ballX + Ball.ActualWidth <= brickX + 2 * Math.Abs(ballData.Vx))
                        {
                            // Слева
                            ballData.Vx = -ballData.Vx;
                            removed = brick;                            
                        }
                        if (ballX <= brickX + brick.ActualWidth
                         && ballX >= brickX + brick.ActualWidth - 2 * Math.Abs(ballData.Vx))
                        {
                            // Справа
                            ballData.Vx = -ballData.Vx;
                            removed = brick;                            
                        }
                    }
                    // Конец проверки соударения
                    #endregion
                }
                if (removed != null)
                {
                    // блок сбит - создаем "бонус"
                    var bonus = new Rectangle
                    {
                        Fill = removed.Fill,
                        Width = removed.Width,
                        Height = removed.Height
                    };
                    Bonuses.Add(bonus);
                    Field.Children.Add(bonus);
                    Canvas.SetLeft(bonus, Canvas.GetLeft(removed)); // применяем
                    Canvas.SetTop(bonus, Canvas.GetTop(removed));  // кллрдинаты

                    // удаляем блок из коллекции и с поля
                    Bricks.Remove(removed);
                    Field.Children.Remove(removed);
                }

                Canvas.SetLeft(Ball, ballX);   // применение новых
                Canvas.SetTop(Ball, ballY);    // координат


                #region Перемещение бонусов
                if(Bonuses.Count > 0)
                { 
                   foreach (var bonus in Bonuses)
                   {
                       double by = Canvas.GetTop(bonus);
                       by += 3;
                        if(by > Field.ActualHeight - bonus.ActualHeight / 2)
                        {
                            removed = bonus;
                        }

                        if(by >= Canvas.GetTop(Ship))
                        {
                            if (Canvas.GetLeft(bonus) + bonus.ActualWidth > Canvas.GetLeft(Ship)
                             && Canvas.GetLeft(bonus) < Canvas.GetLeft(Ship) + Ship.ActualWidth)
                            {
                                Count_bonuses++;                               
                                removed = bonus;
                                BonusesLabel.Content = Count_bonuses;
                            }
                        }
                       Canvas.SetTop(bonus, by);
                       /* Д.З. Арканоид: движение бонусов
                        * Обеспечить исчезновение бонусов 
                        * а) при пересечении с ракеткой
                        * б) при выходе за пределы поля
                        * ** Отобразить кол-во пойманных бонусов и время игры
                        */
                   }
                    if (removed != null)
                    {
                        Bonuses.Remove(removed);
                        Field.Children.Remove(removed);
                    }
                }
                #endregion

                if (Bonuses.Count == 0 && Bricks.Count == 0)
                {
                    MessageBox.Show("Україна понад усе! Перемога!");
                    Close();
                }
            }
            #endregion

            #region Движение каретки - обработка клавиатуры
            if (LeftKeyHold)  // Если удерживается стрелка "влево"
            {
                double x = Canvas.GetLeft(Ship);
                if (x > ShipVelocity) x -= ShipVelocity;
                else x = 0;
                Canvas.SetLeft(Ship, x);
            }
            if (RightKeyHold)  // Если удерживается стрелка "вправо"
            {
                double x = Canvas.GetLeft(Ship);
                if (x < Field.ActualWidth - Ship.ActualWidth - ShipVelocity)
                    x += ShipVelocity;
                else x = Field.ActualWidth - Ship.ActualWidth;
                Canvas.SetLeft(Ship, x);
            }
            #endregion
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // событие начала закрытия окна - здесь останавливаем таймер
            Timer.Stop();  // таймер, как системный ресурс, желательно останавливать            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) this.Close();
            else if (e.Key == Key.Left)
            {
                // double x = Canvas.GetLeft(Ship);// Обработка движения
                // x -= 3;                         // в событиях клавиатуры
                // Canvas.SetLeft(Ship, x);        // не рекомендуется
                LeftKeyHold = true;  // обработка - в таймере
            }
            else if (e.Key == Key.Right) this.RightKeyHold = true;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left) LeftKeyHold = false;
            else if (e.Key == Key.Right) this.RightKeyHold = false;
        }
    }

    // Слабее, чем наследование, связь агрегация: один объект ссылается на
    // другой объект.
    class BallData  // данные для шарика
    {
        public double Vx { get; set; }   // скорость по горизонтали
        public double Vy { get; set; }   // скорость по вертикали
    }
}

/* Задача: сбивать блок при попадании шарика
 * Проблема: информация о столкновении получается в цикле по коллекции
 * (Bricks), а "сбивать" блок - значит удалять его из коллекции.
 * !! Менять коллекцию в цикле по коллекции - запрещено
 * 
 */
