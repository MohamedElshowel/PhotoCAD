//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;

//namespace Design.Presentation.Geometry
//{
//    public class GeometryEngine
//    {
//        public GCanvas GCanvas { get; set; }
        
//        public Dictionary<string,List<GShape>> Shapes { get; set; }
//        public GeometryEngine()
//        {
//            Shapes = new Dictionary<string, List<GShape>>()
//            {
//                {"Supports",new List<GShape>()},
//                {"DistributedLoad",new List<GShape>()},
//                {"ConcentratedLoad",new List<GShape>()},
//                {"Beams",new List<GShape>()},
//                {"Grid",new List<GShape>()},
//                {"Points",new List<GShape>()},
//            };
            
//            GCanvas = new GCanvas();
//        }
//        public void AddShape(ShapeType shapes)
//        {
//            switch (shapes)
//            {
//                case ShapeType.Triangle:
//                    DrawTriangle();
//                    break;
//                case ShapeType.Rectangle:
//                    DrawRectangle();
//                    break;
//                case ShapeType.Circle:
//                    DrawCircle();
//                    break;
//                case ShapeType.Arrow:
//                    DrawArrow();
//                    break;
//                case ShapeType.Roller:
//                    DrawRoller();
//                    break;
//                case ShapeType.Hinged:
//                    DrawHinge();
//                    break;
//                case ShapeType.Fixed:
//                    DrawFixed();
//                    break;
//                case ShapeType.Point:
//                    DrawPoint();
//                    break;
//                case ShapeType.Cross:
//                    DrawCross();
//                    break;
//                case ShapeType.ArrowLoad:
//                    DrawArrowLoad();
//                    break;
//                default:
//                    break;
//            }
//        }

//        #region Helper Functions
//        private void DrawTriangle()
//        {

//        }
//        private void DrawRectangle()
//        {


//        }
//        private void DrawRoller()
//        {

//        }
//        private void DrawCircle()
//        {

//        }
//        private void DrawArrow()
//        {

//        }
//        private void DrawHinge()
//        {

//        }

//        private void DrawFixed()
//        {

//        }
//        private void DrawPoint()
//        {

//        }
//        private void DrawCross()
//        {

//        }
//        private void DrawArrowLoad()
//        {

//        }
//        #endregion
//    }
//}
