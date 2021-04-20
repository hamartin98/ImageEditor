using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Emgu.CV.Structure;

namespace ImageEditor
{
    class ImageEffects
    {
        // Blur effect on the image with the given matrix radius
        public static void BlurEffect(Mat original, int radius)
        {
            CvInvoke.GaussianBlur(original, original, new Size(-1, -1), radius);
        }

        // Returns the smallest and biggest lightness values from the given image
        private static void GetMinAndMaxLightness(Image<Bgr, byte> image, out double min, out double max)
        {
            double minTemp = double.MaxValue;
            double maxTemp = double.MinValue;
            double lightness;

            for (int row = 0; row < image.Rows; row++)
            {
                for (int col = 0; col < image.Cols; col++)
                {
                    Bgr color = image[row, col];
                    lightness = HSL.GetLightness(color);
                    minTemp = Math.Min(minTemp, lightness);
                    maxTemp = Math.Max(maxTemp, lightness);
                }
            }

            min = minTemp;
            max = maxTemp;
        }

        // Map values between the lowest and highest lightness
        public static void ShrinkHistogram(Mat original)
        {
            using(Image<Bgr, byte> image = original.ToImage<Bgr, byte>())
            {
                double minL;
                double maxL;
                Bgr color;

                GetMinAndMaxLightness(image, out minL, out maxL);

                for (int row = 0; row < image.Rows; row++)
                {
                    for (int col = 0; col < image.Cols; col++)
                    {
                        color = image[row, col];
                        image[row, col] = HSL.GetEqualizedValue(minL, maxL, color);
                    }
                }

                original = image.Mat;

                CvInvoke.Imshow("Shrink", image);
            }
        }

        // Stretch images histogram based on the lightness values
        public static void StrectchHistogram(Mat original)
        {
            using (Image<Bgr, byte> image = original.ToImage<Bgr, byte>())
            {
                double minL;
                double maxL;
                Bgr color;

                GetMinAndMaxLightness(image, out minL, out maxL);

                for (int row = 0; row < image.Rows; row++)
                {
                    for (int col = 0; col < image.Cols; col++)
                    {
                        color = image[row, col];
                        image[row, col] = HSL.StretchValue(color, maxL - minL);
                    }
                }

                original = image.Mat;

                CvInvoke.Imshow("Stretch", image);
            }
        }
    }
}
