using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhotoCAD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // To load the 'Upload Photo' from 'Icons' Folder.
            ImageIcon.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Icons\\PhotoIcon.png"));
        }

        #region Menu Items
        private void Menu_About_Click(object sender, RoutedEventArgs e)
        {
            About aboutWindow = new About();
            aboutWindow.Show();
        }
        private void Menu_Exit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to close PhotoCAD?",
                                          "Confirmation",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
        #endregion
        private void Browse_btn_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            fileDialog.DefaultExt = ".jpg";
            fileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = fileDialog.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true && result != null)
            {
                // Open document
                string filename = fileDialog.FileName;
                InputFileName_TB.Text = filename;

                //Load the Image in the Application
                ImageIcon.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(filename);
            }

        }

        private void BrowseFolder_btn_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (dialog.SelectedPath != "")
                {
                    // Get the selected file name and display in a TextBox
                    OutputFilePath_TB.Text = dialog.SelectedPath.ToString();
                }
            }
        }

        private void ImageIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Browse_btn_Click(sender, e);
        }

        private void Start_btn_Click(object sender, RoutedEventArgs e)
        {
            if (InputFileName_TB.Text != "" && OutputFilePath_TB.Text != "")
            {
                //Disable all Browse Butons in the Main Window
                InputBrowse_btn.IsEnabled = false;
                OutputBrowse_btn.IsEnabled = false;
                InputFileName_TB.IsEnabled = false;
                OutputFilePath_TB.IsEnabled = false;
                ImageIcon.IsEnabled = false;

                CropWindow cropWindow = new CropWindow();
                cropWindow.Show();

                // To Load the selected image to the Crop Canvas Background
                ImageBrush ib = new ImageBrush();
                ib.ImageSource = new BitmapImage(new Uri(InputFileName_TB.Text, UriKind.Relative));
                cropWindow.CropCanvas.Background = ib;
                
                //Get the Image Width , Height and Aspect Ratio
                double imageWidth = ib.ImageSource.Width;
                double imageHeight = ib.ImageSource.Height;
                double imageRatio = imageWidth / imageHeight;

                if (imageWidth > 1200)  //Resize the image if it is too Big
                {
                    imageWidth = 1200;
                }
                if (imageWidth < 800)  //Resize the image if it is too Small
                {
                    imageWidth = 800;
                }
                cropWindow.CropCanvas.Width = imageWidth;
                cropWindow.CropCanvas.Height = imageWidth / imageRatio;
                //cropWindow.Width = imageWidth + 200;
                //cropWindow.Height = (cropWindow.Width) / imageRatio;
            }
            else if (InputFileName_TB.Text == "" && OutputFilePath_TB.Text == "")
            {
                MessageBoxResult boxResult = MessageBox.Show("Please select the Photo and the Output Folder!", "Missing Fields",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (InputFileName_TB.Text == "")
            {
                MessageBoxResult boxResult = MessageBox.Show("Please select the Photo!", "Missing Field",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (OutputFilePath_TB.Text == "")
            {
                MessageBoxResult boxResult = MessageBox.Show("Please select the Output Folder!", "Missing Field",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Stop_btn_Click(object sender, RoutedEventArgs e)
        {
            //Enable all Browse Butons in the Main Window
            InputBrowse_btn.IsEnabled = true;
            OutputBrowse_btn.IsEnabled = true;
            InputFileName_TB.IsEnabled = true;
            OutputFilePath_TB.IsEnabled = true;
            ImageIcon.IsEnabled = true;

            //Just Trying
            
        }
    }
}
