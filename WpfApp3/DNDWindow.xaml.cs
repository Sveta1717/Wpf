using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp3
{
    /// <summary>
    /// Логика взаимодействия для DNDWindow.xaml
    /// </summary>
    public partial class DNDWindow : Window
    {
        private bool LeftHold;      // признак удержания левой кнопки мыши (ЛКМ)
        private Point touch;        // точка курсора мыши в момент захвата объекта
        private Rectangle Phantom;  // временная копия объекта для перетаскивания
        private Rectangle Source;   //  исходный элемент, который "копируется"
        private string filepath = @"C:\Users\PC\Desktop\bricks.json";

        public DNDWindow()
        {
            InitializeComponent();
            Phantom = null!;      // null-forgiving (!) - мы уверены, что допускаем null
            Source = null!;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LeftHold = false;
            Phantom = null!;
            Source = null!;
        }

        private void Brick_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /* Суть перетаскивания - в создании копии исходного элемента (фантома)
            * и перемещения его до момента отпускания. Однако, фантом обычно
            * создается не в момент нажатия, а в момент первого движения с 
            * нажатой кнопкой.
            */
            Source = (sender as Rectangle)!;
            if (e.ChangedButton == MouseButton.Left) // событие MouseDown - для всех кнопок, поєтому фильтруем
            {
                LeftHold = (Source != null);  // булевская переменная вернет или да или нет
                touch = e.GetPosition(Source);
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                // На ПКМ поставим удаление блока (кроме исходных)
                if (Source != Brick1 && Source != Brick2)
                {
                    Field.Children.Remove(Source);
                }
            }
        }

        // исходные блоки копируются, а новые блоки перемещаются сами
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (LeftHold)
            {
                Point point = e.GetPosition(Field);
                Title = point.X + " " + point.Y;
                //Title = e.GetPosition(this).X + "" + e.GetPosition(this).Y;
                if (Phantom == null)  // движение есть, а фантома нет - это первое движение
                {
                    if (Source == Brick1 || Source == Brick2)
                    {
                        Phantom = new Rectangle   // задаем параметры фантома
                        {
                            Fill    = Source.Fill,                            
                            Height  = Source.Height,
                            Opacity = .5,
                            Stroke  = Source.Stroke,
                            StrokeThickness = Source.StrokeThickness,
                            Width   = Source.Width
                        };
                        Field.Children.Add(Phantom); // добавляем фантом
                    }
                    else
                    {
                        Phantom = Source;
                        Phantom.Opacity = .5;
                    }
                }
                Canvas.SetLeft(Phantom, point.X - touch.X); // координаты на Canvas
                Canvas.SetTop(Phantom, point.Y - touch.Y);
            }
        }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Phantom != null)
            {
                Phantom.Opacity = 1;
                Phantom.MouseDown += Brick_MouseDown;

                LeftHold = false;
                Phantom = null!;
            }
        }

        #region Menu
        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            /* Сериализация (от англ. serial - последовательный) - представление объекта
            * в виде последовательности данных (бинарных или текстовых) обычно для 
            * сохранения или передачи по обменному каналу
            * В С# популярны три сериализатора: Binary(старый, не рекомендуется)
            * XML, JSON (System.Text, Newtonsoft)
            */
            List<BrickData> bricks = new();
            foreach (var child in Field.Children)
            {
                if (child is Rectangle brick)
                {
                    if (brick != Brick1 && brick != Brick2)
                    {
                        bricks.Add(new()
                        {
                            Height = brick.Height,
                            Width  = brick.Width,
                            Left   = Canvas.GetLeft(brick),
                            Top    = Canvas.GetTop(brick),
                            Type   = (brick.Fill.Equals(Brushes.Salmon)) ? 1 : 2
                        });
                    }
                }
            }
            String json = JsonSerializer.Serialize(bricks);
           
            File.WriteAllText(filepath, json);            
            MessageBox.Show("Файл збережено");
            // "[{\"Width\":70,\"Height\":20,\"Left\":337,\"Top\":101.99999999999997,\"Type\":2},{\"Width\":70,\"Height\":20,\"Left\":337,\"Top\":136.99999999999997,\"Type\":1},{\"Width\":70,\"Height\":20,\"Left\":336,\"Top\":175.99999999999997,\"Type\":2}]"
        }

        private void MenuLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                jsonfile.AcceptsReturn = true;
            }
            else
            {
                return;
            }                
               String json = File.ReadAllText(filepath);//"[{\"Width\":70,\"Height\":20,\"Left\":337,\"Top\":101.99999999999997,\"Type\":2},{\"Width\":70,\"Height\":20,\"Left\":336,\"Top\":175.99999999999997,\"Type\":2}]";
               BrickData[] bricks = JsonSerializer.Deserialize<BrickData[]>(json)!;
            if (bricks == null)
            {
                MessageBox.Show("Помилка завантаження");
            }
            else
            {
                // Очистить поле (кроме исходных)
                List<UIElement> toRemove = new();
                foreach (var child in Field.Children)
                {
                    if (child is Rectangle brick)
                    {
                        if (brick != Brick1 && brick != Brick2)
                        {
                            toRemove.Add(brick);
                        }
                    }
                }

                foreach (var child in toRemove)
                {
                    Field.Children.Remove(child);
                }
                // Вставить новые элементы
                foreach (var brick in bricks)
                {
                    Rectangle r = new()
                    {
                        Width  = brick.Width,
                        Height = brick.Height,
                        Fill   = (brick.Type == 1) ? Brushes.Salmon : Brushes.Lime,
                        Stroke = (brick.Type == 1) ? Brushes.Maroon : Brushes.Orange,
                        StrokeThickness = 2
                    };
                    Canvas.SetLeft(r, brick.Left);
                    Canvas.SetTop(r, brick.Top);
                    r.MouseDown += Brick_MouseDown;

                    Field.Children.Add(r);
                }
            }
        }

        private void MenuClose_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("До побачення!");            
            Close();
        }
        #endregion
        /* Д.З. Реализовать сохранение данных в файл (сериализация) и выгрузку из файла
        *  (десериализация). * Организовать выбор имени файла при помощи диалога
        * Экзамен: завершить все задания, приложить архив проекта (ссылку на репозиторий) 
        */
    }

    class BrickData
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public double Type { get; set; }
    }
}

/* Drag n'Drop (DnD)
Визуальный прием перетаскивания элементов мышью
Реализация приема состоит в следующем:
 - элемент, который поддерживает DnD, обрабатывает нажатие мыши
 - пространство, в котором возможно перетаскивание, поддерживает
  = перемещение мыши
  = отпускание мыши
*/
