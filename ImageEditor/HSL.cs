using System;
using System.Collections.Generic;
using System.Text;

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
            double blueH = blue / 255.0f;
            double greenH = green / 255.0f;
            double redH = red / 255.0f;

            double cMin = blueH;
            double cMax = blueH;

            if (greenH < cMin)
            {
                cMin = greenH;
            }
            else if (greenH > cMax)
            {
                cMax = greenH;
            }

            if (redH < cMin)
            {
                cMin = redH;
            }
            else if (redH > cMax)
            {
                cMax = redH;
            }

            double delta = cMax - cMin;
            double H = 0;
            double S = 0;
            double L = (cMax - cMin) / 2;

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

        // Override ToString method to dispay data as a string
        public override string ToString()
        {
            return $"HSL({H}, {S}, {L})";
        }
    }
}
