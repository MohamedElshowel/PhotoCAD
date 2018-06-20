using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Design.Presentation.Geometry
{
    public class Roller : GShape
    {
        private double scale = 1;

        public double Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public GLine Line { get; set; }
        public GCircle Circle { get; set; }

        public Point InsertionPoint { get; set; }
        public double Height { get; set; }
        public Roller(GCanvas gCanvas, Point insertionPoint) : base(gCanvas)
        {
            InsertionPoint = insertionPoint;
            GCanvas = gCanvas;
            Height = 20;
            var scaledHeight = Height * Scale;
            var sPoint = new Point(InsertionPoint.X - scaledHeight / (2 /**/), InsertionPoint.Y + scaledHeight);
            var ePoint = new Point(InsertionPoint.X + scaledHeight / (2 /**/), InsertionPoint.Y + scaledHeight);

            Line = new GLine(GCanvas, sPoint, ePoint);
            Circle = new GCircle(GCanvas, new Point(insertionPoint.X, InsertionPoint.Y + scaledHeight / 2), scaledHeight * Scale);

        }
        public override void Render()
        {


            Circle.Fill = Line.Fill = Fill;
            Circle.StrokeThickness = Line.StrokeThickness = StrokeThickness;
            Circle.Thickness = Line.Thickness = Thickness;
            Circle.Stroke = Line.Stroke = Stroke;
            Circle.Visibility = Line.Visibility = Visibility;
            //
            Line.Render();
            Circle.Render();
        }
        public override void Remove()
        {
            Line.Remove();
            Circle.Remove();
        }
        public override void Hide()
        {
            Line.Hide();
            Circle.Hide();
        }
        public override void SetScale(double value)
        {
            

            Line.SetTranslate( -(Height*value)/4 +Height/4, Height / 2);
            Line.SetScale(value);
            Circle.SetScale(value);
           
        }
    }
}
