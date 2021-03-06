﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Design.Presentation.Geometry
{
    public class GCircle : GShape
    {
        private double scale = 1;

        public double Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        public Ellipse Circle { get; set; }
        public double Radius { get; set; }
        public Point Position { get; set; }

        public GCircle(GCanvas gCanvas, Point position, double radius) : base(gCanvas)
        {
            //set shape
            Shape = Circle = new Ellipse();
            //set shape specific property
            Position = position;
            Radius = radius * Scale;
            //
            Circle.Height = Radius;
            Circle.Width = Radius;
            var left = Position.X - (Radius / 2);
            var top = Position.Y - (Radius / 2);
            Circle.Margin = new Thickness(left, top, 0, 0);
            Fill = Brushes.Transparent;
            Circle.RenderTransform = new TransformGroup();
        }

        public override void SetScale(double value)
        {
            ((TransformGroup)Circle.RenderTransform).Children.Add(
                new ScaleTransform(value, value, Radius / 2, 0));
        }
        public override void SetTranslate(double valueX,double valueY)
        {
            ((TransformGroup)Circle.RenderTransform).Children.Add(
                 new TranslateTransform( valueX,valueY));
        }


    }
}
