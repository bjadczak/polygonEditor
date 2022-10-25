using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polygonEditor.components
{
    internal class Circle
    {
        public components.Point center;
        public float radius;

        public bool isBeingBuild
        {
            get;
            private set;
        }

        // Create pen for temporary Circle.
        Pen grayPen = new Pen(Color.Gray, components.Line.width);

        // Create pen full Circle.
        Pen blackPen = new Pen(Color.Black, components.Line.width);

        public Circle(Point center, float radius)
        {
            this.radius = 50;
            this.center = center;
            this.center.movePointByDelta(-this.radius, -this.radius);
            isBeingBuild = false;
        }
        public Circle(Point center)
        {
            this.center = center;
            this.radius = 0;
            isBeingBuild = true;
        }

        public void MoveRadius(Point edge)
        {
            if (!isBeingBuild) return;
            float radius = (float)Math.Sqrt(edge.getDistance(this.center));
            this.radius = radius;
        }

        public void finishMoving(Point edge)
        {
            if (!isBeingBuild) return;
            float radius = (float)Math.Sqrt(edge.getDistance(this.center));
            this.radius = radius;
            isBeingBuild = false;
        }

        public void Draw(Bitmap drawArea)
        {
            if (center is null || radius == 0) throw new InvalidOperationException("Cannot draw Circle");

            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.DrawEllipse(blackPen, center.x - radius, center.y - radius, (float)radius * 2, (float)radius * 2);
            }
        }
        public void DrawIncompleatCircle(Bitmap drawArea, Point edge)
        {
            if (center is null || radius == 0) return;

            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.DrawEllipse(grayPen, center.x - radius, center.y - radius, (float)radius * 2, (float)radius * 2);
            }
        }

        public bool IsOnLine(polygonEditor.components.Point p)
        {
            using (var path = new GraphicsPath())
            {
                using (var pen = new Pen(Brushes.Black, components.Line.width * 4))
                {
                    path.AddEllipse(center.x - radius, center.y - radius, (float)radius * 2, (float)radius * 2);
                    return path.IsOutlineVisible((System.Drawing.Point)(p), pen);
                }
            }
        }

        public static polygonEditor.components.Circle findFirstOnLine(IEnumerable<components.Circle> circles, components.Point point)
        {
            if (circles.Count() <= 0) return null;

            foreach (components.Circle c in circles)
            {
                if (c.IsOnLine(point)) return c;
            }

            return null;
        }

        public void moveCircle(float X, float Y)
        {
            this.center.movePointByDelta(X, Y);
        }

        public void enterResizing()
        {
            isBeingBuild = true;
        }

    }
    
}
