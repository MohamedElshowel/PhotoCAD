using Design.Presentation.Geometry;
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
using System.Windows.Shapes;

namespace PhotoCAD
{
    /// <summary>
    /// Interaction logic for CropWindow.xaml
    /// </summary>
    public partial class CropWindow : Window
    {
        public MainWindow MainWindowProperty { get; set; }

        // Importing DLL functions
        //[DllImport(@"D:\ITI_CEI_2017\Graduation Project\Desktop Version - WPF\PhotoCAD\PhotoCAD\bin\x64\Debug\PhotoCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        [DllImport("PhotoCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CloseCPPWindows();
        [DllImport("PhotoCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BordersDetection(string photoPath, int detectionType);
        [DllImport("PhotoCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReleaseMemory(IntPtr ptr);
        [DllImport("PhotoCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Return20();
        [DllImport("PhotoCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WarpImage(string photoPath, double[] cornerPoints, char otherTransformation, string fileName, string folderPath);
        [DllImport("PhotoCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EdgeDetection(string warpedImage, string folderPath, string fileName, int levelOfDetails);
        [DllImport("PhotoCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CreateDXF(string warpedImage, string folderPath, string fileName, int detailsLevel);


        //public static extern int MainFunction(string photoFullPath, string folderPath, string fileName, double[] cornerPoints, char otherTransformation, bool isWarped);

        // Member Variables
        public DependencyObject SelectedElement { get; set; }
        public string FileName { get; set; }
        Point currentPoint = new Point();
        int pointsNo = 0;
        double[] pointsLocationX = new double[4];
        double[] pointsLocationY = new double[4];
        double[] pointsLocation = new double[8];    // C# Array of Points
        double[] pointsArray = new double[8];       // C++ Array of Points

        Ellipse[] cropPoints = new Ellipse[4];
        int detectionType;

        public CropWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindowProperty.None_RB.IsChecked == true)
            {
                detectionType = 0;
            }
            else if (MainWindowProperty.Plan_RB.IsChecked == true)
            {
                detectionType = 1;
            }
            else if (MainWindowProperty.WhitePaper_RB.IsChecked == true)
            {
                detectionType = 2;
            }

            // Initializing DLL Function 'BordersDetection'
            IntPtr ptr = BordersDetection(MainWindowProperty.InputFileText, detectionType);
            Marshal.Copy(ptr, pointsArray, 0, 8);

            // To Realse the Memory of the Pointer
            ReleaseMemory(ptr);

            // Get FileName
            FileName = System.IO.Path.GetFileNameWithoutExtension(MainWindowProperty.InputFileName_TB.Text);

            // Assigning Points Position
            pointsLocation[0] = pointsLocationY[0] = pointsArray[0] / MainWindowProperty.ScaleRatio;
            pointsLocation[1] = pointsLocationX[0] = pointsArray[1] / MainWindowProperty.ScaleRatio;
            pointsLocation[2] = pointsLocationY[1] = pointsArray[2] / MainWindowProperty.ScaleRatio;
            pointsLocation[3] = pointsLocationX[1] = pointsArray[3] / MainWindowProperty.ScaleRatio;
            pointsLocation[4] = pointsLocationY[2] = pointsArray[4] / MainWindowProperty.ScaleRatio;
            pointsLocation[5] = pointsLocationX[2] = pointsArray[5] / MainWindowProperty.ScaleRatio;
            pointsLocation[6] = pointsLocationY[3] = pointsArray[6] / MainWindowProperty.ScaleRatio;
            pointsLocation[7] = pointsLocationX[3] = pointsArray[7] / MainWindowProperty.ScaleRatio;

            // Adding Control Points to the Loaded Image
            for (int i = 0; i < 4; i++)
            {
                Ellipse cropCircle = new Ellipse
                {
                    Fill = Brushes.Orange,
                    StrokeThickness = 50,
                    Width = 10,
                    Height = 10,
                };
                cropCircle.RenderTransform =
                   new TranslateTransform(pointsLocationX[i] - cropCircle.Width / 2, pointsLocationY[i] - cropCircle.Width / 2);
                cropPoints[i] = cropCircle;
                CropCanvas.Children.Add(cropPoints[i]);
                pointsNo = 4;
                DrawLines();
            }

            char otherTransformation = 'n';

            // Call the Warping Function from the dll
            WarpImage(MainWindowProperty.InputFileText, pointsArray, otherTransformation, FileName, MainWindowProperty.OutputFolderText);

            //MainFunction(MainWindowProperty.InputFileName_TB.Text, MainWindowProperty.OutputFilePath_TB.Text, FileName, pointsArray, otherTransformation, false);
        }


        private void CropCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                currentPoint = e.GetPosition(CropCanvas);
            }

            if (pointsNo < 4)
            {
                Ellipse cropCircles = new Ellipse
                {
                    Fill = Brushes.Orange,
                    StrokeThickness = 50,
                    Width = 10,
                    Height = 10,
                };
                cropCircles.RenderTransform =
                    new TranslateTransform(currentPoint.X - cropCircles.Width / 2, currentPoint.Y - cropCircles.Height / 2);
                pointsLocationX[pointsNo] = currentPoint.X;
                pointsLocationY[pointsNo] = currentPoint.Y;
                cropPoints[pointsNo] = cropCircles;
                CropCanvas.Children.Add(cropCircles);

                if (0 < pointsNo)
                {
                    Line cropLine = new Line
                    {
                        Stroke = Brushes.DarkSlateGray,
                        StrokeThickness = 1,
                        X1 = pointsLocationX[pointsNo - 1],
                        X2 = pointsLocationX[pointsNo],
                        Y1 = pointsLocationY[pointsNo - 1],
                        Y2 = pointsLocationY[pointsNo],
                    };
                    CropCanvas.Children.Add(cropLine);
                }

                if (pointsNo == 3)
                {
                    Line cropLine = new Line
                    {
                        Stroke = Brushes.DarkSlateGray,
                        StrokeThickness = 1,
                        X1 = pointsLocationX[pointsNo],
                        X2 = pointsLocationX[0],
                        Y1 = pointsLocationY[pointsNo],
                        Y2 = pointsLocationY[0],
                    };
                    CropCanvas.Children.Add(cropLine);
                }
                pointsNo++;
            }

            var canvas = sender as Canvas;
            if (canvas == null) return;

            HitTestResult hitTestResult = VisualTreeHelper.HitTest(canvas, e.GetPosition(canvas));
            if (hitTestResult.VisualHit is Ellipse)
            {
                SelectedElement = hitTestResult.VisualHit;
            }
        }

        private void CropCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (SelectedElement != null)
            {
                var mousePosition = e.GetPosition(CropCanvas);
                ((Ellipse)SelectedElement).RenderTransform = new TranslateTransform(mousePosition.X, mousePosition.Y);
            }
        }

        private void CropCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SelectedElement = null;
        }

        // Draw lines connecting 
        private void DrawLines()
        {
            if (0 < pointsNo)
            {
                for (int i = 0; i < pointsNo; i++)
                {
                    if (i < 3)
                    {
                        Line cropLine = new Line();
                        cropLine.Stroke = Brushes.DarkSlateGray;
                        cropLine.StrokeThickness = 1;
                        cropLine.X1 = pointsLocationX[i];
                        cropLine.Y1 = pointsLocationY[i];
                        cropLine.X2 = pointsLocationX[i + 1];
                        cropLine.Y2 = pointsLocationY[i + 1];
                        CropCanvas.Children.Add(cropLine);
                    }
                    else if (i == 3)
                    {
                        Line cropLine = new Line
                        {
                            Stroke = Brushes.DarkSlateGray,
                            StrokeThickness = 1,
                            X1 = pointsLocationX[i],
                            X2 = pointsLocationX[0],
                            Y1 = pointsLocationY[i],
                            Y2 = pointsLocationY[0],
                        };
                        CropCanvas.Children.Add(cropLine);
                    }
                }
            }
        }

        private void Clear_BTN_Click(object sender, RoutedEventArgs e)
        {
            char otherTransformation = 'c';
            TransferePointsFromCanvasToCPP();
            WarpImage(MainWindowProperty.InputFileText, pointsArray, otherTransformation, FileName, MainWindowProperty.OutputFolderText);

            // Clear All Array Elements
            Array.Clear(cropPoints, 0, 4);

            // Remove all circles from the screen
            CropCanvas.Children.Clear();
            pointsNo = 0;
        }

        private void Rotate_BTN_Click(object sender, RoutedEventArgs e)
        {
            char otherTransformation = 'r';
            CloseCPPWindows();
            TransferePointsFromCanvasToCPP();
            WarpImage(MainWindowProperty.InputFileText, pointsArray, otherTransformation, FileName, MainWindowProperty.OutputFolderText);
        }

        private void Mirror_BTN_Click(object sender, RoutedEventArgs e)
        {
            char otherTransformation = 'm';
            CloseCPPWindows();
            TransferePointsFromCanvasToCPP();
            WarpImage(MainWindowProperty.InputFileText, pointsArray, otherTransformation, FileName, MainWindowProperty.OutputFolderText);

        }

        private void FullSize_BTN_Click(object sender, RoutedEventArgs e)
        {
            char otherTransformation = 'f';
            CloseCPPWindows();
            TransferePointsFromCanvasToCPP();
            WarpImage(MainWindowProperty.InputFileText, pointsArray, otherTransformation, FileName, MainWindowProperty.OutputFolderText);
        }

        private void SaveImage_BTN_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ToEdgeDetection_BTN_Click(object sender, RoutedEventArgs e)
        {
            CloseCPPWindows();                        //To Close the C++ Window
            ImageWarping_tab.IsEnabled = false;
            EdgeDetection_tab.IsEnabled = true;
            EdgeDetection_tab.IsSelected = true;    //To Activate the Edge Detection Tab

            // Load the Edges of the image to the canvas
            string warpedImage = MainWindowProperty.OutputFolderText + "/" + FileName + "_warped.jpg";
            EdgeDetection(warpedImage, MainWindowProperty.OutputFolderText, FileName, 50);
            string imageEdges = MainWindowProperty.OutputFolderText + "/" + FileName + "_edges.jpg";
            LoadImageToCanvas(EdgeDetectionCanvas, imageEdges);
        }

        private void SliderValue_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            string imageEdges = MainWindowProperty.OutputFolderText + "/" + FileName + "_edges.jpg";
            EdgeDetection(imageEdges, MainWindowProperty.OutputFolderText, FileName, (int)SliderValue.Value);
            LoadImageToCanvas(EdgeDetectionCanvas, imageEdges);
        }

        private void ToDXF_btn_Click(object sender, RoutedEventArgs e)
        {
            EdgeDetection_tab.IsEnabled = false;
            DXF_tab.IsEnabled = true;
            DXF_tab.IsSelected = true;    //To Activate the DXF Tab

            // Load the Edges of the image to the canvas
            string imageEdges = MainWindowProperty.OutputFolderText + "/" + FileName + "_edges.jpg";
            //

            // Function To Read the Edges and convert it to vector<Points> then DXF 
            string warpedImage = MainWindowProperty.OutputFolderText + "/" + FileName + "_warped.jpg";
            CreateDXF(warpedImage, MainWindowProperty.OutputFolderText, FileName, 50);

            //LoadImageToCanvas(EdgeDetectionCanvas, imageEdges);
        }

        private void LoadImageToCanvas(Canvas canvas, string newImagePath)
        {
            // Get the Actual Height and Width of the Image
            using (FileStream fileStream = new FileStream(newImagePath, FileMode.Open, FileAccess.Read))
            {
                BitmapFrame frame = BitmapFrame.Create(fileStream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                MainWindowProperty.ImageHeight = frame.PixelHeight;
                MainWindowProperty.ImageWidth = frame.PixelWidth;
                MainWindowProperty.ImageRatio = MainWindowProperty.ImageWidth / MainWindowProperty.ImageHeight;
            }

            // To Load the selected image to the Crop Canvas Background
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri(newImagePath, UriKind.Relative));
            canvas.Background = ib;

            //Get the Image Width , Height and Aspect Ratio
            double imgWidth = ib.ImageSource.Width;
            double imgHeight = ib.ImageSource.Height;
            double imgRatio = imgHeight / imgWidth;


            if (imgWidth > 1200)  //Resize the image if it is too Big
            {
                imgWidth = 1200;
                imgHeight = imgWidth * imgRatio;
            }
            else if (imgWidth < 600)  //Resize the image if it is too Small
            {
                imgWidth = 600;
                imgHeight = imgWidth * imgRatio;
            }

            //To Set the Scale Factor from Actual Image Size and the Canvas Size
            MainWindowProperty.ScaleRatio = MainWindowProperty.ImageWidth / imgWidth;

            //To Set the Canvas Size relative to the image
            canvas.Width = imgWidth;
            canvas.Height = imgHeight;

            //To Set the Canvas Size relative to the Canvas/Image
            Width = imgWidth;
            Height = (canvas.Height) + 35;
        }

        private void TransferePointsFromCanvasToCPP()
        {
            for (int i = 0; i < 8; i++)
            {
                pointsArray[i] = pointsLocation[i] * MainWindowProperty.ScaleRatio;
            }
        }

        private void Finish_btn_Click(object sender, RoutedEventArgs e)
        {
            // Restart the Application
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void OpenFile_btn_Click(object sender, RoutedEventArgs e)
        {
            //Open DXF File
            string fileDXF = MainWindowProperty.OutputFolderText + "/" + FileName + ".dxf";
            System.Diagnostics.Process.Start(fileDXF);

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

        private void Exit_btn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
