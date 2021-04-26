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
    /// Interaction logic for EdgeDetectionSettings.xaml
    /// </summary>
    public partial class EdgeDetectionSettings : UserControl
    {
        public delegate void DetectButtonClickEvent(string method, bool isColored);
        public event DetectButtonClickEvent DetectButtonClicked;


        public EdgeDetectionSettings()
        {
            InitializeComponent();
        }

        private void btnDetect_Click(object sender, RoutedEventArgs e)
        {
            string method = cboxMethod.ToString();
            bool isColored = (bool)cbIsColored.IsChecked;

            DetectButtonClicked(method, isColored);
        }
    }
}
