using Emgu.CV;
using System;
using System.Drawing;
using Emgu.CV.Structure;

namespace ImageEditor
{
    class ImageEffects
    {
        // Blur effect on the image with the given matrix radius
        public static Mat BlurEffect(Mat original, int radius)
        {
            Mat result = original.Clone();
            CvInvoke.GaussianBlur(original, result, new Size(-1, -1), radius);
            return result;
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
                    color = HSL.IncreaseSaturatiion(color, percentage / 100.0);
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
                    color = HSL.IncreaseLightness(color, percentage / 100.0);
                    image[row, col] = color;
                }
            }

            return image.Mat;
        }

        // Keeps the selected color and convert every other color into grayscale
        public static Mat SplashEffect(Mat original, Bgr desColor, int treshold, bool isReverse = false)
        {
            Image<Bgr, byte> result = original.ToImage<Bgr, byte>();
            
            int rows = result.Rows;
            int cols = result.Cols;
            Bgr currColor;
            Bgr grayColor;

            for (int row = 0; row < rows; ++row)
            {
                for (int col = 0; col < cols; ++col)
                {
                    currColor = result[row, col];

                    bool inRange = IsColorInRange(currColor, desColor, treshold);

                    if ((!inRange && !isReverse) || (inRange && isReverse))
                    {
                        grayColor = CalculateGrayColor(currColor);
                        result[row, col] = grayColor;
                    }
                }
            }

            return result.Mat;
        }

        // Convert the selected color into grayscale and keep every other color
        public static Mat ReverseSplashEffect(Mat original, Bgr desColor, int treshold)
        {
            return SplashEffect(original, desColor, treshold, true);
        }

        // Returns true if the given color is inside the given treshold value
        private static bool IsChannelInRange(double colorChannel, double desColorChannel, int treshold)
        {
            return Math.Abs(desColorChannel - colorChannel) <= treshold;
        }

        // Returns true if every channel value is in the given treshold value
        private static bool IsColorInRange(Bgr currColor, Bgr desColor, int treshold)
        {
            return IsChannelInRange(currColor.Blue, desColor.Blue, treshold) && 
                   IsChannelInRange(currColor.Green, desColor.Green, treshold) &&
                   IsChannelInRange(currColor.Red, desColor.Red, treshold);  
        }

        // Calcualte a grayscale value from the given color
        private static Bgr CalculateGrayColor(Bgr color)
        {
            int value = (int)(color.Blue * 0.11 + color.Green * 0.59 + color.Red * 0.3);
            return new Bgr(value, value, value);
        }
    }
}
