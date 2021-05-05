using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageEditor.OptionFrames
{
    /// <summary>
    /// Interaction logic for SplashEffectSettings.xaml
    /// </summary>
    public partial class SplashEffectSettings : UserControl
    {
        public delegate void ButtonClickedEvent(int treshold);
        public event ButtonClickedEvent SplashButtonClicked;
        public event ButtonClickedEvent ReverseSplashButtonClicked;

        public SplashEffectSettings()
        {
            InitializeComponent();
        }

        private void btnSplash_Click(object sender, RoutedEventArgs e)
        {
            int treshold = (int)sldTreshold.Value;
            NotifySplash(treshold);
        }

        private void btnReverseSplash_Click(object sender, RoutedEventArgs e)
        {
            int treshold = (int)sldTreshold.Value;
            NotifyReverseSplash(treshold);
        }

        private void NotifySplash(int treshold)
        {
            if(SplashButtonClicked != null)
            {
                SplashButtonClicked(treshold);
            }
        }

        private void NotifyReverseSplash(int treshold)
        {
            if (ReverseSplashButtonClicked != null)
            {
                ReverseSplashButtonClicked(treshold);
            }
        }
    }
}
