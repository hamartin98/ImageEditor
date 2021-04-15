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
        private Bitmap resultImage;

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

        private void btnPixelize_Click(object sender, RoutedEventArgs e)
        {
            PixelizeAvg();
        }

        private void btnPixelize2_Click(object sender, RoutedEventArgs e)
        {
            PixelizeCenter();
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
                resultImage = new Bitmap(fileName);
            }
        }

        // Convert the pixels of the originalImage to Ascii characters based on the brightness of the pixel
        // Write the result into a file the open the file
        private void ConvertToAscii()
        {
            // Do nothing if there is no image opened
            if (originalImage == null)
            {
                return;
            }

            using (StreamWriter writer = new StreamWriter("result.txt"))
            {
                const string chars = " .:-=+*#%@";
                Color color;
                double brightness;
                double charIdx;
                char pixelChar;

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
            double red = Math.Pow(color.R, 2) * 0.241;
            double green = Math.Pow(color.G, 2) * 0.691;
            double blue = Math.Pow(color.B, 2) * 0.068;

            return Math.Sqrt(red + green + blue);
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

        // Pixelize the image by averaging the color of the pixels in the given radius
        private void PixelizeAvg(int radius = 10)
        {
            if(originalImage == null)
            {
                return;
            }

            for(int yIdx = 0; yIdx < originalImage.Height; yIdx += radius)
            {
                for(int xIdx = 0; xIdx < originalImage.Width; xIdx += radius)
                {
                    Color avgColor = GetAvgColor(xIdx, yIdx, radius);

                    for (int offsetX = 0; offsetX < radius; offsetX++)
                    {
                        for (int offSetY = 0; offSetY < radius; offSetY++)
                        {
                            if (xIdx + offsetX < originalImage.Width && yIdx + offSetY < originalImage.Height)
                            {
                                resultImage.SetPixel(xIdx + offsetX, yIdx + offSetY, avgColor);
                            }
                        }
                    }
                }
            }

            resultContainer.Source = BitmapToImageSource(resultImage);
        }


        // Pixelization by using the color of the pixel from the middle of the radius
        private void PixelizeCenter(int radius = 15)
        {
            if (originalImage == null)
            {
                return;
            }

            for (int yIdx = 0; yIdx < originalImage.Height; yIdx += radius)
            {
                for (int xIdx = 0; xIdx < originalImage.Width; xIdx += radius)
                {
                    Color centerColor = originalImage.GetPixel(xIdx + radius / 2, yIdx + radius / 2);

                    for (int offsetX = 0; offsetX < radius; offsetX++)
                    {
                        for (int offSetY = 0; offSetY < radius; offSetY++)
                        {
                            if (xIdx + offsetX < originalImage.Width && yIdx + offSetY < originalImage.Height)
                            {
                                resultImage.SetPixel(xIdx + offsetX, yIdx + offSetY, centerColor);
                            }
                        }
                    }
                }
            }

            resultContainer.Source = BitmapToImageSource(resultImage);
        }

        // Returns the average color from the radius of the given pixel
        private Color GetAvgColor(int startX, int startY, int radius)
        {
            int rSum = 0;
            int gSum = 0;
            int bSum = 0;
            int count = 0;
            Color currColor;

            for(int yIdx = startY; yIdx < startY + radius; yIdx++)
            {
                for(int xIdx = startX; xIdx < startX + radius; xIdx++)
                {
                    if (xIdx < originalImage.Width && yIdx < originalImage.Height)
                    {
                        currColor = originalImage.GetPixel(xIdx, yIdx);
                        rSum += currColor.R;
                        gSum += currColor.G;
                        bSum += currColor.B;
                        count++;
                    }
                }
            }

            return Color.FromArgb(rSum / count, gSum / count, bSum / count);
        }

        // Convert Bitmap to ImageSource to display it in a Image cotrol
        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
