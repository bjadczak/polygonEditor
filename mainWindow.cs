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
using static System.Windows.Forms.LinkLabel;

namespace poligonEditor
{
    public partial class mainWindow : Form
    {
        // Actual object we are going draw on, like a "canvas"
        private Bitmap drawArea;

        List<components.Point> points = new List<components.Point>();

        List<components.Line> lines = new List<components.Line>();

        components.Line tmpLine = null;

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
            if (!(drawArea is null)) drawArea.Dispose();
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
            foreach (components.Line l in lines)
            {
                l.Draw(drawArea);
            }
            mainPictureBox.Refresh();
        }

        private void clickOnPictureBox(object sender, MouseEventArgs e)
        {
            // Deside which button was pressed
            switch (e.Button) { 
                case MouseButtons.Left:
                    if(tmpLine is null)
                    {
                        tmpLine = new components.Line(new poligonEditor.components.Point(e.X, e.Y));
                    }
                    else
                    {
                        tmpLine.SetSecondPoint(new poligonEditor.components.Point(e.X, e.Y));
                        lines.Add(tmpLine);
                        tmpLine.Draw(drawArea);
                        mainPictureBox.Refresh();
                        tmpLine = null;
                    }
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
