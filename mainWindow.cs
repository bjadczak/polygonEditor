using polygonEditor.components;
using polygonEditor.misc;
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
using System.Xml.Linq;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace polygonEditor
{
    public partial class mainWindow : Form
    {
        // Actual object we are going draw on, like a "canvas"
        private Bitmap drawArea;

        // List of poligons we have on the screen
        List<components.Polygon> polygons = new List<components.Polygon>();

        // Listo of references to points of polygons that is being build
        List<components.Point> buildingPoints = new List<components.Point>();

        components.Polygon tmpPoli = null;

        // Moste recent point where we saw mouse
        components.Point movingPoint = null;

        // Point of poligon we are moving
        components.Point holdingPoint = null;
        // Line of poligon we are moving
        components.Line holdingLine = null;
        // Polygon we are moving
        components.Polygon holdingPoligon = null;

        // Active mode of input
        misc.enums.mode activeMode = misc.enums.mode.addingPoint;

        // List of all active relations
        List<misc.IRelation> relations = new List<misc.IRelation>();

        // Active line
        components.Line activeLine = null;


        public mainWindow()
        {
            InitializeComponent();

            // We create and attache our "canvas" to pictureBox
            drawArea = new Bitmap(mainPictureBox.Size.Width, mainPictureBox.Size.Height);
            mainPictureBox.Image = drawArea;

            createDefoultPolygons();

            drawOnPictureBox();
        }

        // When we resize our window we need to adapt our bitmap size
        private void mainWindow_Resize(object sender, EventArgs e)
        {
            if (!(drawArea is null)) drawArea.Dispose();
            if (mainPictureBox.Size.Width == 0 && mainPictureBox.Size.Height == 0) return;
            drawArea = new Bitmap(mainPictureBox.Size.Width, mainPictureBox.Size.Height);
            mainPictureBox.Image = drawArea;
            drawOnPictureBox();
        }
        
        private void createDefoultPolygons()
        {
            List<Line> polygonOneLines = new List<Line>();
            List<Line> polygonTwoLines = new List<Line>();
            
            components.Point p1pt1 = new components.Point(50, 50);
            components.Point p1pt2 = new components.Point(50, 200);
            components.Point p1pt3 = new components.Point(200, 200);

            components.Point p2pt1 = new components.Point(400, 400);
            components.Point p2pt2 = new components.Point(400, 250);
            components.Point p2pt3 = new components.Point(250, 250);

            Line parralelLine1 = new Line(p1pt3, p1pt1);
            Line parralelLine2 = new Line(p2pt3, p2pt1);

            Line fixedLengthLine = new Line(p1pt1, p1pt2);

            polygonOneLines.Add(fixedLengthLine);
            polygonOneLines.Add(new Line(p1pt2, p1pt3));
            polygonOneLines.Add(parralelLine1);

            polygonTwoLines.Add(new Line(p2pt1, p2pt2));
            polygonTwoLines.Add(new Line(p2pt2, p2pt3));
            polygonTwoLines.Add(parralelLine2);

            var polygonOne = new Polygon(polygonOneLines);
            var polygonTwo = new Polygon(polygonTwoLines);

            polygons.Add(polygonOne);
            polygons.Add(polygonTwo);

            relations.Add(new angleRelation(parralelLine1, parralelLine2));
            relations.Add(new lengthRelation(fixedLengthLine, 150));
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
            foreach (components.Polygon p in polygons)
            {
                p.Draw(drawArea);
            }


            mainPictureBox.Refresh();
        }

        private void addAPoint(int X, int Y)
        {
            components.Point actPoint = new components.Point(X, Y);
            // If we are building poligon, check if new point is overlapping with already added
            if (buildingPoints.Count() > 0 && !(polygonEditor.components.Polygon.checkOverallping(actPoint, buildingPoints, tmpPoli.startingPoint) is null)) return;

            // Check if we are creating new poligon from scrach
            if (tmpPoli is null)
            {
                // If we are not build a poligon, we shopuld check if we have clocked on an edge
                if (addPointInTheMiddle(X, Y)) return;
                // Check if we are overlapping with existing poligon point
                if (!(polygonEditor.components.Polygon.checkOverallping(actPoint, polygonEditor.components.Polygon.GetPointsFrom(polygons)) is null)) return;
                tmpPoli = new components.Polygon(actPoint);
                buildingPoints.Add(actPoint);
            }
            else
            {
                movingPoint = null;
                if (tmpPoli.addNewPoint(actPoint, polygonEditor.components.Polygon.GetPointsFrom(polygons)))
                {
                    buildingPoints.Add(actPoint);

                    if (tmpPoli.isPoligonComplet())
                    {
                        polygons.Add(tmpPoli);
                        tmpPoli = null;
                        buildingPoints.Clear();
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
            polygonEditor.components.Point closest = polygonEditor.components.Point.findClosest(polygonEditor.components.Polygon.GetPointsFrom(polygons), actPoint);

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
            holdingLine = null;
            holdingPoligon = null;
            if (!(tmpPoli is null)) buildingPoints.Clear();
            tmpPoli = null;
            if (!(activeLine is null)) activeLine.selected = false;
            activeLine = null;

            drawOnPictureBox();
        }
        private void moveAnEdge(int X, int Y)
        {
            components.Point actPoint = new components.Point(X, Y);
            polygonEditor.components.Line closest = polygonEditor.components.Line.findFirstOnLine(polygonEditor.components.Polygon.GetLinesFrom(polygons), actPoint);
            if (!(closest is null))
            {
                holdingLine = closest;

                movingPoint = actPoint;

                drawOnPictureBox();
            }

        }
        // We are moving poligon if we grab it by line
        private void movePoligon(int X, int Y)
        {
            components.Point actPoint = new components.Point(X, Y);
            polygonEditor.components.Line closest = polygonEditor.components.Line.findFirstOnLine(polygonEditor.components.Polygon.GetLinesFrom(polygons), actPoint);
            if (!(closest is null))
            {
                foreach (var p in polygons)
                {
                    if (p.containsLine(closest))
                    {
                        holdingPoligon = p;
                        break;
                    }
                }

                if (holdingPoligon is null) throw new InvalidOperationException("No poligons set");

                movingPoint = actPoint;

                drawOnPictureBox();
            }
        }
        // Removing point
        private void deletePoint(int X, int Y)
        {
            components.Point actPoint = new components.Point(X, Y);
            polygonEditor.components.Point closest = polygonEditor.components.Point.findClosest(polygonEditor.components.Polygon.GetPointsFrom(polygons), actPoint);

            if (!(closest is null))
            {
                foreach(var p in polygons)
                {
                    if (p.containsPoint(closest))
                    {
                        if(p.Count <= 2)
                        {
                            polygons.Remove(p);
                            for (int i = 0; i < relations.Count; i++)
                            {
                                if (relations[i].isThisPointInRelation(closest))
                                {
                                    relations[i].deleteLabel();
                                    relations.Remove(relations[i]);
                                }
                            }
                            return;
                        }
                        else
                        {
                            p.deletePoint(closest);
                            for (int i = 0; i < relations.Count; i++)
                            {
                                if (relations[i].isThisPointInRelation(closest))
                                {
                                    relations[i].deleteLabel();
                                    relations.Remove(relations[i]);
                                }
                            }
                            return;
                        }
                    }
                }
            }
        }
        private bool addPointInTheMiddle(int X, int Y)
        {
            components.Point actPoint = new components.Point(X, Y);
            polygonEditor.components.Line closest = polygonEditor.components.Line.findFirstOnLine(polygonEditor.components.Polygon.GetLinesFrom(polygons), actPoint);
            if (!(closest is null))
            {
                foreach (var p in polygons)
                {
                    if (p.containsLine(closest))
                    {

                        polygonEditor.components.Point tmp = new polygonEditor.components.Point((closest.Pt1.x + closest.Pt2.x) / 2, (closest.Pt1.y + closest.Pt2.y) / 2);

                        p.lines.Add(new polygonEditor.components.Line(tmp, closest.Pt2));
                        closest.setPt2(tmp);

                        break;
                    }
                }

                drawOnPictureBox();
                return true;
            }
            else return false;
        }

        // Adding length relation
        private void addRelationLength(int X, int Y)
        {
            components.Point actPoint = new components.Point(X, Y);
            polygonEditor.components.Line closest = polygonEditor.components.Line.findFirstOnLine(polygonEditor.components.Polygon.GetLinesFrom(polygons), actPoint);
            if (!(closest is null))
            {
                if (!(activeLine is null)) activeLine.selected = false;
                activeLine = null;
                closest.selected = true;
                drawOnPictureBox();
                int ret = polygonEditor.misc.inputDialog.ShowDialog("Fixed length relation", "Please input desired length", closest.displayLength);
                if (ret <= 0)
                {
                    closest.selected = false;
                    return;
                }
                closest.selected = false;
                relations.Add(new polygonEditor.misc.lengthRelation(closest, ret));

                Polygon.FixPoligons(polygons, relations, null);

                drawOnPictureBox();
            }
        }

        // Adding parallel relation
        private void addRelationParallel(int X, int Y)
        {
            components.Point actPoint = new components.Point(X, Y);
            polygonEditor.components.Line closest = polygonEditor.components.Line.findFirstOnLine(polygonEditor.components.Polygon.GetLinesFrom(polygons), actPoint);

            // Check if it is a second sine selected
            if (activeLine is null)
            {
                if (!(closest is null))
                {
                    closest.selected = true;
                    activeLine = closest;
                    drawOnPictureBox();
                }
            }
            else
            {
                if (!(closest is null))
                {
                    if (closest.Pt1 == activeLine.Pt1 || closest.Pt1 == activeLine.Pt2 || closest.Pt2 == activeLine.Pt1 || closest.Pt2 == activeLine.Pt2) return;
                    closest.selected = true;
                    drawOnPictureBox();

                    // Add paralell relation
                    relations.Add(new angleRelation(activeLine, closest));

                    Polygon.FixPoligons(polygons, relations, null);

                    activeLine.selected = closest.selected = false;
                    activeLine = null;
                    drawOnPictureBox();
                }
            }
        }
        // Deleting relations on line
        private void deleteRelations(int X, int Y)
        {
            components.Point actPoint = new components.Point(X, Y);
            polygonEditor.components.Line closest = polygonEditor.components.Line.findFirstOnLine(polygonEditor.components.Polygon.GetLinesFrom(polygons), actPoint);
            if (!(closest is null))
            { 
                Predicate<IRelation> constainsClosest = r => r.isThisLineInRelation(closest);
                foreach (var rel in relations.FindAll(constainsClosest)) rel.deleteLabel();
                
                relations.RemoveAll(constainsClosest);

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
                        case misc.enums.mode.movingPoligon:
                            {
                                movePoligon(e.X, e.Y);
                            }
                            break;
                        case misc.enums.mode.deletingPoint:
                            {
                                deletePoint(e.X, e.Y);
                            }
                            break;
                        case misc.enums.mode.addingRelationLength:
                            {
                                addRelationLength(e.X, e.Y);
                            }
                            break;
                        case misc.enums.mode.addingRelationParallel:
                            {
                                addRelationParallel(e.X, e.Y);
                            }
                            break;
                        case misc.enums.mode.deletingRelations:
                            {
                                deleteRelations(e.X, e.Y);
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
            polygonEditor.components.Point tmp = movingPoint;
            movingPoint = new components.Point(e.X, e.Y);

            if (!(holdingPoint is null))
            {
                holdingPoint.movePoint(movingPoint);
                Polygon.FixPoligons(polygons, relations, holdingPoint);
            }
            else if (!(holdingLine is null) && !(tmp is null))
            {
                holdingLine.moveLine(tmp, movingPoint);
                Polygon.FixPoligons(polygons, relations, holdingPoint);
            }
            else if (!(holdingPoligon is null) && !(tmp is null))
            {
                holdingPoligon.movePoligon(tmp, movingPoint);
            }

            drawOnPictureBox();

            mainPictureBox.Refresh();
        }

        private void endOfClickOnPictureBox(object sender, MouseEventArgs e)
        {
            holdingPoint = null;
            holdingLine = null;
            holdingPoligon = null;
            Polygon.FixPoligons(polygons, relations, null);
        }

        private void resetContextMenu()
        {
            foreach(ToolStripMenuItem menuItem in contextMenuStrip.Items)
            {
                menuItem.Checked = false;
            }
            foreach(ToolStripMenuItem menuItem in addARelationMenuItem.DropDownItems)
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

        private void moveAPoligonMenuItem_Click(object sender, EventArgs e)
        {
            resetContextMenu();
            moveAPoligonMenuItem.Checked = true;
            activeMode = misc.enums.mode.movingPoligon;
        }

        private void deleteAPointMenuItem_Click(object sender, EventArgs e)
        {
            resetContextMenu();
            deleteAPointMenuItem.Checked = true;
            activeMode = misc.enums.mode.deletingPoint;
        }

        private void defaultMethodOfDrawingMenuItem_Click(object sender, EventArgs e)
        {
            bresenhamsAlgorithmMethodOfDrawingMenuItem.Checked = false;
            defaultMethodOfDrawingMenuItem.Checked = true;
            polygonEditor.components.Line.useBresenhams = false;
            drawOnPictureBox();
        }

        private void bresenhamsAlgorithmMethodOfDrawingMenuItem_Click(object sender, EventArgs e)
        {
            defaultMethodOfDrawingMenuItem.Checked = false;
            bresenhamsAlgorithmMethodOfDrawingMenuItem.Checked = true;
            polygonEditor.components.Line.useBresenhams = true;
            drawOnPictureBox();
        }

        private void addFixedLengthMenuItem_Click(object sender, EventArgs e)
        {
            resetContextMenu();
            addARelationMenuItem.Checked = true;
            addFixedLengthMenuItem.Checked = true;
            activeMode = misc.enums.mode.addingRelationLength;
        }

        private void selectParallelLinesMenuItem_Click(object sender, EventArgs e)
        {
            resetContextMenu();
            addARelationMenuItem.Checked = true;
            selectParallelLinesMenuItem.Checked = true;
            activeMode = misc.enums.mode.addingRelationParallel;
        }

        private void deleteRelationsMenuItem_Click(object sender, EventArgs e)
        {
            resetContextMenu();
            deleteRelationsMenuItem.Checked = true;
            activeMode = misc.enums.mode.deletingRelations;
        }
    }
}
