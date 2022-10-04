using Microsoft.VisualBasic;
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
using System.Windows.Threading;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for MinesWindow.xaml
    /// </summary>
    public partial class MinesWindow : Window
    {
        private const String MINE_SYMBOL = "\x2622";
        private const String FREE_SYMBOL = "\x0DF4";
        private const String FLAG_SYMBOL = "\x2691";

        private Random random = new();

        private DispatcherTimer Timer;
        private DispatcherTimer Clock;
        private int time;

        public MinesWindow()
        {
            InitializeComponent();
            Timer = new() { Interval = new TimeSpan(0, 0, 0, 0, 20) };
            Timer.Tick += this.TimerTick;

            Clock = new() { Interval = new TimeSpan(0, 0, 0, 1) };
            Clock.Tick += this.ClockTick;

            Timer.Start();
            time = 0;
            Clock.Start();

            for (int y = 0; y < App.FIELD_SIZE_Y; y++)
            {
                for (int x = 0; x < App.FIELD_SIZE_X; x++)
                {
                    FieldLabel label = new()
                    {
                        X = x,
                        Y = y,
                        IsMine = random.Next(3) == 0 // random.Next (10).ToString();
                    };

                    label.Content = FREE_SYMBOL;     // label.IsMine ? "\x1F4A" : "\x2622";                   
                    label.FontSize = 20;

                    label.HorizontalContentAlignment = HorizontalAlignment.Center;
                    label.VerticalContentAlignment = VerticalAlignment.Center;
                    
                    label.Background = Brushes.Bisque;                       
                    label.Margin = new Thickness (1);

                    // Подключаем обработчик события
                    label.MouseLeftButtonUp += LabelClick;
                    label.MouseRightButtonUp += LabelRightClick;

                    //Регистрируем имя для элемента, по этому имени его
                    // можно будет найти(в другом коде)
                    this.RegisterName($"Label_{x}_{y}", label);

                    Field.Children.Add(label);
                }
            }
        }

        private bool IsWin()
        {
            foreach (var child in Field.Children)
            {
                if (child is FieldLabel label)
                {
                    if( ! label.IsMine && (label.Content.Equals(FREE_SYMBOL) ||
                        label.Content.Equals(FLAG_SYMBOL)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // левая кнопка мыши
        private void LabelClick(object sender, RoutedEventArgs e)
        {            
            if (sender is FieldLabel label)
            {
                //Если контент флажок, то не обрабатываем событие
                if (label.Content.Equals(FLAG_SYMBOL)) return;

                //Если мина - сообщение "Game over!", иначе кол-во мин отображаем на самой ячейке
                if (label.IsMine)
                {
                    label.Content = MINE_SYMBOL;
                    
                    if (MessageBoxResult.No ==
                    MessageBox.Show( "Play again?", "Game over!", button: MessageBoxButton.YesNo))
                    {
                        this.Close();                        
                    }                    
                    else
                    {
                        foreach(var child in Field.Children)
                        {
                            if(child is FieldLabel cell)
                            {
                                cell.Content = FREE_SYMBOL;
                                cell.IsMine = random.Next(3) == 0;
                                cell.Background = Brushes.Bisque;
                            }
                        }
                    }                    
                    return;
                }
                // определить имена всех соседей
                String[] names = // массив имен
                {
                    $"label_{label.X - 1}_{label.Y - 1}",
                    $"label_{label.X    }_{label.Y - 1}",
                    $"label_{label.X + 1}_{label.Y - 1}",
                    $"label_{label.X - 1}_{label.Y    }",
                    $"label_{label.X + 1}_{label.Y    }",
                    $"label_{label.X - 1}_{label.Y + 1}",
                    $"label_{label.X    }_{label.Y + 1}",
                    $"label_{label.X + 1}_{label.Y + 1}",
                };
                int mines = 0;
                foreach (String name in names)
                {
                    // Поиск элемента по имени, преобразование типа
                    //var neighbour = this.FindName(name) as FieldLablel;
                    //if(neighbour != null)
                    if (this.FindName(name) is FieldLabel neighbour)
                    {
                        if (neighbour.IsMine) mines += 1; //MessageBox.Show($"X:{neighbour.X}, Y:{neighbour.Y},Mine:{neighbour.IsMine}");
                    }
                    //MessageBox.Show("Нет соседа");

                    //if (lablel.IsMine == true)
                    //{
                    //    MessageBox.Show($"Це міна");
                    //}
                    //MessageBox.Show($"Мін немає");
                    //MessageBox.Show($"X = {lablel.X}, Y = {lablel.Y}, {lablel.IsMine}");                  
                }

                switch (mines)
                {
                    case 0: label.Background = Brushes.Lavender;break;
                    case 1: label.Background = Brushes.Red; break;
                    case 2: label.Background = Brushes.Orange; break;
                    case 3: label.Background = Brushes.Yellow; break;
                    case 4: label.Background = Brushes.Green; break;
                    case 5: label.Background = Brushes.Blue; break;
                    case 6: label.Background = Brushes.DarkBlue; break;
                    case 7: label.Background = Brushes.Violet; break;
                    default: break;
                }

                label.Content = mines.ToString();

                //Состояние поле изменилось - проверяем условие победы
                if (IsWin())
                {
                    MessageBox.Show("Play again?", "Game over!", button: MessageBoxButton.YesNo);
                }
            }
        }

        private void LabelRightClick(object sender, RoutedEventArgs e)
        {
            if (sender is FieldLabel label)
            {
                // Если контент - не закрытая ячейка, то не обрабатываем событие
                if (!label.Content.Equals(FREE_SYMBOL)
                  && !label.Content.Equals(FLAG_SYMBOL)) return;

                label.Content =
                    label.Content.Equals(FLAG_SYMBOL)
                    ? FREE_SYMBOL
                    : FLAG_SYMBOL;
            }
        }

        private void TimerTick(object? sender, EventArgs e)
        {           
        }

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

        class FieldLabel : Label
        {
            public int X { get; set; }
            public int Y { get; set; }
            public bool IsMine { get; set; }
        }
    }
}
