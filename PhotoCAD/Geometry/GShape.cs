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

        public GCanvas GCanvas { get; set; }
        public Shape Shape { get; set; }
        public Thickness Thickness { get; set; }
        public Visibility Visibility { get; set; }
        public Brush Stroke { get; set; }
        public int StrokeThickness { get; set; }
        public Brush Fill { get; set; }
        public int Id { get; set; }
        public GShape(GCanvas gCanvas)
        {
            GCanvas = gCanvas;
            Thickness = new Thickness(101, -11, 362, 250);
            Visibility = Visibility.Visible;
            Stroke = Brushes.Black;
            StrokeThickness = 1;
            Fill = Brushes.Red;
            //
            Id = GeometryEngine.Id;
            GeometryEngine.Id++;
        }



        public virtual void Render()
        {
            Shape.Stroke = Stroke;
            Shape.Fill = Fill;
            Shape.Visibility = Visibility;
            Shape.StrokeThickness = StrokeThickness;
            if (GCanvas.Canvas.Children.Contains(Shape))
            {
                return;
            }
            GCanvas.Canvas.Children.Add(Shape);

        }
        public virtual void Remove()
        {/*----------> Consider Revision*/
            if (!GCanvas.Canvas.Children.Contains(Shape)) return;
            GCanvas.Canvas.Children.Remove(Shape);

        }
        public virtual void Hide()
        {
            Shape.Visibility = Visibility = Visibility.Collapsed;
        }
        public virtual void New()
        {

        }
        public virtual void SetScale(double value)
        {

        }
        public virtual void SetTranslate(double valueX,double valueY)
        {

        }
        public virtual void Rotate(double angle)
        {

        }

    }
}
