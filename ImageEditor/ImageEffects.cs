using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ImageEditor
{
    class ImageEffects
    {
        public static void BlurEffect(Mat original, int radius)
        {
            CvInvoke.GaussianBlur(original, original, new Size(-1, -1), radius);
        }
    }
}
