using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Threading;

namespace ImageEditor
{
    static class Convolution
    {
        private static float[,] vertical = new float[,]
        {
            { 1f / 3, 0f, -1f / 3 },
            { 1f /3, 0f, -1f / 3 },
            { 1f / 3, 0f, -1f / 3 },
        };

        private static float[,] horizontal = new float[,]
        {
            {1f / 3, 1f / 3, 1f / 3},
            {0f, 0f, 0f},
            {-1f / 3, -1f / 3, -1f / 3}
        };

        private static float[,] laplace = new float[,]
        {
            { 0f, 1f, 0f},
            { 1f, -4f, 1f},
            { 0f, 1f, 0f}
        };

        private static float[,] sobel = new float[,]
        {
            { 1f / 16, 2f / 16, 1f / 16},
            { 2f / 16, 4f / 16, 2f / 16},
            { 1f / 16, 2f / 16, 1f / 16}
        };

        private static float[,] edge = new float[,]
        {
            { -1f, -1.0f, -1.0f},
            { -1f, 8f, -1f},
            { -1f, -1f, -1f}
        };

        // If the given coordinates are out of the image set them to the nearest edge of the image
        private static void CheckPixel(ref int x, ref int y, int xMax, int yMax)
        {
            x = Math.Min(xMax, Math.Max(0, x));
            y = Math.Min(yMax, Math.Max(0, y));
        }

        private static Image<Gray, byte> Convolve(Mat original, float[,] kernel)
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
                
                if (isColored)
                {
                    bgrResult = Colorize(result, color);
                }

                return bgrResult;
            }
        }

        // Adds up the pixels in the from the image1 and image2
        private static Image<Gray, byte> AddImages(Image<Gray, byte> image1, Image<Gray, byte> image2)
        {
            Image<Gray, byte> result = image1.CopyBlank();

            //CvInvoke.Add(image1, image2, result);

            int rows = image1.Rows;
            int cols = image1.Cols;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
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

            int cols = original.Cols;
            int rows = original.Rows;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
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

        // Paralell method for edge detection
        public static Image<Bgr, byte> ParallelEdgeDetection(Mat original, bool isColored, Bgr color)
        {
            Image<Gray, byte> gray = original.ToImage<Gray, byte>();
            Image<Gray, byte> verticalImage = gray.CopyBlank();
            Image<Gray, byte> horizontalImage = gray.CopyBlank();

            

            Thread thread1 = new Thread( () => { ParallelConvolution(gray, verticalImage, vertical); } );
            Thread thread2 = new Thread( () => { ParallelConvolution(gray, horizontalImage, horizontal); } );
            
            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            using (Image<Gray, byte> result = AddImages(verticalImage, horizontalImage))
            {
                Image<Bgr, byte> bgrResult = result.Convert<Bgr, byte>();

                if (isColored)
                {
                    bgrResult = Colorize(result, color);
                }

                return bgrResult;
            }
        }

        // Optimized Convolution method used in the ParallelEdgeDetection method
        private static void ParallelConvolution(Image<Gray, byte> image, Image<Gray, byte> result, float[,] kernel)
        {
            double sum;
            int x;
            int y;

            int rows = image.Height - 1;
            int cols = image.Width - 1;

            for (int row = 0; row < rows; ++row)
            {
                for (int col = 0; col < cols; ++col)
                {
                    sum = 0.0;

                    for (int yMod = -1; yMod < 2; ++yMod)
                    {
                        for (int xMod = -1; xMod < 2; ++xMod)
                        {
                            x = col + xMod;
                            y = row + yMod;
                            
                            x = Math.Min(cols, Math.Max(0, x));
                            y = Math.Min(rows, Math.Max(0, y));

                            sum = Math.Abs(sum + kernel[yMod + 1, xMod + 1] * image.Data[y, x, 0]);
                        }
                    }

                    result.Data[row, col, 0] = (byte)sum;
                }
            }
        }
    }
}
