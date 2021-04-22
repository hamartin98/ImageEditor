using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageEditor
{
    class Convolution
    {
        private static double[,] vertical = new double[,]
            {
                { 1.0 / 3, 0.0, -1.0 / 3 },
                { 1.0 /3, 0.0, -1.0 / 3 },
                { 1.0 / 3, 0.0, -1.0 / 3 },
            };

        private static double[,] horizontal = new double[,]
        {
                {1.0 / 3, 1.0 / 3, 1.0 / 3},
                {0.0, 0.0, 0.0},
                {-1.0 / 3, -1.0 / 3, -1.0 / 3}
        };

        private static double[,] laplace = new double[,]
        {
                { 0, 1, 0},
                { 1, -4, 1},
                { 0, 1, 0}
        };

        private static double[,] sobel = new double[,]
        {
                { 1.0 / 16, 2.0 / 16, 1.0 / 16},
                { 2.0 / 16, 4.0 / 16, 2.0 / 16},
                { 1.0 / 16, 2.0 / 16, 1.0 / 16}
        };

        private static double[,] edge = new double[,]
        {
                { -1, -1, -1},
                { -1, 8, -1},
                { -1, -1, -1}
        };

        // If the given coordinates are out of the image set them to the nearest edge of the image
        private static void CheckPixel(ref int x, ref int y, int xMax, int yMax)
        {
            x = Math.Min(xMax, Math.Max(0, x));
            y = Math.Min(yMax, Math.Max(0, y));
        }

        private static Image<Gray, byte> Convolve(Mat original, double[,] kernel)
        {
            Image<Gray, byte> image = original.ToImage<Gray, byte>();
            Image<Gray, byte> dest = image.CopyBlank();

            double sum;
            int x;
            int y;

            for (int row = 0; row < image.Rows - 1; row++)
            {
                for (int col = 0; col < image.Cols - 1; col++)
                {
                    sum = 0.0;

                    for (int yMod = -1; yMod < 2; yMod++)
                    {
                        for (int xMod = -1; xMod < 2; xMod++)
                        {
                            x = col + xMod;
                            y = row + yMod;
                            CheckPixel(ref x, ref y, image.Cols, image.Rows);

                            sum = Math.Abs(sum + kernel[yMod + 1, xMod + 1] * image.Data[y, x, 0]);
                        }
                    }

                    dest.Data[row, col, 0] = (byte)sum;
                }
            }

            image.Dispose();
            return dest;
        }

        // Detect edges on a grayscale image, the color it with the given color if the isColored parameter is true
        public static Image<Bgr, byte> EdgeDetection(Mat original, bool isColored, Bgr color)
        {
            using (Image<Gray, byte> verticalImage = Convolve(original, vertical))
            using (Image<Gray, byte> horizontalImage = Convolve(original, horizontal))
            using (Image<Gray, byte> result = AddImages(verticalImage, horizontalImage))
            {
                Image<Bgr, byte> bgrResult = result.Convert<Bgr, byte>();
                //Image<Gray, byte> result = verticalImage;

                CvInvoke.Imshow("Normal", bgrResult);

                if (isColored)
                {
                    bgrResult = Colorize(result, color);
                }

                CvInvoke.Imshow("Colorized", bgrResult);

                return bgrResult;
            }
        }

        // Adds up the pixels in the from the image1 and image2
        private static Image<Gray, byte> AddImages(Image<Gray, byte> image1, Image<Gray, byte> image2)
        {
            Image<Gray, byte> result = image1.CopyBlank();

            //CvInvoke.Add(image1, image2, result);

            for (int row = 0; row < image1.Rows; row++)
            {
                for (int col = 0; col < image2.Cols; col++)
                {
                    //result.Data[row, col, 0] = (byte)Math.Sqrt(Math.Pow(image1.Data[row, col, 0], 2) + Math.Pow(image1.Data[row, col, 0], 2));
                    result.Data[row, col, 0] = (byte)(Math.Abs(image1.Data[row, col, 0] + image1.Data[row, col, 0]));
                }
            }

            return result;
        }

        // Change the color of the detected edges
        private static Image<Bgr, byte> Colorize(Image<Gray, byte> original, Bgr color)
        {
            Image<Bgr, byte> colorized = new Image<Bgr, byte>(original.Size);
            Bgr black = new Bgr(0, 0, 0);

            for (int row = 0; row < original.Rows; row++)
            {
                for (int col = 0; col < original.Cols; col++)
                {
                    if (original.Data[row, col, 0] > 10)
                    {
                        colorized[row, col] = color;
                    }
                    else
                    {
                        colorized[row, col] = black;
                    }
                }
            }

            return colorized;
        }
    }
}
