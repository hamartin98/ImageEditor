using Emgu.CV;
using System;
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

            int rows = image.Rows;
            int cols = image.Cols;

            for (int row = 0; row < rows; ++row)
            {
                for (int col = 0; col < cols; ++col)
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
        public static Image<Bgr, byte> ShrinkHistogram(Mat original)
        {
            Image<Bgr, byte> image = original.ToImage<Bgr, byte>();
            double minL;
            double maxL;
            Bgr color;

            int rows = image.Rows;
            int cols = image.Cols;

            GetMinAndMaxLightness(image, out minL, out maxL);

            for (int row = 0; row < rows; ++row)
            {
                for (int col = 0; col < cols; ++col)
                {
                    color = image[row, col];
                    image[row, col] = HSL.GetEqualizedValue(minL, maxL, color);
                }
            }

            return image;
        }

        // Stretch images histogram based on the lightness values
        public static Image<Bgr, byte> StrectchHistogram(Mat original)
        {
            Image<Bgr, byte> image = original.ToImage<Bgr, byte>();
            double minL;
            double maxL;
            Bgr color;

            int rows = image.Rows;
            int cols = image.Cols;

            GetMinAndMaxLightness(image, out minL, out maxL);

            for (int row = 0; row < rows; ++row)
            {
                for (int col = 0; col < cols; ++col)
                {
                    color = image[row, col];
                    image[row, col] = HSL.StretchValue(color, maxL - minL);
                }
            }

            return image;
        }

        // Increase all pixel's hue with the given degree value
        public static Mat IncreaseHueBy(Mat original, double degree)
        {
            Image<Bgr, byte> image = original.ToImage<Bgr, byte>();
            int rows = image.Rows;
            int cols = image.Cols;
            Bgr color;

            for (int row = 0; row < rows; ++row)
            {
                for(int col = 0; col < cols; ++col)
                {
                    color = image[row, col];
                    color = HSL.IncreaseHue(color, degree);
                    image[row, col] = color;
                }
            }

            return image.Mat;
        }

        // Increase all pixel's saturation with the given percentage value
        public static Mat IncreaseSaturationBy(Mat original, double percentage)
        {
            Image<Bgr, byte> image = original.ToImage<Bgr, byte>();
            int rows = image.Rows;
            int cols = image.Cols;
            Bgr color;

            for (int row = 0; row < rows; ++row)
            {
                for (int col = 0; col < cols; ++col)
                {
                    color = image[row, col];
                    color = HSL.IncreaseSaturatiion(color, percentage);
                    image[row, col] = color;
                }
            }

            return image.Mat;
        }

        // Increase all pixel's lightness with the given percentage value
        public static Mat IncreaseLightnessBy(Mat original, double percentage)
        {
            Image<Bgr, byte> image = original.ToImage<Bgr, byte>();
            int rows = image.Rows;
            int cols = image.Cols;
            Bgr color;

            for (int row = 0; row < rows; ++row)
            {
                for (int col = 0; col < cols; ++col)
                {
                    color = image[row, col];
                    color = HSL.IncreaseLightness(color, percentage);
                    image[row, col] = color;
                }
            }

            return image.Mat;
        }
    }
}
