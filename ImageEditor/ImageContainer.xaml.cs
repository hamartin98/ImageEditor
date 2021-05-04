using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageEditor
{
    /// <summary>
    /// Interaction logic for ImageContainer.xaml
    /// </summary>
    public partial class ImageContainer : UserControl
    {
        public ImageContainer()
        {
            InitializeComponent();

            border.RenderTransformOrigin = new Point(0.5, 0.5);
            border.RenderTransform = new TranslateTransform(2.0, 2.0);
            
        }

        private TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform).Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform).Children.First(tr => tr is ScaleTransform);
        }

        private void btnZoom_Click(object sender, RoutedEventArgs e)
        {
            content.RenderTransform = new ScaleTransform(2.0, 2.0);
        }

        private void content_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double rate = e.Delta / 120 * 0.2;
            double newX = content.LayoutTransform.Value.M11 + rate;
            double newY = content.LayoutTransform.Value.M22 + rate;

            if(newX > 0.2 && newY > 0.2)
            {
                ScaleTransform scale = new ScaleTransform(newX, newY);
                content.LayoutTransform = scale;
            }
        }

        private void content_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                Debug.WriteLine($"{e.GetPosition(content).ToString()}");
            }
        }

        private void content_DragEnter(object sender, DragEventArgs e)
        {
            Debug.WriteLine($"{e.GetPosition(content).ToString()}");

            List<Point> pointsList = new List<Point>();
            pointsList.Add(e.GetPosition(content));
            Polygon polygon = new Polygon();
            polygon.Points = new PointCollection(pointsList);

        }
    }
}
