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
    /// Interaction logic for MagnifierOptions.xaml
    /// </summary>
    public partial class MagnifierOptions : UserControl
    {
        private Size basicSize; // basic size to be multiplied, used with rectangle magnifier
        private int basicRadius; // basic radius to be multiplied, used with circle magnifier
        private int multiplier = 1;

        // Returns the selected type as a string
        public string SelectedType { get; private set; }

        // Returns the new calculated size based on the multiplier
        public Size CurrentSize => new Size(basicSize.Width * multiplier, basicSize.Height * multiplier);
        
        // Returns the new calculated radius based on the multiplier
        public int CurrentRadius => basicRadius * multiplier;

        public delegate void SettingsChangedEvent();
        public event SettingsChangedEvent SettingsChanged;

        public MagnifierOptions()
        {
            InitializeComponent();

            basicSize = new Size(80, 50);
            basicRadius = 50;
        }

        private void Notify()
        {
            if (SettingsChanged != null)
            {
                SettingsChanged();
            }
        }

        private void cboxMagnifierType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedType = cboxMagnifierType.SelectedValue.ToString();
            Notify();
        }

        private void sldSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            multiplier = (int)sldSize.Value;
            Notify();
        }
    }
}
