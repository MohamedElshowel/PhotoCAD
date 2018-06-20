using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Design.Presentation.Geometry
{
  

    public class GShape
    {
        public double Scale { get; set; }
        public ShapeType ShapeType { get; set; }
        public GCanvas GCanvas { get; set; }
        public Shape Shape { get; set; }
        public Thickness Thickness { get; set; }
        public Visibility Visibility { get; set; }
        public Brush Stroke { get; set; }
        public int StrokeThickness { get; set; }
        public Brush Fill { get; set; }

        public GShape(GCanvas gCanvas)
        {
            GCanvas = gCanvas;
            Thickness = new Thickness(101, -11, 362, 250);
            Visibility = Visibility.Visible;
            Stroke = Brushes.Black;
            StrokeThickness = 1;
            Fill = Brushes.Red;
            Scale = 1;
            //

        }
        public virtual void Draw()
        {
            Shape.Stroke = Stroke;
            Shape.Fill = Fill;
            Shape.Visibility = Visibility;
            Shape.StrokeThickness = StrokeThickness;
            if (GCanvas.Canvas.Children.Contains(Shape)) return;
            GCanvas.Canvas.Children.Add(Shape);
        }
        public virtual void Remove()
        {
            if (!GCanvas.Canvas.Children.Contains(Shape)) return;
            GCanvas.Canvas.Children.Remove(Shape);
        }
        public virtual void Hide()
        {
            Shape.Visibility = Visibility=Visibility.Collapsed;
        }

    }
}
