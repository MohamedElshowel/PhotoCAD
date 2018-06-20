using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace Design.Presentation.Geometry
{
    public class GLine : GShape
    {
        public Line Line { get; set; }
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public GLine(GCanvas gCanvas, Point startPoint, Point EndPoint):base(gCanvas)
        {
            Shape = Line = new Line();
            Line.X1 = startPoint.X;
            Line.Y1 = startPoint.Y;
            //
            Line.X2 = EndPoint.X*Scale;
            Line.Y2 = EndPoint.Y*Scale;
            //
            GCanvas.Shapes.Add(this);
        }

    }
}
