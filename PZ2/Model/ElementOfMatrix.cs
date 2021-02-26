using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PZ2.Model
{
    public class ElementOfMatrix
    {
        Canvas canvas;
        private PowerEntity powerEntity;
        private Shape shape;
        public long lineID = -1;
        float size;
        public bool IsLineOnElement;
        private int x, y;
        public bool IsVisited;
        public bool IsExamined;
        Action<long, Color, bool> lineClick;
        private SolidColorBrush sb;

        public ElementOfMatrix parent;

        public List<PowerEntity> powerEntities = new List<PowerEntity>();
        public Shape Shape { get => shape; set => shape = value; }
        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }

        public ElementOfMatrix(float size, Canvas canvas, int x, int y, Action<long, Color, bool> clickOnline)
        {
            this.lineClick = clickOnline;
            this.size = size;
            this.canvas = canvas;
            this.X = x;
            this.Y = y;
            shape = new Ellipse() { Width = size, Height = size };

            sb = new SolidColorBrush { Color = System.Windows.Media.Color.FromRgb(255, 255, 255) };
            shape.Fill = sb;
            canvas.Children.Add(shape);
            Canvas.SetLeft(shape, X - size * 3.5 / 2);
            Canvas.SetTop(shape, Y - size * 3.5 / 2);
        }

        public void Create(PowerEntity entity, Color color)
        {
            sb = new SolidColorBrush { Color = color };

            Shape.Width = size * 4f;
            Shape.Height = size * 4f;

            Canvas.SetLeft(shape, X - size * 3.5 / 2);
            Canvas.SetTop(shape, Y - size * 3.5 / 2);
            //Canvas.SetZIndex(Shape, 2);

            Shape.Fill = sb;
            this.powerEntities.Add(entity);
            Shape.MouseEnter += ToolTip;
            Shape.MouseLeftButtonDown += ZoomDot;
        }

        void ToolTip(object sender, RoutedEventArgs e)
        {
            Shape.ToolTip = MainWindow.ToopTip(powerEntities);
        }


        void ZoomDot(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 1;
            animation.To = 10;
            animation.Duration = new Duration(TimeSpan.FromSeconds(7));
            animation.AutoReverse = true;

            var ellipse = (Ellipse)e.Source;

            ScaleTransform st = new ScaleTransform();

            st.CenterX = 2.5;
            st.CenterY = 2.5;

            ellipse.RenderTransform = st;

            st.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
            st.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
        }

        public void AssignCross()
        {
            Shape.Height = 3;
            Shape.Width = 3;
            SolidColorBrush sb = new SolidColorBrush { Color = System.Windows.Media.Color.FromRgb(196, 0, 255) };
            Shape.Fill = sb;
            Canvas.SetLeft(Shape, X - size * 3.5 / 2);
            Canvas.SetTop(Shape, Y - size * 3.5 / 2);
        }

        public void LinePart(long lineID)
        {
            if (powerEntities.Count == 0)
                this.lineID = lineID;
        }

        public void ToolTipLine(object sender, RoutedEventArgs e)
        {
            Polyline pl = (Polyline)e.Source;
            ToolTip toolTip = new ToolTip();
            toolTip.Content = "LineEntity \n ID: "+this.lineID +"\n X: " +this.X + "\n Y: " +this.Y;
            pl.ToolTip = toolTip;
        }

        public void Draw(bool enable)
        {
            if (enable)
            {
                Shape.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
            else
            {
                Shape.Fill = sb;
            }
        }


        public void RightClick(object sender, RoutedEventArgs e)
        {
            if (lineID >= 0) //??
            {
                OnClick();
            }
        }
        async void OnClick()
        {
            lineClick(lineID, Color.FromRgb(255, 0, 0), true);
            await Delay();
            lineClick(lineID, Color.FromRgb(0, 0, 0), false);
        }

        async Task Delay()
        {
            await Task.Delay(7000);
        }
    }
}
