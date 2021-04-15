using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpenImage_Click(object sender, RoutedEventArgs e)
        {
            OpenImage();
        }

        // Open an image from the computer, then show it on the UI
        private void OpenImage()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.Title = "Open an image";
            fileDialog.Filter = "Image files|*.jpg;*.jpeg;*.png|" +
                                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                                "Portable Network Graphic (*.png)|*.png";

            fileDialog.RestoreDirectory = true;

            if(fileDialog.ShowDialog() == true)
            {
                imageContainer.Source = new BitmapImage(new Uri(fileDialog.FileName));
            }
        }
    }
}
