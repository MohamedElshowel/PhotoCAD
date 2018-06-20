using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Design.Presentation.Geometry
{
    public class GLine : GShape
    {
        private double scale=1;

        public double Scale
        {
            get { return scale; }
            set { scale = value; }
        }



        public Line Line { get; set; }
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public GLine(GCanvas gCanvas, Point startPoint, Point endPoint):base(gCanvas)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            Shape = Line = new Line();
            Line.X1 = StartPoint.X /*Scale*/;
            Line.Y1 = StartPoint.Y;
            //
            Line.X2 = EndPoint.X /*Scale*/;
            Line.Y2 = EndPoint.Y;
            //Add this Shape to Canvas
            Line.RenderTransform = new TransformGroup();
        }
        public override void SetScale(double value)
        {
                    ((TransformGroup)Line.RenderTransform).Children.Add(
                        new ScaleTransform(value,value,Line.X1,Line.Y1));
        }
        public override void SetTranslate(double valueX,double valueY)
        {

            ((TransformGroup)Line.RenderTransform).Children.Add(
                        new TranslateTransform(valueX, valueY));
        }
    }
}
