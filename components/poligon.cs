using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace poligonEditor.components
{
    internal class Poligon : IDisposable
    {
        // Implementing Global Unique ID might be helpful in the future
        public Guid InstanceID { get; private set; }

        public List<components.Line> lines;

        public components.Point startingPoint { get; private set; }
        private components.Point finishingPoint;

        // Create pen for temporary line.
        Pen grayPen = new Pen(Color.Gray, components.Line.width);

        // Count of lines
        public int Count { get => lines.Count; }

        // Constructor for empty poligon and one with only one point (startingPoint)
        public Poligon()
        {
            lines = new List<components.Line>();
            InstanceID = Guid.NewGuid();
            startingPoint = null;
            finishingPoint = null;
        }
        public Poligon(components.Point startingPoint)
        {
            lines = new List<components.Line>();
            InstanceID = Guid.NewGuid();
            finishingPoint = this.startingPoint = startingPoint;
        }

        public bool isPoligonComplet() => !(startingPoint is null) && !(finishingPoint is null) && lines.Count > 0 && startingPoint.InstanceID == finishingPoint.InstanceID;

        // We build our poligon by those two methods, adding points on the way
        public void addFirstPoint(components.Point pt)
        {
            if (isPoligonComplet()) throw new InvalidOperationException("Poligon already complet");
            if ((startingPoint is null)) throw new InvalidOperationException("Poligon already has starting point");

            startingPoint = finishingPoint = pt;
        }

        public bool addNewPoint(components.Point pt, IEnumerable<components.Point> allPoints)
        {
            if (isPoligonComplet()) throw new InvalidOperationException("Poligon already complet");

            using (var tmp = checkOverallping(pt, allPoints))
            {
                if (!(tmp is null))
                    return false;
            }
            using (var tmp = checkOverallping(pt, this.GetPoints()))
            {
                if (!(tmp is null))
                {
                    if (tmp.InstanceID == startingPoint.InstanceID)
                    {
                        pt = startingPoint;
                    }
                    else return false;
                }
            }

            lines.Add(new components.Line(finishingPoint, pt));
            finishingPoint = pt;
            return true;

        }

        public static components.Point checkOverallping(components.Point pt, IEnumerable<components.Point> allPoints, components.Point exceptThis = null)
        {
            if (allPoints.Count() > 0)
            {
                foreach (components.Point p in allPoints)
                {
                    if (p.isOverlapping(pt))
                        if (!(exceptThis is null) && exceptThis == p) return null;
                        else return p;
                }
            }
            return null;
        }

        public void Draw(Bitmap drawArea)
        {
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                foreach (components.Line l in lines)
                {
                    l.Draw(drawArea);
                }
            }
        }

        public void Dispose()
        {
            foreach(components.Line l in lines) l.Dispose();
        }

        public void DrawIncompleteLine(Bitmap drawArea, components.Point pt)
        {
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.DrawLine(grayPen, (System.Drawing.Point)finishingPoint, (System.Drawing.Point)pt);
            }
        }
        public IEnumerable<poligonEditor.components.Line> GetLines()
        {
            foreach (components.Line l in lines)
            {
                yield return l;
            }
        }
        public static IEnumerable<poligonEditor.components.Line> GetLinesFrom(IEnumerable<poligonEditor.components.Poligon> poligons)
        {
            foreach(components.Poligon p in poligons)
            {
                foreach(poligonEditor.components.Line l in p.GetLines())
                {
                    yield return l;
                }
            }
        }
        public IEnumerable<poligonEditor.components.Point> GetPoints()
        {
            foreach (components.Line l in lines)
            {
                yield return l.Pt1;
            }
        }
        public static IEnumerable<poligonEditor.components.Point> GetPointsFrom(IEnumerable<poligonEditor.components.Poligon> poligons)
        {
            foreach (components.Poligon p in poligons)
            {
                foreach (poligonEditor.components.Point pt in p.GetPoints())
                {
                    yield return pt;
                }
            }
        }
        public bool containsLine(components.Line line) => lines.Contains(line);
        public bool containsPoint(components.Point point)
        {
            foreach(var l in lines)
            {
                if (l.Pt1 == point) return true; 
            }
            return false;
        }

        public void movePoligon(poligonEditor.components.Point firstPoint, poligonEditor.components.Point secondPoint)
        {
            foreach(var p in GetPoints())
            {
                p.movePointByDelta(secondPoint.x - firstPoint.x, secondPoint.y - firstPoint.y);
            }
        }

        public void deletePoint(components.Point point)
        {
            if (Count <= 2) throw new InvalidOperationException("Too few points in poligon");
            components.Line l1 = null, l2 = null;
            foreach(var l in lines)
            {
                if (l.Pt2 == point) l1 = l;
                if (l.Pt1 == point) l2 = l;
            }
            if (l1 is null || l2 is null) throw new InvalidOperationException("Uknown error");
            if (point == startingPoint) startingPoint = finishingPoint = l1.Pt1;
            l1.setPt2(l2.Pt2);
            lines.Remove(l2);


        }
        
    }

}
