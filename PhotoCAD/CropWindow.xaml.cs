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
        public static extern IntPtr BordersDetection(string photoPath, int detectionType);
        [DllImport(@"D:\ITI_CEI_2017\PhotoCAD\x64\Debug\PhotoCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReleaseMemory(IntPtr ptr);
        [DllImport(@"D:\ITI_CEI_2017\PhotoCAD\x64\Debug\PhotoCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Return20();

        // Member Variables
        public DependencyObject SelectedElement { get; set; }
        Point currentPoint = new Point();
        int pointsNo = 0;
        double[] pointsLocationX = new double[4];
        double[] pointsLocationY = new double[4];


        public CropWindow()
        {
            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Initializing DLL Function 'BordersDetection'
            string f = MainWindowProperty.InputFileText;
            IntPtr ptr = BordersDetection(MainWindowProperty.InputFileText, 1);
            int arrayLength = Marshal.ReadInt32(ptr);
            ptr = IntPtr.Add(ptr, 8);
            double[] pointsArray = new double[arrayLength];
            Marshal.Copy(ptr, pointsArray, 0, arrayLength);

            // Adding Control Points to the Loaded Image
            for (int i = 0; i < 7; i++)
            {
                Ellipse cropCircles = new Ellipse
                {
                    Fill = Brushes.Orange,
                    StrokeThickness = 50,
                    Width = 10,
                    Height = 10,
                };
                cropCircles.RenderTransform = new TranslateTransform(currentPoint.X - cropCircles.Width / 2, currentPoint.Y - cropCircles.Height / 2);
                pointsLocationX[pointsNo] = pointsArray[i];
                i++;
                pointsLocationY[pointsNo] = pointsArray[i];
                CropCanvas.Children.Add(cropCircles);
                pointsNo++;
            }
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
                cropCircles.RenderTransform = new TranslateTransform(currentPoint.X - cropCircles.Width / 2, currentPoint.Y - cropCircles.Height / 2);
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
    }
}
