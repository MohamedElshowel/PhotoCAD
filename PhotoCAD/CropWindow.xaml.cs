using Design.Presentation.Geometry;
using System;
using System.Collections.Generic;
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
        [DllImport(@"D:\ITI_CEI_2017\PhotoCAD\x64\Debug\PhotoCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CloseCPPWindows();
        [DllImport(@"D:\ITI_CEI_2017\PhotoCAD\x64\Debug\PhotoCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BordersDetection(string photoPath, int detectionType);
        [DllImport(@"D:\ITI_CEI_2017\PhotoCAD\x64\Debug\PhotoCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReleaseMemory(IntPtr ptr);
        [DllImport(@"D:\ITI_CEI_2017\PhotoCAD\x64\Debug\PhotoCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Return20();
        [DllImport(@"D:\ITI_CEI_2017\PhotoCAD\x64\Debug\PhotoCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int MainFunction(string photoFullPath, string folderPath, double[] cornerPoints, char otherTransformation, bool isWarped);

        // Member Variables
        public DependencyObject SelectedElement { get; set; }
        Point currentPoint = new Point();
        int pointsNo = 0;
        double[] pointsLocationX = new double[4];
        double[] pointsLocationY = new double[4];
        double[] pointsLocation = new double[8];

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

            double[] pointsArray = new double[8];
            Marshal.Copy(ptr, pointsArray, 0, 8);

            // To Realse the Memory of the Pointer
            ReleaseMemory(ptr);

            // Assigning Points Position
            pointsLocation[0] = pointsLocationY[0] = pointsArray[0] * (1 / MainWindowProperty.ScaleRatio);
            pointsLocation[1] = pointsLocationX[0] = pointsArray[1] * (1 / MainWindowProperty.ScaleRatio);
            pointsLocation[2] = pointsLocationY[1] = pointsArray[2] * (1 / MainWindowProperty.ScaleRatio);
            pointsLocation[3] = pointsLocationX[1] = pointsArray[3] * (1 / MainWindowProperty.ScaleRatio);
            pointsLocation[4] = pointsLocationY[2] = pointsArray[4] * (1 / MainWindowProperty.ScaleRatio);
            pointsLocation[5] = pointsLocationX[2] = pointsArray[5] * (1 / MainWindowProperty.ScaleRatio);
            pointsLocation[6] = pointsLocationY[3] = pointsArray[6] * (1 / MainWindowProperty.ScaleRatio);
            pointsLocation[7] = pointsLocationX[3] = pointsArray[7] * (1 / MainWindowProperty.ScaleRatio);

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
                pointsNo++;
            }

            char otherTransformation = 'n';
            // Call the Warping Function from the dll
            MainFunction(MainWindowProperty.InputFileName_TB.Text, MainWindowProperty.OutputFilePath_TB.Text, pointsArray, otherTransformation, false);
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
        }

        private void AddPoints_BTN_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Clear_BTN_Click(object sender, RoutedEventArgs e)
        {
            char otherTransformation = 'c';
            MainFunction(MainWindowProperty.InputFileName_TB.Text, MainWindowProperty.OutputFilePath_TB.Text, pointsLocation, otherTransformation, false);
            // Clear All Array Elements
            Array.Clear(cropPoints, 0, 4);
        }

        private void Rotate_BTN_Click(object sender, RoutedEventArgs e)
        {
            char otherTransformation = 'r';
            MainFunction(MainWindowProperty.InputFileName_TB.Text, MainWindowProperty.OutputFilePath_TB.Text, pointsLocation, otherTransformation, false);
        }

        private void Mirror_BTN_Click(object sender, RoutedEventArgs e)
        {
            char otherTransformation = 'm';
            MainFunction(MainWindowProperty.InputFileName_TB.Text, MainWindowProperty.OutputFilePath_TB.Text, pointsLocation, otherTransformation, false);
        }

        private void FullSize_BTN_Click(object sender, RoutedEventArgs e)
        {
            char otherTransformation = 'f';
            MainFunction(MainWindowProperty.InputFileName_TB.Text, MainWindowProperty.OutputFilePath_TB.Text, pointsLocation, otherTransformation, false);
        }
    }
}
