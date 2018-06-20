using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Design.Presentation.Geometry
{
    /// <summary>
    /// Draws an Arrow
    /// </summary>
    public sealed class Arrow : GShape
    {
        public double Length { get; set; }
        public double HeadHeight { get; set; }
        public double HeadAngle { get; set; }

        public List<GLine> Lines { get; set; }
        public Point InsertionPoint { get; set; }
        public Direction Direction { get; set; }
        public Arrow(GCanvas gCanvas, Point insertionPoint, double length) : base(gCanvas)
        {
            GCanvas = gCanvas;
            InsertionPoint = insertionPoint;
            Length = length;
            HeadHeight = 5;
            var body = new GLine(GCanvas, InsertionPoint, new Point(InsertionPoint.X, InsertionPoint.Y + Length));

            var headRightLine = new GLine(GCanvas, InsertionPoint, new Point(InsertionPoint.X + HeadHeight, InsertionPoint.Y + HeadHeight));
            var headLeftLine = new GLine(GCanvas, InsertionPoint, new Point(InsertionPoint.X - HeadHeight, InsertionPoint.Y + HeadHeight));

            Lines = new List<GLine>()
            {
                body,
                headRightLine,
                headLeftLine
            };

            foreach (var line in Lines)
            {
                line.Line.RenderTransform = new TransformGroup();
            }

        }
        public void Rotate(double angle)
        {
            foreach (var line in Lines)
            {
                ((TransformGroup)line.Line.RenderTransform).Children.Add(
                    new RotateTransform(angle, InsertionPoint.X, InsertionPoint.Y));
            }
        }
        public override void Remove()
        {
            Lines.ForEach(e => e.Remove());
            Lines.Clear();


        }

        public override void Render()
        {
            foreach (var line in Lines)
            {
                line.Fill = Fill;
                line.StrokeThickness = StrokeThickness;
                line.Thickness = Thickness;
                line.Stroke = Stroke;
                line.Visibility = Visibility;
            }

            Lines.ForEach(e => e.Render());

        }
        public override void Hide()
        {
            Lines.ForEach(e => e.Hide());
        }

        public override void SetScale(double value)
        {
            ((TransformGroup)Lines[0].Line.RenderTransform).Children.Add(
                new ScaleTransform(value, value, Lines[0].Line.X1, Lines[0].Line.Y1)); //set the body scale;

            ((TransformGroup)Lines[1].Line.RenderTransform).Children.Add(
                new ScaleTransform(value, value, Lines[0].Line.X1, Lines[1].Line.Y1)); //set the body scale;

            ((TransformGroup)Lines[1].Line.RenderTransform).Children.Add(
                new ScaleTransform(value, value, Lines[0].Line.X1, Lines[1].Line.Y1)); //set the body scale;

        }

    }

}
