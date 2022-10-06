using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using poligonEditor.components;
using static System.Windows.Forms.LinkLabel;

namespace poligonEditor
{
    public partial class mainWindow : Form
    {
        // Actual object we are going draw on, like a "canvas"
        private Bitmap drawArea;

        List<point> points = new List<point>();

        public mainWindow()
        {
            InitializeComponent();

            // We create and attache our "canvas" to pictureBox
            drawArea = new Bitmap(mainPictureBox.Size.Width, mainPictureBox.Size.Height);
            mainPictureBox.Image = drawArea;

            drawOnPictureBox();
        }

        // When we resize our window we need to adapt our bitmap size
        private void mainWindow_Resize(object sender, EventArgs e)
        {
            if (drawArea != null) drawArea.Dispose();
            drawArea = new Bitmap(mainPictureBox.Size.Width, mainPictureBox.Size.Height);
            mainPictureBox.Image = drawArea;
            drawOnPictureBox();
        }

        // Method that calls to draw specific vertixces and lines
        private void drawOnPictureBox()
        {
            // Fill bitmap with white background
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.Clear(Color.White);
            }
            foreach (point p in points)
            {
                p.Draw(drawArea);
            }
            mainPictureBox.Refresh();
        }

        private void clickOnPictureBox(object sender, MouseEventArgs e)
        {
            // Deside which button was pressed
            switch (e.Button) { 
                case MouseButtons.Left:
                    // We schould add a point to the list
                    point p = new point(e.X, e.Y);
                    points.Add(p);
                    p.Draw(drawArea);
                    mainPictureBox.Refresh();
                    break;
                case MouseButtons.Right:
                    break;
                case MouseButtons.Middle:
                    break;
                default:
                    throw new ArgumentException("Event argument contained invalid Button propperty");
            }
        }
    }
}
