using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Bitmap originalImage;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpenImage_Click(object sender, RoutedEventArgs e)
        {
            OpenImage();
        }

        private void btnConvertToAscii_Click(object sender, RoutedEventArgs e)
        {
            ConvertToAscii();
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
                string fileName = fileDialog.FileName;

                // Show the opened image on the ui
                imageContainer.Source = new BitmapImage(new Uri(fileName));

                // Store the image as a Bitmap to manipulate it
                originalImage = new Bitmap(fileName);
            }
        }

        // Convert the pixels of the originalImage to Ascii characters based on the brightness of the pixel
        // Write the result into a file the open the file
        private void ConvertToAscii()
        {
            const string chars = " .:-=+*#%@";
            Color color;
            double brightness;
            double charIdx;
            char pixelChar;

            using (StreamWriter writer = new StreamWriter("result.txt"))
            {
                for (int yIdx = 0; yIdx < originalImage.Height; yIdx++)
                {
                    for (int xIdx = 0; xIdx < originalImage.Width; xIdx++)
                    {
                        color = originalImage.GetPixel(xIdx, yIdx);
                        brightness = GetBrightness(color);
                        charIdx = brightness / 255 * (chars.Length - 1);
                        pixelChar = chars[(int)Math.Round(charIdx)];
                        writer.Write(pixelChar);
                    }
                    writer.Write("\n");
                }
            }

            ShowTxtFile("result.txt");
        }

        // Returns the brightness of the given color
        private double GetBrightness(Color color)
        {
            return Math.Sqrt(color.R * color.R * 0.241 + color.G * color.G * 0.691 + color.B * color.B * 0.068);
        }

        // Open the txt file from the given path with the default text editor
        private void ShowTxtFile(string path)
        {

            var process = new ProcessStartInfo(path)
            {
                Arguments = Path.GetFileName(path),
                UseShellExecute = true,
                Verb = "OPEN",
            };

            Process.Start(process);
        }
    }
}
