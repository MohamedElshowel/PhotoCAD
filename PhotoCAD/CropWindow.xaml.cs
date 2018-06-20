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
using System.Windows.Shapes;

namespace PhotoCAD
{
    /// <summary>
    /// Interaction logic for CropWindow.xaml
    /// </summary>
    public partial class CropWindow : Window
    {
        private UIElement uiElement;

        Point currentPoint = new Point();
        int pointsNo = 0;
        double[] pointsLocationX = new double[4];
        double[] pointsLocationY = new double[4];
        public CropWindow()
        {
            InitializeComponent();

            // Trying to Draw a Circle on Canvas;

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




            // Try Zone

        }
    }
}
