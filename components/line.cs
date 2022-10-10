﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace poligonEditor.components
{
    internal class Line : IDisposable
    {
        // Implementing Global Unique ID might be helpful in the future
        public Guid InstanceID { get; private set; }

        // Basic line information
        public components.Point Pt1 { get; private set; }
        public components.Point Pt2 { get; private set; }

        public const int width = 1;

        // Method of drawing lines
        public static bool useBresenhams = false;


        // Create drawing mechanisms
        Bresenham b = new Bresenham((Brush)Brushes.Black);
        defaultDrawing d = new defaultDrawing(new Pen(Color.Black, width));

        // Information is line compleat
        public bool isLineComplet 
        { 
            get { return !(Pt1 is null || Pt2 is null); } 
        }

        // Default constructor
        public Line(Point pt1, Point pt2)
        {
            InstanceID = Guid.NewGuid();
            Pt1 = pt1;
            Pt2 = pt2;
        }

        // Constructor to be used during creation of line in editor
        public Line(Point pt1)
        {
            InstanceID = Guid.NewGuid();
            Pt1 = pt1;
            Pt2 = null;
        }
        public Line()
        {
            Pt1 = Pt2 = null;
        }

        // Method that updated line that is during the creation
        public void SetSecondPoint(Point pt2)
        {
            if (isLineComplet) throw new InvalidOperationException("Trying to use method in instance that already has 2 points");

            Pt2 = pt2;

        }

        public void Draw(Bitmap drawArea)
        {
            if (Pt1 is null || Pt2 is null) throw new InvalidOperationException("Cannot draw lines without both points");
            
            lineDrawing drawing = useBresenhams ? (lineDrawing)b: (lineDrawing)d;


            drawing.draw(drawArea, Pt1, Pt2);
            Pt1.Draw(drawArea);
            Pt2.Draw(drawArea);
        }

        

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        // Check if point is on this line
        public bool IsOnLine(poligonEditor.components.Point p)
        {
            using (var path = new GraphicsPath())
            {
                using (var pen = new Pen(Brushes.Black, width * 4))
                {
                    path.AddLine((System.Drawing.Point)(Pt1), (System.Drawing.Point)(Pt2));
                    return path.IsOutlineVisible((System.Drawing.Point)(p), pen);
                }
            }
        }

        // Find first line that is on the given point
        public static poligonEditor.components.Line findFirstOnLine(IEnumerable<components.Line> lines, components.Point point)
        {
            if (lines.Count() <= 0) return null;

            foreach (components.Line l in lines)
            {
                if (l.IsOnLine(point)) return l;
            }

            return null;
        }

        public void moveLine(poligonEditor.components.Point firstPoint, poligonEditor.components.Point secondPoint)
        {
            Pt1.movePointByDelta(secondPoint.x - firstPoint.x, secondPoint.y - firstPoint.y);
            Pt2.movePointByDelta(secondPoint.x - firstPoint.x, secondPoint.y - firstPoint.y);
        }
        public void setPt1(components.Point newPt1)
        {
            Pt1 = newPt1;
        }

        public void setPt2(components.Point newPt2)
        {
            Pt2 = newPt2;
        }

        internal class Bresenham : lineDrawing
        {
            private Brush brush;
            public Bresenham(Brush brush)
            {
                this.brush = brush;
            }
            public void draw(Bitmap drawArea, Point point1, Point point2)
            {
                // We can "flip" x and y axis to draw negative and positive slopes
                // Negative and positive as in what is the sign of first derivative
                if (Math.Abs(point2.y - point1.y) < Math.Abs(point2.x - point1.x))
                {
                    // We can also "flip" points first and second to always go "right"
                    if (point1.x > point2.x)
                    {
                        drawPositiveSlope(drawArea, point2, point1);
                    }
                    else
                    {
                        drawPositiveSlope(drawArea, point1, point2);
                    }
                }
                else
                {
                    // Same as in top comment
                    if (point1.y > point2.y)
                    {
                        drawNegativeSlope(drawArea, point2, point1);
                    }
                    else
                    {
                        drawNegativeSlope(drawArea, point1, point2);
                    }
                }

            }

            public void drawPositiveSlope(Bitmap drawArea, Point point1, Point point2)
            {
                int dx = (int)(point2.x - point1.x);
                int dy = (int)(point2.y - point1.y);
                int stepY = 1;
                
                // If we are going "down" we change the step direction
                if(dy < 0)
                {
                    stepY *= -1;
                    dy *= -1;
                }
                
                int diff = (2 * dy) - dx;
                components.Point point = new components.Point(point1.x, point1.y);

                for(int i = 0; i<=Math.Abs(dx); i++)
                {
                    drawOnePoint(drawArea, point);
                    // We move either to "right" or "up-right"
                    if(diff > 0)
                    {
                        point.movePointByDelta(1, stepY);
                        diff += 2 * (dy - dx);
                    }
                    else
                    {
                        point.movePointByDelta(1, 0);
                        diff += 2 * dy;
                    }
                }

            }

            public void drawNegativeSlope(Bitmap drawArea, Point point1, Point point2)
            {
                int dx = (int)(point2.x - point1.x);
                int dy = (int)(point2.y - point1.y);
                int stepX = 1;

                // If we are going "down" we change the step direction
                if (dx < 0)
                {
                    stepX *= -1;
                    dx *= -1;
                }

                int diff = (2 * dx) - dy;
                components.Point point = new components.Point(point1.x, point1.y);

                for (int i = 0; i <= Math.Abs(dy); i++)
                {
                    drawOnePoint(drawArea, point);
                    // We move either to "right" or "up-right"
                    if (diff > 0)
                    {
                        point.movePointByDelta(stepX, 1);
                        diff += 2 * (dx - dy);
                    }
                    else
                    {
                        point.movePointByDelta(0, 1);
                        diff += 2 * dx;
                    }
                }
            }


            private void drawOnePoint(Bitmap drawArea, Point point)
            {
                using (Graphics g = Graphics.FromImage(drawArea))
                {
                    g.FillRectangle(brush, point.x, point.y, 1, 1);
                }
            }
        }

        internal class defaultDrawing : lineDrawing
        {
            private Pen pen;

            public defaultDrawing(Pen pen)
            {
                this.pen = pen;
            }

            public void draw(Bitmap drawArea, Point point1, Point point2)
            {
                using (Graphics g = Graphics.FromImage(drawArea))
                {
                    g.DrawLine(pen, (System.Drawing.Point)point1, (System.Drawing.Point)point2);
                }
            }
        }

    }
    interface lineDrawing
    {
        void draw(Bitmap drawArea, components.Point point1, components.Point point2);
    }
}
