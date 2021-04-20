using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.Util;
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

        // Save the image to the specified location
        public void Save(string path, int quality = 100)
        {
            QualityMapper(ref quality);
            KeyValuePair<ImwriteFlags, int> flag =  new KeyValuePair<ImwriteFlags, int>(saveFlags[Format], quality);
            CvInvoke.Imwrite(path, Data, flag);
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
            ImageEffects.BlurEffect(Data, radius);
        }

        // Apply histogram shrinking on the image
        public void ShrinkHistogram()
        {
            ImageEffects.ShrinkHistogram(Data);
        }

        // Apply histogram stretching on the image
        public void StretchHistogram()
        {
            ImageEffects.StrectchHistogram(Data);
        }
    }
}
