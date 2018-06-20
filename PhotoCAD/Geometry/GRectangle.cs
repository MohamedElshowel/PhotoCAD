using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Design.Presentation.Geometry
{
    public class GRectangle : GShape
    {
        public Rectangle Rectangle { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double RadiusX { get; set; }
        public double RadiusY { get; set; }

        public Point Position { get; set; }
        public GRectangle(GCanvas gCanvas, double width, double height, Point position) : base(gCanvas)
        {
            Width = width;
            Height = height;
            Position = position;
            //
            Shape = Rectangle = new Rectangle();
            Rectangle.Width = Width;
            Rectangle.Height = Height;
            Rectangle.RadiusX = RadiusX;
            Rectangle.RadiusY = RadiusY;

            var left = Position.X - (Width / 2);
            var top = Position.Y - (Height / 2);
            Rectangle.Margin = new Thickness(left, top, 0, 0);
            Fill = Brushes.Transparent;
            //Add this Shape to Canvas
        }
        public override void Render()
        {
            base.Render();
        }
        public override void Remove()
        {
            base.Remove();
        }
        public override void Hide()
        {
            base.Hide();
        }

        public override void SetScale(double value)
        {
            //var left =  Position.X +Width;
            //var top = (Position.Y-Height) / 2;
            Rectangle.RenderTransform = new ScaleTransform(value, value, Width / 2, -Height / 2);
        }
    }
}
