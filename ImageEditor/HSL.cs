using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace ImageEditor
{
    // Class to store and calculate HSL values
    class HSL
    {
        public double H { get; }
        public double S { get; }
        public double L { get; }

        public HSL(double H, double S, double L)
        {
            this.H = H;
            this.S = S;
            this.L = L;
        }

        // Calculate H, S, L values from RGB
        public static HSL ConvertFromRGB(int red, int green, int blue)
        {
            double blueH = blue / 255.0;
            double greenH = green / 255.0;
            double redH = red / 255.0;

            double cMin = Math.Min(blueH, Math.Min(greenH, redH));
            double cMax = Math.Max(blueH, Math.Max(greenH, redH));

            double delta = cMax - cMin;
            double H = 0;
            double S = 0;
            double L = (cMax - cMin) / 2.0;

            if (delta > Math.Abs(0.000001))
            {
                if (L <= 0.5)
                {
                    S = delta / (cMax + cMin);
                }
                else
                {
                    S = delta / (2 - cMax - cMin);
                }

                double rDiff = (cMax - redH) / delta;
                double gDiff = (cMax - greenH) / delta;
                double bDiff = (cMax - blueH) / delta;

                if (redH == cMax)
                {
                    H = bDiff - gDiff;
                }
                else if (greenH == cMax)
                {
                    H = 2 + rDiff - bDiff;
                }
                else
                {
                    H = 4 + gDiff - rDiff;
                }

                H *= 60;
                if (H < 0)
                {
                    H += 360;
                }
            }

            return new HSL(H, S, L);
        }


        // Returns the brightness of the color
        public static double GetLightness(Bgr color)
        {
            double tempB = color.Blue / 255.0;
            double tempG = color.Green / 255.0;
            double tempR = color.Red / 255.0;

            double min = Math.Min(tempB, Math.Min(tempG, tempR));
            double max = Math.Max(tempB, Math.Max(tempG, tempR));

            return (max - min) / 2.0;
        }

        public static Bgr GetEqualizedValue(double min, double max, Bgr color)
        {
            double blue = color.Blue;
            double green = color.Green;
            double red = color.Red;

            double oldValue = GetLightness(color);

            double newValue = (100 / (max - min)) * (oldValue);

            double diff = Math.Abs(oldValue - newValue) / 100;
            //MessageBox.Show($"old: {GetLightness(color)}, new: {newValue}");

            return new Bgr(blue + blue * diff, green + green * diff, red + red * diff);
        }

        public static Bgr StretchValue(Bgr color, double minDiff)
        {
            double blue = color.Blue;
            double green = color.Green;
            double red = color.Red;

            double oldValue = GetLightness(color);

            double newValue = 255 * (oldValue / minDiff);

            double diff = Math.Abs(oldValue - newValue) / 100;
            //MessageBox.Show($"old: {GetLightness(color)}, new: {newValue}");

            return new Bgr(blue + blue * diff, green + green * diff, red + red * diff);
        }

        // Override ToString method to dispay data as a string
        public override string ToString()
        {
            return $"HSL({H}, {S}, {L})";
        }
    }
}
