using Emgu.CV.Structure;
using System;

namespace ImageEditor
{
    // Class to store and calculate HSL values
    class HSL
    {
        public double H { get; private set; }
        public double S { get; private set; }
        public double L { get; private set; }

        public HSL(double H, double S, double L)
        {
            this.H = H;
            this.S = S;
            this.L = L;
        }

        // Increase the hue value of the color
        // 0 - 360°
        public void HueIncreaseBy(double degree)
        {
            this.H = Math.Max(0, Math.Min(360, H + degree));
        }

        // Increase the saturation value of the color
        // 0 - 1
        public void SaturationIncreaseBy(double degree)
        {
            this.S = Math.Max(0.0, Math.Min(1.0, H + degree));
        }

        // Increase the lightness value of the color
        // 0 - 1
        public void LightnessIncreaseBy(double degree)
        {
            this.L = Math.Max(0.0, Math.Min(1.0, H + degree));
        }

        // Calculate H, S, L values from RGB
        public static HSL ConvertFromBgr(int blue, int green, int red)
        {
            double blueH = blue / 255.0;
            double greenH = green / 255.0;
            double redH = red / 255.0;

            double min = Math.Min(blueH, Math.Min(greenH, redH));
            double max = Math.Max(blueH, Math.Max(greenH, redH));

            double delta = max - min;
            double H = 0;
            double S = 0;
            double L = (max - min) / 2.0;

            if (delta > Math.Abs(0.000001))
            {
                if (L <= 0.5)
                {
                    S = delta / (max + min);
                }
                else
                {
                    S = delta / (2 - max - min);
                }

                double rDiff = (max - redH) / delta;
                double gDiff = (min - greenH) / delta;
                double bDiff = (max - blueH) / delta;

                if (redH == max)
                {
                    H = bDiff - gDiff;
                }
                else if (greenH == max)
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

        public static HSL ConvertFromBgr(Bgr color)
        {
            return ConvertFromBgr((int)color.Blue, (int)color.Green, (int)color.Red);
        }


        // Convert HSL color to Bgr color
        public static Bgr ConvertToBgr(HSL hslColor)
        {
            int blue = (int)(hslColor.L * 255);
            int green = (int)(hslColor.L * 255);
            int red = (int)(hslColor.L * 255);

            double h = hslColor.H;
            double s = hslColor.S;
            double l = hslColor.L;

            if (hslColor.S != 0)
            {
                double hue = h / 360;
                double v1 = (l < 0.5) ? (l * (1 + s)) : ((l + s) - (l * s));
                double v2 = 2 * l - v1;

                red = (int)(255 * HueToRgb(v2, v1, hue + (1/3.0)));
                green = (int)(255 * HueToRgb(v2, v1, hue));
                blue = (int)(255 * HueToRgb(v2, v1, hue - (1/3.0)));
            }

            return new Bgr(blue, green, red);
        }

        // Convert Hue to Rgb
        private static double HueToRgb(double v1, double v2, double hue)
        {
            if (hue < 0)
            {
                hue++;
            }
            else if (hue > 1)
            {
                hue--;
            }

            if ((6 * hue) < 1)
            {
                return (v1 + (v2 - v1) * 6 * hue);
            }

            if ((2 * hue) < 1)
            {
                return v2;
            }

            if ((3 * hue) < 2)
            {
                return (v1 + (v2 - v1) * ((2.0f / 3) - hue) * 6);
            }

            return v1;
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

        // Calculate new lightness value between the min and max interval
        public static Bgr GetEqualizedValue(double min, double max, Bgr color)
        {
            double blue = color.Blue;
            double green = color.Green;
            double red = color.Red;

            double oldValue = GetLightness(color);
            double newValue = (100 / (max - min)) * (oldValue);
            double diff = Math.Abs(oldValue - newValue) / 100;

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
            
            return new Bgr(blue + blue * diff, green + green * diff, red + red * diff);
        }

        // Override ToString method to dispay data as a string
        public override string ToString()
        {
            return $"HSL({H}, {S}, {L})";
        }

        // Increase the given color's Hue with the given value
        public static Bgr IncreaseHue(Bgr color, double degree)
        {
            HSL hslColor = ConvertFromBgr(color);
            hslColor.HueIncreaseBy(degree);
            return ConvertToBgr(hslColor);
        }

        // Increase the given color's Saturation with the given value
        public static Bgr IncreaseSaturatiion(Bgr color, double percentage)
        {
            HSL hslColor = ConvertFromBgr(color);
            hslColor.SaturationIncreaseBy(percentage);
            return ConvertToBgr(hslColor);
        }

        // Increase the given color's Lightness with the given value
        public static Bgr IncreaseLightness(Bgr color, double percentage)
        {
            HSL hslColor = ConvertFromBgr(color);
            hslColor.LightnessIncreaseBy(percentage);
            return ConvertToBgr(hslColor);
        }
    }
}
