using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageEditor
{
    /// <summary>
    /// Interaction logic for DrawControl.xaml
    /// </summary>
    public partial class DrawControl : UserControl
    {
        private Point currentPoint = new Point();
        private List<Point> pointList = new List<Point>();

        public DrawControl()
        {
            InitializeComponent();
            canvas.Background = Brushes.White;
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                Line line = new Line();

                line.Stroke = Brushes.Red;
                line.StrokeThickness = 4;
                line.X1 = currentPoint.X;
                line.Y1 = currentPoint.Y;
                line.X2 = e.GetPosition(this).X;
                line.Y2 = e.GetPosition(this).Y;

                currentPoint = e.GetPosition(this);

                canvas.Children.Add(line);
                
            }
        }

        private void canvas_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                currentPoint = e.GetPosition(this);
                pointList.Add(e.GetPosition(this));
            }
        }

        private void DrawPoly()
        {
            Polygon polygon = new Polygon();
            polygon.Stroke = Brushes.Red;
            polygon.Points = new PointCollection(pointList);
            polygon.StrokeThickness = 3;
            canvas.Children.Add(polygon);
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
