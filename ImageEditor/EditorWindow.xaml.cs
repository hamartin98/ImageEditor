using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImageEditor
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        private ImageData imageData = null;

        public EditorWindow()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenImage();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveImage();
        }

        // Open an image from the computer and set the data of the imageData property
        private void OpenImage()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.Title = "Open images";
            fileDialog.Filter = "Image files|*.jpg;*.jpeg;*.png|" +
                                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                                "Portable Network Graphic (*.png)|*.png";

            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == true)
            {
                string fileName = fileDialog.FileName;
                imageData = new ImageData(fileName);

                UpdateImageContainer();
            }
        }

        // Save the image to the computer
        private void SaveImage()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "jpg";
            saveFileDialog.Filter = "Jpg files|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|";
            saveFileDialog.AddExtension = true;

            if (saveFileDialog.ShowDialog() == true)
            {
                string destPath = saveFileDialog.FileName;
                imageData.Save(destPath);
            }
        }

        // Show information about the image on the UI
        private void DisplayInfo()
        {
            tbPath.Text = imageData.Path;
            tbResolution.Text = imageData.ResolutionStr;
        }

        // Sets the image on the UI
        private void UpdateImageContainer()
        {
            BitmapImage bitmapImage = imageData.ToBitmapImage();
            imgMain.Source = bitmapImage;
            DisplayInfo();
            
        }
    }
}
