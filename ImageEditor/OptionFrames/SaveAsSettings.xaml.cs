using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace ImageEditor.OptionFrames
{
    /// <summary>
    /// Interaction logic for SaveAs.xaml
    /// </summary>
    public partial class SaveAsSettings : UserControl
    {
        public delegate void ButtonClickedEvent(string fullPath, string format, int saveQuality);
        public event ButtonClickedEvent SaveButtonClicked;

        private string path = "";

        public SaveAsSettings()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string fileName = tbFileName.Text;
            string format = cboxFormat.SelectedValue.ToString();
            int saveQuality = (int)sldQuality.Value;

            if(fileName == null)
            {
                fileName = "image";
            }
            string fullPath = $"{path}\\{fileName}.{format}";

            if(SaveButtonClicked != null)
            {
                SaveButtonClicked(fullPath, format, saveQuality);
            }
        }

        private void btnSelectPath_Click(object sender, RoutedEventArgs e)
        {
            using(CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog())
            {
                openFileDialog.IsFolderPicker = true;

                if(openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    path = openFileDialog.FileName;
                    tbPath.Text = path;
                }
            }
        }
    }
}
