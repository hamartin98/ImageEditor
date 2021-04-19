﻿using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
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

        // Store the format of the image as a string e.g. jpg
        public string Format { get; private set; }

        // Store format - ImwriteFlags pairs to save images in the correct format
        private Dictionary<string, ImwriteFlags> saveFlags;

        // Initialize Image
        public ImageData(string path)
        {
            InitSaveFlags();
            SetData(path);
        }

        // Load image data from the given path
        private void SetData(string path)
        {
            Path = path;
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
        public void Save(string path, int quality = 100)
        {
            KeyValuePair<ImwriteFlags, int> flag =  new KeyValuePair<ImwriteFlags, int>(saveFlags[Format], quality);
            CvInvoke.Imwrite(path, Data, flag);
        }

        // Extract the format of the image from the path
        private void ExtractFormat()
        {
            int idx = Path.LastIndexOf('.');
            Format =  Path.Substring(idx + 1);
        }
    }
}
