using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageEditor
{
    class ImageData
    {
        public string Path { get; private set; }

        public int Width { get; private set; }
        
        public int Height { get; private set; }

        // Store the data of the image
        public Mat Data { get; private set; } = null;

        // Return the dimensions of the image as a string
        public string ResolutionStr => $"{Width} x {Height} ";

        // Store the format of the image as a string e.g. jpg
        public string Format { get; private set; }

        // Store format - ImwriteFlags pairs to save images in the correct format
        private Dictionary<string, ImwriteFlags> saveFlags;

        // returns true if the data is not null
        public bool IsDataSet 
        { 
            get => Data != null;
        }

        public ImageData()
        {
            InitSaveFlags();
        }

        // Load image data from the given path
        public void SetData(string path)
        {
            Path = path;
            if (Data != null)
            {
                Data.Dispose();
            }
            GC.Collect();
            Data = CvInvoke.Imread(path);
            Width = Data.Width;
            Height = Data.Height;
            ExtractFormat();
        }


        // Initialize saveFlag dictionary
        private void InitSaveFlags()
        {
            saveFlags = new Dictionary<string, ImwriteFlags>();

            saveFlags.Add("jpg", ImwriteFlags.JpegQuality); // 1 - 100, 1 == full compression
            saveFlags.Add("png", ImwriteFlags.PngCompression); // 0 - 9, 9 == full compression
        }

        // Return the Mat as an Image to manipulate it
        public Image<Bgra, byte> ToImage()
        {
            return Data.ToImage<Bgra, byte>();
        }

        // Return the image as a bitmap
        public Bitmap ToBitmap()
        {
            return Data.ToBitmap();
        }

        // Return as a BitmapImage to display the image in an Image control
        public BitmapImage ToBitmapImage()
        {
            return ImageDataConverter.MatToBitmapImage(Data);
        }

        // Save image without compression
        public void Save(string fullPath)
        {
            Save(fullPath, 100);
        }

        // Save the image to the specified location
        public void Save(string fullPath, int quality = 100, string format = "")
        {
            if (Data != null)
            {
                Format = format == "" ? Format : format;
                QualityMapper(ref quality);

                KeyValuePair<ImwriteFlags, int> flag = new KeyValuePair<ImwriteFlags, int>(saveFlags[Format], quality);
                CvInvoke.Imwrite(fullPath, Data, flag);
            }
        }

        // Extract the format of the image from the path
        private void ExtractFormat()
        {
            int idx = Path.LastIndexOf('.');
            Format =  Path.Substring(idx + 1);
        }

        // Return the quality value based on the file format
        private void QualityMapper(ref int quality)
        {
            if (Format == "png") // map (1, 100) to (9, 0)
            {
                int value = (100 - quality) / 11;
                quality = Math.Max(0, value); // don't go below zero
            }
        }

        // Blur the image with the given radius
        public void BlurEffect(int radius)
        {
            if(Data != null)
            {
                Data = ImageEffects.BlurEffect(Data, radius);
            }
        }

        // Apply histogram shrinking on the image
        public void ShrinkHistogram()
        {
            if (Data != null)
            {
                Data = ImageEffects.ShrinkHistogram(Data).Mat;
            }
        }

        // Apply histogram stretching on the image
        public void StretchHistogram()
        {
            if (Data != null)
            {
                Data = ImageEffects.StrectchHistogram(Data).Mat;
            }
        }

        // Increase image's hue by the given degree
        public void IncreaseHue(double degree)
        {
            if (Data != null)
            {
                Data = ImageEffects.IncreaseHueBy(Data, degree);
            }
        }

        // Increase image's saturation by the given percentage
        public void IncreaseSaturation(double percentage)
        {
            if (Data != null)
            {
                Data = ImageEffects.IncreaseSaturationBy(Data, percentage);
            }
        }

        // Increase image's saturation by the given percentage
        public void IncreaseLightness(double percentage)
        {
            if (Data != null)
            {
                Data = ImageEffects.IncreaseLightnessBy(Data, percentage);
            }
        }

        // Detect edges on the image, if iscolored is true, the detected edges color will be set to the given color
        public void EdgeDetection(bool isColored, Bgr color)
        {
            if (Data != null)
            {
                Data = Convolution.ParallelEdgeDetection(Data, isColored, color).Mat;
            }
        }
    }
}
