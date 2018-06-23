using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
        // Importing C++ function from dll to close any OpenCV (C++) window
        [DllImport(@"D:\ITI_CEI_2017\PhotoCAD\x64\Debug\PhotoCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CloseCPPWindows();

        bool stopped = true;
        public string InputFileText { get; set; }
        public string OutputFolderText { get; set; }
        public int ImageHeight { get; set; }
        public int ImageWidth { get; set; }
        public double ImageRatio { get; set; }
        public double ScaleRatio { get; set; } = 1;


        public MainWindow()
        {
            InitializeComponent();

            // To load the 'Upload Photo' from 'Icons' Folder.
            //ImageIcon.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Icons\\PhotoIcon.png"));
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
                InputFileName_TB.Text = fileDialog.FileName;
                InputFileText = fileDialog.FileName;

                // Pass the Folder to the 'OutputFolder_TB'
                OutputFilePath_TB.Text = System.IO.Path.GetDirectoryName(InputFileText);

                //Load the Image in the Application
                ImageIcon.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(fileDialog.FileName);
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
                    OutputFolderText = dialog.SelectedPath.ToString();
                }
            }
        }

        private void ImageIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Browse_btn_Click(sender, e);
        }

        private void Start_btn_Click(object sender, RoutedEventArgs e)
        {
            if (stopped)
            {
                if (InputFileName_TB.Text == "" && OutputFilePath_TB.Text == "")
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
                if (OutputFilePath_TB.Text == System.IO.Path.GetDirectoryName(InputFileText))
                {
                    MessageBoxResult boxResult = MessageBox.Show("Please select different Output Folder!", "Choosing the same folder conflict",
                       MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (InputFileName_TB.Text != "" && OutputFilePath_TB.Text != "")
                {
                    Start_btn.Content = "■  Stop";
                    stopped = false;
                    // Disable all Browse Butons in the Main Window
                    InputBrowse_btn.IsEnabled = false;
                    OutputBrowse_btn.IsEnabled = false;
                    InputFileName_TB.IsEnabled = false;
                    OutputFilePath_TB.IsEnabled = false;
                    ImageIcon.IsEnabled = false;
                    None_RB.IsEnabled = false;
                    Plan_RB.IsEnabled = false;
                    WhitePaper_RB.IsEnabled = false;

                    CropWindow cropWindow = new CropWindow();
                    cropWindow.MainWindowProperty = this;

                    // Load the Image to the Canvas
                    LoadImageToCanvas(cropWindow, InputFileName_TB.Text);
                    cropWindow.Show();
                }
            }

            else
            {
                //Close All C# Windows except the 'MainWindow'
                //for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 1; intCounter--)
                //    App.Current.Windows[intCounter].Close();
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();

                //Close All C++ Windows
                CloseCPPWindows();

                Start_btn.Content = "Start";
                stopped = true;

                //Enable all Browse Butons in the Main Window
                InputBrowse_btn.IsEnabled = true;
                OutputBrowse_btn.IsEnabled = true;
                InputFileName_TB.IsEnabled = true;
                OutputFilePath_TB.IsEnabled = true;
                ImageIcon.IsEnabled = true;
                None_RB.IsEnabled = true;
                Plan_RB.IsEnabled = true;
                WhitePaper_RB.IsEnabled = true;
            }


        }

        public void LoadImageToCanvas(CropWindow window, string imagePath)
        {
            // Get the Actual Height and Width of the Image
            using (FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                BitmapFrame frame = BitmapFrame.Create(fileStream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                ImageHeight = frame.PixelHeight;
                ImageWidth = frame.PixelWidth;
                ImageRatio = ImageWidth / ImageHeight;
            }

            // To Load the selected image to the Crop Canvas Background
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri(imagePath, UriKind.Relative));
            window.CropCanvas.Background = ib;

            //Get the Image Width , Height and Aspect Ratio
            double imgWidth = ib.ImageSource.Width;
            double imgHeight = ib.ImageSource.Height;
            double imgRatio = imgHeight / imgWidth;


            if (imgWidth > 1250)  //Resize the image if it is too Big
            {
                imgWidth = 1250;
                imgHeight = imgWidth * imgRatio;
            }
            else if (imgWidth < 600)  //Resize the image if it is too Small
            {
                imgWidth = 600;
                imgHeight = imgWidth * imgRatio;
            }

            //To Set the Scale Factor from Actual Image Size and the Canvas Size
            ScaleRatio = ImageWidth / imgWidth;
            window.FileName = System.IO.Path.GetFileNameWithoutExtension(imagePath);

            //To Set the Canvas Size relative to the image
            window.CropCanvas.Width = imgWidth;
            window.CropCanvas.Height = imgHeight;

            //To Set the Canvas Size relative to the Canvas/Image
            window.Width = imgWidth;
            window.Height = (window.CropCanvas.Height);
        }
    }
}
