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
    /// Interaction logic for SliderControl.xaml
    /// </summary>
    public partial class SliderControl : UserControl
    {
        public delegate void ValueChangedEvent(double value);
        public event ValueChangedEvent ValueChanged;

        // Store the previos value of the slider to calculate delta from the difference
        private double prevValue;

        public SliderControl(string title, double minValue, double maxValue, double step, double startValue, double tickFrequency)
        {
            InitializeComponent();
            SetData(title, minValue, maxValue, step, startValue, tickFrequency);
        }

        // Set all data of the current control
        public void SetData(string title, double minValue, double maxValue, double step, double startValue, double tickFrequency)
        {
            tbTitle.Text = title;

            sldValue.Minimum = minValue;
            sldValue.Maximum = maxValue;
            sldValue.SmallChange = step;
            sldValue.Value = startValue;
            sldValue.TickFrequency = step;

            prevValue = startValue;
        }

        private void sldValue_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            // Delta is the difference between the previous and current value
            // If the slider is dragged, we pass the delta value to the delegate

            double delta = sldValue.Value - prevValue;
            prevValue = sldValue.Value;

            if(ValueChanged != null)
            {
                ValueChanged(delta);
            }
        }
    }
}
