using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Design.Presentation.Geometry
{
    public class GCanvas
    {

        public List<GShape> Shapes { get; set; }
        //
        private Canvas _canvas;
        private Vector translateVector;

        public Canvas Canvas { get { return _canvas; } set { _canvas = value; Initializer(); } }
        // Zoom
        public Double ZoomMax { get; set; }
        public Double ZoomMin { get; set; }
        public Double ZoomSpeed { get; set; }
        public Double Zoom { get; set; }
        public bool IsMouseCaptured { get; private set; }
        private Point origin;  // Original Offset of Canvas
        private Point start;   // Original Position of the mouse
        public GCanvas()
        {
            ZoomMax = 25;
            ZoomMin = .5;
            ZoomSpeed = .001;
            Zoom = 1;
            Shapes = new List<GShape>();
        }

        private void Initializer()
        {
            Canvas.MouseWheel += Canvas_MouseWheel;
            Canvas.MouseLeftButtonUp += MyCanvas_MouseLeftButtonUp;
            Canvas.MouseMove += MyCanvas_MouseMove;
            Canvas.MouseLeftButtonDown += MyCanvas_MouseLeftButtonDown;
        }

        // Zoom on Mouse wheel
        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Zoom += ZoomSpeed * e.Delta; // Ajust zooming speed (e.Delta = Mouse spin value )
            if (Zoom < ZoomMin) { Zoom = ZoomMin; } // Limit Min Scale
            if (Zoom > ZoomMax) { Zoom = ZoomMax; } // Limit Max Scale

            Point mousePos = e.GetPosition(Canvas);

            if (Zoom > 1)
            {
                Canvas.RenderTransform = new ScaleTransform(Zoom, Zoom, mousePos.X, mousePos.Y); // transform Canvas size from mouse position
            }
            else
            {
                Canvas.RenderTransform = new ScaleTransform(Zoom, Zoom); // transform Canvas size
            }
        }

        /////



        void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Canvas.ReleaseMouseCapture();
        }

        void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Canvas.IsMouseCaptured) return;
            Point p = e.MouseDevice.GetPosition(Canvas);

            Matrix m = Canvas.RenderTransform.Value;
            m.OffsetX = origin.X + (p.X - start.X);
            m.OffsetY = origin.Y + (p.Y - start.Y);

            Canvas.RenderTransform = new MatrixTransform(m);
        }
        void MyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (Canvas.IsMouseCaptured) return;
            Canvas.CaptureMouse();

            start = e.GetPosition(Canvas);
            origin.X = Canvas.RenderTransform.Value.OffsetX;
            origin.Y = Canvas.RenderTransform.Value.OffsetY;
        }

        public void Render()
        {
            Shapes.ForEach(e => e.Draw());
        }
        public void Hide()
        {
            Shapes.ForEach(e => e.Remove());
        }
    }
}
