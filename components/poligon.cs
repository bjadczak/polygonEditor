using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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

        public bool addNewPoint(components.Point pt, List<components.Point> allPoints)
        {
            if (isPoligonComplet()) throw new InvalidOperationException("Poligon already complet");

            using (var tmp = checkOverallping(pt, allPoints))
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

        public static components.Point checkOverallping(components.Point pt, List<components.Point> allPoints)
        {
            if (allPoints.Count > 0)
            {
                foreach (components.Point p in allPoints)
                {
                    if (p.isOverlapping(pt))
                        return p;
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
            throw new NotImplementedException();
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
                yield return l.Pt2;
            }
        }
    }

    internal class PoligonConstructor
    {
        public components.Poligon tmpPoligon;

        public void createPoligon()
        {
            //if (tmpPoligon is null)
            //{
            //    components.Point tmpPoint = new components.Point(e.X, e.Y);
            //    tmpPoligon = new components.Poligon(tmpPoint);
            //    //allPoints.Add(tmpPoint);
            //}
            //else
            //{
            //    //movingPoint = null;
            //    components.Point tmpPoint = new components.Point(e.X, e.Y);
            //    tmpPoligon.addNewPoint(tmpPoint);
            //    //allPoints.Add(tmpPoint);


            //    if (tmpPoligon.isPoligonComplet())
            //    {
            //        poli.Add(tmpPoligon);
            //        tmpPoligon = null;
            //        drawOnPictureBox();
            //    }
            //    else
            //    {
            //        tmpPoligon.Draw(drawArea);
            //        mainPictureBox.Refresh();
            //    }
            //}
        }

    }
}
