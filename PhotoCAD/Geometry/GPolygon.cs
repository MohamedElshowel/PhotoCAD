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
    public class GPolygon : GShape
    {
        private PointCollection _points;

        public Polygon Polygon { get; set; }
        public PointCollection Points
        {
            get { return _points; }
            set { _points = value; Polygon.Points = Points; }
        }
        public Point Position { get; }
        public Point Origin { get; set; }
        public GPolygon(GCanvas gCanvas, PointCollection points, [Optional] Point Position, [Optional] Point origin) : base(gCanvas)
        {
            ///Set shape properties
            Shape = Polygon = new Polygon();
            Points = points;
            this.Position = Position;
            
            if (origin == default(Point))
            {
                // origin = Points.FirstOrDefault();
            }
            else
            {
                Origin = origin;
            }
            //Set polygon Properties
            if (Position != default(Point))
            {
                var left = Position.X - Origin.X;
                var top = Position.Y - Origin.Y;
                Polygon.Margin = new Thickness(left, top, 0, 0);
            }

            Polygon.HorizontalAlignment = HorizontalAlignment.Left;
            Polygon.VerticalAlignment = VerticalAlignment.Center;
            GCanvas.Shapes.Add(this);

        }
        public GPolygon(GCanvas gCanvas) : this(gCanvas, new PointCollection(), default(Point), default(Point))
        {

        }
    }
}
