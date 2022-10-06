using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poligonEditor.components
{
    internal class point : IDisposable
    {
        // Implementing Global Unique ID might be helpful in the future
        public Guid InstanceID { get; private set; }

        // Basic point information
        private int _x;
        private int _y;
        public int x
        {
            get => _x;
        }
        public int y
        {
            get => _y;
        }

        public point(int x, int y)
        {
            this.InstanceID = Guid.NewGuid();
            _x = x;
            _y = y;
        }
        // Function to draw point on a bitmap
        public void Draw(Bitmap drawArea)
        {
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.FillRectangle((Brush)Brushes.Black, _x, _y, 1, 1);

            }
        }

        public void Dispose()
        {
            
        }
    }
}
