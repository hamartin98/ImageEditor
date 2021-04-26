using Emgu.CV.Structure;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using ImageEditor.OptionFrames;
using System.Windows.Controls;

namespace ImageEditor
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        private ImageData imageData = null;
        private int saveQuality = 100;
        private readonly string fileDialogFilter = "Image files|*.jpg;*.jpeg;*.png;*.bmp;|" +
                                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                                "Portable Network Graphic (*.png)|*.png|" +
                                "Bmp (*.bmp)|*.bmp";

        // store the slider to remove when a new one is added
        private UserControl currentSlider = null;

        public EditorWindow()
        {
            InitializeComponent();
            imageData = new ImageData();
        }

        // Open an image from the computer and set the data of the imageData property
        private void OpenImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Open image";
            openFileDialog.Filter = fileDialogFilter;

            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                imageData.SetData(fileName);

                UpdateImageContainer();
            }
        }

        // Save the image to the computer
        private void SaveImage()
        {
            if (imageData.IsDataSet)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = imageData.Format;
                saveFileDialog.AddExtension = true;

                saveFileDialog.Title = "Save image";
                saveFileDialog.Filter = fileDialogFilter;

                if (saveFileDialog.ShowDialog() == true)
                {
                    string destPath = saveFileDialog.FileName;
                    imageData.Save(destPath, saveQuality);
                }
            }
        }

        // Show information about the image on the UI
        private void DisplayInfo()
        {
            tbPath.Text = imageData.Path;
            tbResolution.Text = imageData.ResolutionStr;
        }

        // Sets the image on the UI
        private void UpdateImageContainer()
        {
            if (imageData.IsDataSet)
            {
                imgMain.Source = imageData.ToBitmapImage();
                DisplayInfo();
            }
        }

        private void sldQuality_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            saveQuality = (int)sldQuality.Value;
        }

        private void sldBlur_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (imageData.IsDataSet)
            {
                int radius = (int)sldBlur.Value;
                imageData.BlurEffect(radius);
                UpdateImageContainer();
            }
        }

        // Occurs when files are dropped on the window, only the first file and only image files are accepted
        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string fileName = files[0];

                bool match = Regex.IsMatch(fileName, @".*\.(jpe?g|png|bmp)");
                if(match)
                {
                    imageData.SetData(fileName);
                    UpdateImageContainer();
                }
                else
                {
                    MessageBox.Show("Wrong file format!");
                }
            }
        }

        private void sldHue_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (imageData.IsDataSet)
            {
                double value = sldHue.Value;
                imageData.IncreaseHue(value);
                UpdateImageContainer();
            }
        }

        private void sldSaturation_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (imageData.IsDataSet)
            {
                double value = sldSaturation.Value / 100.0;
                imageData.IncreaseSaturation(value);
                UpdateImageContainer();
            }
        }

        private void sldLightness_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (imageData.IsDataSet)
            {
                double value = sldLightness.Value / 100.0;
                imageData.IncreaseLightness(value);
                UpdateImageContainer();
            }
        }

        private void btnDetectEdge_Click(object sender, RoutedEventArgs e)
        {
            if (imageData.IsDataSet)
            {
                bool isColored = (bool)cbEdgeColored.IsChecked;
                Bgr color = GetBgrColor();
                imageData.EdgeDetection(isColored, color);
                UpdateImageContainer();
            }
        }

        // Returns the selected color from the colorPicker
        private Color GetColor()
        {
            return (Color)colorPicker.SelectedColor;
        }

        // Convert the Color type color to a Bgr type color
        private Bgr GetBgrColor()
        {
            Color color = GetColor();
            return new Bgr(color.B, color.G, color.R);
        }

        private void menuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenImage();
        }

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveImage();
        }

        private void menuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            LoadOptionsFrame("SaveOptions");
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void menuRedo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuRedo_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void menuUndo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuCopy_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuPaste_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuShrink_Click(object sender, RoutedEventArgs e)
        {
            if (imageData.IsDataSet)
            {
                imageData.ShrinkHistogram();
                UpdateImageContainer();
            }
        }

        private void menuStretch_Click(object sender, RoutedEventArgs e)
        {
            if (imageData.IsDataSet)
            {
                imageData.StretchHistogram();
                UpdateImageContainer();
            }
        }

        private void menuEdgeDetect_Click(object sender, RoutedEventArgs e)
        {
            if (imageData.IsDataSet)
            {
                bool isColored = (bool)cbEdgeColored.IsChecked;
                Bgr color = GetBgrColor();
                imageData.EdgeDetection(true, color);
                UpdateImageContainer();
            }
        }

        // Load an user control into the optionsFrame to show different settings
        // Search for items in the OptionFrames folder
        private void LoadOptionsFrame(string fileName)
        {
            optionsFrame.Source = new System.Uri($"OptionFrames/{fileName}.xaml", System.UriKind.RelativeOrAbsolute);
        }

        private void btnmagni_Click(object sender, RoutedEventArgs e)
        {
            //myMagnifier.ZoomFactor = 0.05;
        }

        private void imgMain_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            magnifier.ZoomFactor = 0.5;
        }

        private void imgMain_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            magnifier.ZoomFactor = 0;
        }

        // Load the currently used user control into the grid
        private void SetOptionsControl(UserControl control)
        {
            Grid.SetRow(control, 1);
            if(currentSlider != null)
            {
                // Remove the previous slider
                settingsGrid.Children.Remove(currentSlider);
            }
            settingsGrid.Children.Add(control);
            currentSlider = control;
        }

        private void menuBlur_Click(object sender, RoutedEventArgs e)
        {
            SliderControl sliderControl = new SliderControl("Blur options", 0, 100, 10, 0, 10);
            SetOptionsControl(sliderControl);

            sliderControl.ValueChanged += (double delta) =>
            {
                imageData.BlurEffect((int)delta);
                UpdateImageContainer();
            };
        }

        private void menuHue_Click(object sender, RoutedEventArgs e)
        {
            SliderControl sliderControl = new SliderControl("Hue options", -360, 360, 10, 0, 10);
            SetOptionsControl(sliderControl);

            sliderControl.ValueChanged += (double delta) =>
            {
                imageData.IncreaseHue(delta);
                UpdateImageContainer();
            };
        }

        private void menuSaturation_Click(object sender, RoutedEventArgs e)
        {
            SliderControl sliderControl = new SliderControl("Saturation options", -100, 100, 10, 0, 10);
            SetOptionsControl(sliderControl);

            sliderControl.ValueChanged += (double delta) =>
            {
                imageData.IncreaseSaturation(delta);
                UpdateImageContainer();
            };
        }

        private void menulightness_Click(object sender, RoutedEventArgs e)
        {
            SliderControl sliderControl = new SliderControl("Lightness options", -100, 100, 10, 0, 10);
            SetOptionsControl(sliderControl);

            sliderControl.ValueChanged += (double delta) =>
            {
                imageData.IncreaseLightness(delta);
                UpdateImageContainer();
            };
        }
    }
}
