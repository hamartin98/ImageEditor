using Emgu.CV;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;
using Emgu.CV;

namespace ImageEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string originPath = null;
        private int width;
        private int height;

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

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

        private void btnCompress_Click(object sender, RoutedEventArgs e)
        {
            CompressImage();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            PixelizeEmgu();
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
                BitmapImage image = new BitmapImage(new Uri(fileName));

                imageContainer.Source = image;
                originPath = fileName;
                width = image.PixelWidth;
                height = image.PixelHeight;

                tbOriginPath.Text = originPath;
                tbResolution.Text = $"{width} x {height}";
            }
        }

        // Convert the pixels of the originalImage to Ascii characters based on the brightness of the pixel
        // Write the result into a file the open the file
        private void ConvertToAscii()
        {
            // Do nothing if there is no image opened
            if (originPath == null)
            {
                return;
            }

            using (StreamWriter writer = new StreamWriter("result.txt"))
            using (Bitmap image = new Bitmap(originPath))
            {
                const string chars = " .:-=+*#%@";
                Color color;
                double brightness;
                double charIdx;
                char pixelChar;

                for (int yIdx = 0; yIdx < height; yIdx++)
                {
                    for (int xIdx = 0; xIdx < width; xIdx++)
                    {
                        color = image.GetPixel(xIdx, yIdx);
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
            if (originPath == null)
            {
                return;
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();

            using (Bitmap image = new Bitmap(originPath))
            using (Bitmap result = new Bitmap(originPath))
            {
                for (int yIdx = 0; yIdx < height; yIdx += radius)
                {
                    for (int xIdx = 0; xIdx < width; xIdx += radius)
                    {
                        Color avgColor = GetAvgColor(xIdx, yIdx, radius);

                        for (int offsetX = 0; offsetX < radius; offsetX++)
                        {
                            for (int offSetY = 0; offSetY < radius; offSetY++)
                            {
                                if (xIdx + offsetX < width && yIdx + offSetY < height)
                                {
                                    result.SetPixel(xIdx + offsetX, yIdx + offSetY, avgColor);
                                }
                            }
                        }
                    }
                }

                resultContainer.Source = BitmapToImageSource(result);
            }

            watch.Stop();
            MessageBox.Show($"Elapsed time: {watch.ElapsedMilliseconds} ms");
        }


        // Pixelization by using the color of the pixel from the middle of the radius
        private void PixelizeCenter(int radius = 15)
        {
            if (originPath == null)
            {
                return;
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();

            using (Bitmap image = new Bitmap(originPath))
            using(Bitmap result = new Bitmap(originPath))
            {
                for (int yIdx = 0; yIdx < height; yIdx += radius)
                {
                    for (int xIdx = 0; xIdx < width; xIdx += radius)
                    {
                        if (xIdx + radius / 2 < width && yIdx + radius / 2 < height)
                        {
                            Color centerColor = image.GetPixel(xIdx + radius / 2, yIdx + radius / 2);

                            for (int offsetX = 0; offsetX < radius; offsetX++)
                            {
                                for (int offSetY = 0; offSetY < radius; offSetY++)
                                {
                                    if (xIdx + offsetX < width && yIdx + offSetY < height)
                                    {
                                        result.SetPixel(xIdx + offsetX, yIdx + offSetY, centerColor);
                                    }
                                }
                            }
                        }
                    }
                }

                resultContainer.Source = BitmapToImageSource(result);
            }

            watch.Stop();
            MessageBox.Show($"Elapsed time: {watch.ElapsedMilliseconds} ms");
        }

        // Returns the average color from the radius of the given pixel
        private Color GetAvgColor(int startX, int startY, int radius)
        {
            int rSum = 0;
            int gSum = 0;
            int bSum = 0;
            int count = 0;
            Color currColor;

            using(Bitmap image = new Bitmap(originPath))
            {
                for (int yIdx = startY; yIdx < startY + radius; yIdx++)
                {
                    for (int xIdx = startX; xIdx < startX + radius; xIdx++)
                    {
                        if (xIdx < image.Width && yIdx < image.Height)
                        {
                            currColor = image.GetPixel(xIdx, yIdx);
                            rSum += currColor.R;
                            gSum += currColor.G;
                            bSum += currColor.B;
                            count++;
                        }
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
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        // Return the correct codec info 
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            
            foreach(ImageCodecInfo codec in codecs)
            {
                if(codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }

        // Compress the image with the given quality
        // For now only works with jpeg files
        private void CompressImage(int quality = 40)
        {
            if(originPath == null)
            {
                return;
            }

            using(Bitmap image = new Bitmap(originPath))
            {
                ImageCodecInfo encoder = GetEncoder(ImageFormat.Jpeg);
                Encoder qualityEncoder = Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(qualityEncoder, quality);
                myEncoderParameters.Param[0] = myEncoderParameter;

                BitmapImage img = BitmapToImageSource(image);
                resultContainer.Source = img;

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = "jpg";
                saveFileDialog.Filter = "Jpg files|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|";
                saveFileDialog.AddExtension = true;

                if(saveFileDialog.ShowDialog() == true)
                {
                    string destPath = saveFileDialog.FileName;
                    image.Save(destPath, encoder, myEncoderParameters);
                }
            }
        }

        // Pixelize image with Emgu library to faster processing
        private void PixelizeEmgu(int radius = 4)
        {
            if (originPath == null)
            {
                return;
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();

            using (Image<Bgr, byte> image = new Image<Bgr, byte>(originPath))
            {
                int[] sums = new int[3] { 0, 0, 0 };
                int count = radius * radius;

                for (int yIdx = 0; yIdx < height; yIdx += radius)
                {
                    for (int xIdx = 0; xIdx < width; xIdx += radius)
                    {
                        sums = new int[3] { 0, 0, 0 };

                        for (int xOff = xIdx; xOff < xIdx + radius; xOff++)
                        {
                            for (int yOff = yIdx; yOff < yIdx + radius; yOff++)
                            {
                                if (xOff < width && yOff < height)
                                {
                                    Bgr color = image[yOff, xOff];
                                    sums[0] += (int)color.Blue;
                                    sums[1] += (int)color.Green;
                                    sums[2] += (int)color.Red;
                                }
                            }
                        }

                        Bgr avgColor = new Bgr(sums[0] / count, sums[1] / count, sums[2] / count);

                        for (int xOff = xIdx; xOff < xIdx + radius; xOff++)
                        {
                            for (int yOff = yIdx; yOff < yIdx + radius; yOff++)
                            {
                                if (xOff < width && yOff < height)
                                {
                                    image[yOff, xOff] = avgColor;
                                }
                            }
                        }
                    }
                }

                using (Bitmap bmp = image.ToBitmap())
                {
                    resultContainer.Source = BitmapToImageSource(bmp);
                }
            }

            watch.Stop();
            MessageBox.Show($"Elapsed time: {watch.ElapsedMilliseconds} ms");
        }
    }
}
