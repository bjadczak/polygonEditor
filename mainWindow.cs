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

        // List of poligons we have on the screen
        List<components.Poligon> poli = new List<components.Poligon>();

        // Listo of references to all points(verteces)
        List<components.Point> allPoints = new List<components.Point>();

        components.Poligon tmpPoli = null;

        // Moste recent point where we saw mouse
        components.Point movingPoint = null;

        // Point of poligon we are moving
        components.Point holdingPoint = null;

        //

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

            // Drawing poligon that is being build
            if (!(tmpPoli is null))
            {
                tmpPoli.Draw(drawArea);
                tmpPoli.DrawIncompleteLine(drawArea, movingPoint);
            }

            // Draw all poligons taht we have stored
            foreach (components.Poligon p in poli)
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
                    // TODO: Add stopping of creation of new poligon
                    if(tmpPoli is null)
                    {
                        components.Point tmpPoint = new components.Point(e.X, e.Y);
                        tmpPoli = new components.Poligon(tmpPoint);
                        allPoints.Add(tmpPoint);
                    }
                    else
                    {
                        movingPoint = null;
                        components.Point tmpPoint = new components.Point(e.X, e.Y);
                        tmpPoli.addNewPoint(tmpPoint);
                        allPoints.Add(tmpPoint);

                        tmpPoli.Draw(drawArea);
                        mainPictureBox.Refresh();
                        if (tmpPoli.isPoligonComplet())
                        {
                            poli.Add(tmpPoli);
                            tmpPoli = null;
                        }
                    }
                    break;
                case MouseButtons.Right:
                    if (allPoints.Count <= 0 || !(tmpPoli is null)) break;
                    components.Point actPoint = new components.Point(e.X, e.Y);
                    components.Point closest = allPoints[0];

                    foreach (components.Point p in allPoints)
                    {
                        if (actPoint.getDistance(p) < actPoint.getDistance(closest)) closest = p;
                    }

                    holdingPoint = closest;

                    drawOnPictureBox();

                    break;
                case MouseButtons.Middle:
                    break;
                default:
                    throw new ArgumentException("Event argument contained invalid Button propperty");
            }
        }

        // We draw a gray line to show user that new line is going to appear when
        // they click
        private void mouseMoveOverCanvas(object sender, MouseEventArgs e)
        {
            //if (tmpPoli is null) return;

            movingPoint = new components.Point(e.X, e.Y);

            if (!(holdingPoint is null)) holdingPoint.movePoint(movingPoint);

            drawOnPictureBox();

            mainPictureBox.Refresh();
        }

        private void endOfClickOnPictureBox(object sender, MouseEventArgs e)
        {
            holdingPoint = null;
        }
    }
}
