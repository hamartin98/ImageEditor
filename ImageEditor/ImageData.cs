using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Drawing.Imaging;
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

        // Initialize Image
        public ImageData(string path)
        {
            SetData(path);
        }

        private void SetData(string path)
        {
            Path = path;
            Data = new Mat(path);
            Width = Data.Width;
            Height = Data.Height;
        }

        // Return as an Image to manipilate
        public Image<Bgr, byte> ToImage()
        {
            throw new NotImplementedException();
        }

        // Return the image as a bitmap
        public Bitmap ToBitmap()
        {
            throw new NotImplementedException();
        }

        // Return as a BitmapImage to display the image in an Image control
        public BitmapImage ToBitmapImage()
        {
            return ImageDataConverter.MatToBitmapImage(Data);
        }

        // Save the image to the specified location
        public void Save(string path)
        {
            Data.Save(path);
        }
    }
}
