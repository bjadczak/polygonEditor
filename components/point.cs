using polygonEditor.misc;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polygonEditor.components
{
    internal class Point : IDisposable
    {
        // Implementing Global Unique ID might be helpful in the future
        public Guid InstanceID { get; private set; }

        // Basic point information
        public float x { get; private set; }
        public float y { get; private set; }

        public Line first;
        public Line second;

        // Size of displayed point
        const int pointSize = 8;

        public Point(float x, float y)
        {
            this.InstanceID = Guid.NewGuid();
            this.x = x;
            this.y = y;
        }
        public Point(Point p)
        {
            this.InstanceID = Guid.NewGuid();
            this.x = p.x;
            this.y = p.y;
        }
        // Function to draw point on a bitmap
        public void Draw(Bitmap drawArea)
        {
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                drawCircle(g, x, y);
            }
        }

        public static explicit operator System.Drawing.Point(components.Point p) => new System.Drawing.Point((int)p.x, (int)p.y);

        public void Dispose()
        {
            
        }
        // Method to recive comparable distance to a second point
        public static double getDistance(components.Point firstPoint, components.Point secondPoint)
        {
            return (firstPoint.x - secondPoint.x) * (firstPoint.x - secondPoint.x) + (firstPoint.y - secondPoint.y) * (firstPoint.y - secondPoint.y);
        }

        public double getDistance(components.Point secondPoint)
        {
            return components.Point.getDistance(this, secondPoint);
        }

        private void drawCircle(Graphics g, float x, float y)
        {  
            g.FillEllipse((Brush)Brushes.DarkCyan, x - pointSize/2, y - pointSize/2, pointSize, pointSize);
        }

        public void movePoint(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public void movePointByDelta(float x, float y)
        {
            this.x += x;
            this.y += y;
        }
        public void movePointByDelta(components.Point p)
        {
            this.x += p.x;
            this.y += p.y;
        }
        public void movePoint(components.Point p)
        {
            this.x = p.x;
            this.y = p.y;
        }

        // We can check if point is close enough to be selected
        public bool inSelectingDistance(components.Point mouse)
        {
            const int selectionDistance = 8*pointSize;
            return this.getDistance(mouse) < selectionDistance;
        }
        // We can check if placing here second point, will lead to overlapping
        public bool isOverlapping(components.Point secondPoint)
        {
            return this.getDistance(secondPoint) <= pointSize*5;
        }

        public static components.Point findClosest(IEnumerable<components.Point> points, components.Point point)
        {
            if (points.Count() <= 0) return null;

            components.Point closest = null;

            foreach (components.Point p in points)
            {
                if (p.inSelectingDistance(point) && (closest is null || point.getDistance(p) < point.getDistance(closest))) closest = p;
            }

            return closest;
        }
    }
}
