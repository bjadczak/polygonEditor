using poligonEditor.components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

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
        // Line of poligon we are moving
        components.Line holdingLine = null;

        // Active mode of input
        misc.enums.mode activeMode = misc.enums.mode.addingPoint;


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

        private void addAPoint(int X, int Y)
        {
            components.Point actPoint = new components.Point(X, Y);
            // Check if we are creating new poligon from scrach
            if (tmpPoli is null)
            {
                // Check if we are overlapping with existing poligon point
                if (!(poligonEditor.components.Poligon.checkOverallping(actPoint, allPoints) is null)) return;
                tmpPoli = new components.Poligon(actPoint);
                allPoints.Add(actPoint);
            }
            else
            {
                movingPoint = null;
                if (tmpPoli.addNewPoint(actPoint, allPoints))
                {
                    allPoints.Add(actPoint);

                    if (tmpPoli.isPoligonComplet())
                    {
                        poli.Add(tmpPoli);
                        tmpPoli = null;
                        drawOnPictureBox();
                    }
                    else
                    {
                        tmpPoli.Draw(drawArea);
                        mainPictureBox.Refresh();
                    }
                }
            }
        }
        private void moveAPoint(int X, int Y)
        {
            components.Point actPoint = new components.Point(X, Y);
            // Moving a point we check which point is selected
            poligonEditor.components.Point closest = poligonEditor.components.Point.findClosest(allPoints, actPoint);

            if (!(closest is null))
            {
                holdingPoint = closest;

                movingPoint = actPoint;

                drawOnPictureBox();
            }
        }
        private void resetOnRMB()
        {
            holdingPoint = null;
            if (!(tmpPoli is null)) foreach (components.Point p in tmpPoli.GetPoints())
            {
                allPoints.Remove(p);
            }
            tmpPoli = null;

            drawOnPictureBox();
        }
        private void moveAnEdge(int X, int Y)
        {
            components.Point actPoint = new components.Point(X, Y);
            poligonEditor.components.Line closest = poligonEditor.components.Line.findFirstOnLine(poligonEditor.components.Poligon.GetLinesFrom(poli), actPoint);
            if (!(closest is null))
            {
                holdingLine = closest;

                movingPoint = actPoint;

                drawOnPictureBox();
            }

        }

        private void clickOnPictureBox(object sender, MouseEventArgs e)
        {
            // Deside which button was pressed
            switch (e.Button) { 
                case MouseButtons.Left:
                    // Chceck what mode is active
                    switch (activeMode)
                    {
                        case misc.enums.mode.addingPoint:
                            {
                                addAPoint(e.X, e.Y);
                            }
                            break;
                        case misc.enums.mode.movingPoint:
                            {
                                moveAPoint(e.X, e.Y);
                            }
                            break;
                        case misc.enums.mode.movingEdge:
                            {
                                moveAnEdge(e.X, e.Y);
                            }
                            break;
                        default:
                            break;
                    }
                    
                    break;
                case MouseButtons.Right:
                    // We open context menu
                    // First we get rid of any poligos in creation, and lose moving points
                    resetOnRMB();

                    contextMenuStrip.Show(this, new System.Drawing.Point(e.X, e.Y));

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
            if (tmpPoli is null && holdingPoint is null && holdingLine is null) return;

            poligonEditor.components.Point tmp = movingPoint;
            movingPoint = new components.Point(e.X, e.Y);

            if (!(holdingPoint is null)) holdingPoint.movePoint(movingPoint);
            else if (!(holdingLine is null) && !(tmp is null))
            {
                holdingLine.moveLine(tmp, movingPoint);
            }

                drawOnPictureBox();

            mainPictureBox.Refresh();
        }

        private void endOfClickOnPictureBox(object sender, MouseEventArgs e)
        {
            holdingPoint = null;
            holdingLine = null;
        }

        private void resetContextMenu()
        {
            foreach(ToolStripMenuItem menuItem in contextMenuStrip.Items)
            {
                menuItem.Checked = false;
            }
        }

        private void addAPointMenuItem_Click(object sender, EventArgs e)
        {
            resetContextMenu();
            addAPointMenuItem.Checked = true;
            activeMode = misc.enums.mode.addingPoint;
        }

        private void movaAPointMenuItem_Click(object sender, EventArgs e)
        {
            resetContextMenu();
            movaAPointMenuItem.Checked = true;
            activeMode = misc.enums.mode.movingPoint;
        }

        private void moveAnEdgeMenuItem_Click(object sender, EventArgs e)
        {
            resetContextMenu();
            moveAnEdgeMenuItem.Checked = true;
            activeMode = misc.enums.mode.movingEdge;
        }
    }
}
