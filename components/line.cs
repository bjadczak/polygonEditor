using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        // Create pen.
        Pen blackPen = new Pen(Color.Black, 3);

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
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.DrawLine(blackPen, (System.Drawing.Point)Pt1, (System.Drawing.Point)Pt2);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}
