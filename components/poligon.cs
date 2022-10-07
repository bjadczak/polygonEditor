using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        private const int precisionOfFinishing = 16;

        // Create pen for temporary line.
        Pen grayPen = new Pen(Color.Gray, 3);

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

        public void addNewPoint(components.Point pt)
        {
            if (isPoligonComplet()) throw new InvalidOperationException("Poligon already complet");

            if (pt.getDistance(startingPoint) < precisionOfFinishing) pt = startingPoint;

            lines.Add(new components.Line(finishingPoint, pt));
            finishingPoint = pt;

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
    }
}
