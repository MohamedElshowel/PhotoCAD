﻿using Design.Presentation.Geometry;
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
        [DllImport(@"D:\ITI_CEI_2017\PhotoCAD\x64\Debug\PhotoCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CloseCPPWindows();
        [DllImport(@"D:\ITI_CEI_2017\PhotoCAD\x64\Debug\PhotoCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BordersDetection(string photoPath, int detectionType);
        [DllImport(@"D:\ITI_CEI_2017\PhotoCAD\x64\Debug\PhotoCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReleaseMemory(IntPtr ptr);
        [DllImport(@"D:\ITI_CEI_2017\PhotoCAD\x64\Debug\PhotoCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Return20();
        [DllImport(@"D:\ITI_CEI_2017\PhotoCAD\x64\Debug\PhotoCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int MainFunction(string photoFullPath, string folderPath, string fileName, double[] cornerPoints, char otherTransformation, bool isWarped);

        // Member Variables
        public DependencyObject SelectedElement { get; set; }
        public string FileName { get; set; }
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
                pointsNo = 4;
                DrawLines();
            }

            char otherTransformation = 'n';
            // Call the Warping Function from the dll
            MainFunction(MainWindowProperty.InputFileName_TB.Text, MainWindowProperty.OutputFilePath_TB.Text, FileName, pointsArray, otherTransformation, false);
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
            MainFunction(MainWindowProperty.InputFileName_TB.Text, MainWindowProperty.OutputFilePath_TB.Text, FileName, pointsLocation, otherTransformation, false);
            // Clear All Array Elements
            Array.Clear(cropPoints, 0, 4);

            // Remove all circles from the screen
            CropCanvas.Children.Clear();
            pointsNo = 0;
        }

        private void Rotate_BTN_Click(object sender, RoutedEventArgs e)
        {
            char otherTransformation = 'r';
            MainFunction(MainWindowProperty.InputFileName_TB.Text, MainWindowProperty.OutputFilePath_TB.Text, FileName, pointsLocation, otherTransformation, false);
        }

        private void Mirror_BTN_Click(object sender, RoutedEventArgs e)
        {
            char otherTransformation = 'm';
            MainFunction(MainWindowProperty.InputFileName_TB.Text, MainWindowProperty.OutputFilePath_TB.Text, FileName, pointsLocation, otherTransformation, false);
        }

        private void FullSize_BTN_Click(object sender, RoutedEventArgs e)
        {
            char otherTransformation = 'f';
            MainFunction(MainWindowProperty.InputFileName_TB.Text, MainWindowProperty.OutputFilePath_TB.Text, FileName, pointsLocation, otherTransformation, false);
        }

        private void SaveImage_BTN_Click(object sender, RoutedEventArgs e)
        {
            char otherTransformation = 's';
            int success = MainFunction(MainWindowProperty.InputFileName_TB.Text, MainWindowProperty.OutputFilePath_TB.Text, FileName, pointsLocation, otherTransformation, false);
            if (success == 0)
            {
                MessageBoxResult boxResult = MessageBox.Show("The Photo Saved Successfully", "Saved Succeeded",
                       MessageBoxButton.OK, MessageBoxImage.None);
            }
        }

        private void ToEdgeDetection_BTN_Click(object sender, RoutedEventArgs e)
        {
            CloseCPPWindows();                        //To Close the C++ Window
            ImageWarping_tab.IsEnabled = false;
            EdgeDetection_tab.IsEnabled = true;
            EdgeDetection_tab.IsSelected = true;    //To Activate the Edge Detection Tab

            LoadImageToCanvas(EdgeDetectionCanvas);
        }

        public void LoadImageToCanvas(Canvas canvas)
        {
            // Get the Actual Height and Width of the Image
            using (FileStream fileStream = new FileStream(MainWindowProperty.InputFileName_TB.Text, FileMode.Open, FileAccess.Read))
            {
                BitmapFrame frame = BitmapFrame.Create(fileStream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                MainWindowProperty.ImageHeight = frame.PixelHeight;
                MainWindowProperty.ImageWidth = frame.PixelWidth;
                MainWindowProperty.ImageRatio = MainWindowProperty.ImageWidth / MainWindowProperty.ImageHeight;
            }

            // To Load the selected image to the Crop Canvas Background
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri(MainWindowProperty.InputFileName_TB.Text, UriKind.Relative));
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
            FileName = System.IO.Path.GetFileNameWithoutExtension(MainWindowProperty.InputFileName_TB.Text);

            //To Set the Canvas Size relative to the image
            canvas.Width = imgWidth;
            canvas.Height = imgHeight;

            //To Set the Canvas Size relative to the Canvas/Image
            Width = imgWidth;
            Height = (canvas.Height) + 35;
        }

    }
}
