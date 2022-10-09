using System;
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

        // Create pen.
        Pen blackPen = new Pen(Color.Black, width);

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
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.DrawLine(blackPen, (System.Drawing.Point)Pt1, (System.Drawing.Point)Pt2);
            }
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

    }
}
