using Emgu.CV.Structure;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Windows;

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

        public EditorWindow()
        {
            InitializeComponent();
            imageData = new ImageData();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenImage();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveImage();
        }

        private void btnAutoContrast_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnShrink_Click(object sender, RoutedEventArgs e)
        {
            imageData.ShrinkHistogram();
        }

        private void btnStretch_Click(object sender, RoutedEventArgs e)
        {
            imageData.StretchHistogram();
        }

        // Button to call methods
        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            //imageData.EdgeDetection(false);
            UpdateImageContainer();
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

        // Show information about the image on the UI
        private void DisplayInfo()
        {
            tbPath.Text = imageData.Path;
            tbResolution.Text = imageData.ResolutionStr;
        }

        // Sets the image on the UI
        private void UpdateImageContainer()
        {
            imgMain.Source = imageData.ToBitmapImage();
            DisplayInfo();   
        }

        private void sldQuality_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            saveQuality = (int)sldQuality.Value;
        }

        private void sldBlur_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            int radius = (int)sldBlur.Value;
            imageData.BlurEffect(radius);
            UpdateImageContainer();
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
            double value = sldHue.Value;
            imageData.IncreaseHue(value);
            UpdateImageContainer();
        }

        private void sldSaturation_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            double value = sldSaturation.Value / 100.0;
            imageData.IncreaseSaturation(value);
            UpdateImageContainer();
        }

        private void sldLightness_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            double value = sldLightness.Value / 100.0;
            imageData.IncreaseLightness(value);
            UpdateImageContainer();
        }

        private void btnDetectEdge_Click(object sender, RoutedEventArgs e)
        {
            bool isColored = (bool)cbEdgeColored.IsChecked;
            Bgr color = new Bgr(255, 0, 255);
            imageData.EdgeDetection(isColored, color);
            UpdateImageContainer();
        }
    }
}
