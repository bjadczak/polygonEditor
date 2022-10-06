using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poligonEditor.components
{
    internal class Point : IDisposable
    {
        // Implementing Global Unique ID might be helpful in the future
        public Guid InstanceID { get; private set; }

        // Basic point information
        public int x { get; private set; }
        public int y { get; private set; }

        public Point(int x, int y)
        {
            this.InstanceID = Guid.NewGuid();
            this.x = x;
            this.y = y;
        }
        // Function to draw point on a bitmap
        public void Draw(Bitmap drawArea)
        {
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.FillRectangle((Brush)Brushes.Black, x, y, 1, 1);
            }
        }

        public static explicit operator System.Drawing.Point(components.Point p) => new System.Drawing.Point(p.x, p.y);

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
