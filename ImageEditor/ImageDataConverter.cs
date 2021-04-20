using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace ImageEditor
{
    class ImageDataConverter
    {
        // Convert Bitmap to BitmapImage
        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
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

        // Convert Bitmap to BitmapImage with downscaling for efficiency
        public static BitmapImage BitmapToBitmapImageScale(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            using (Bitmap resized = new Bitmap(bitmap, new Size(bitmap.Width / 2, bitmap.Height / 2)))
            {
                resized.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        // Convert a Mat to BitmapImage to display on the UI
        public static BitmapImage MatToBitmapImage(Mat source)
        {
            using(Bitmap bitmap = source.ToBitmap())
            {
                //return BitmapToBitmapImage(bitmap);
                return BitmapToBitmapImageScale(bitmap);
            }
        }
    }
}
