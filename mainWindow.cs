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

        misc.context ctx;


        public mainWindow()
        {
            InitializeComponent();

            ctx = new misc.context(mainPictureBox.Size.Width, mainPictureBox.Size.Height);

            mainPictureBox.Image = ctx.drawArea;

            drawOnPictureBox();
        }

        // When we resize our window we need to adapt our bitmap size
        private void mainWindow_Resize(object sender, EventArgs e)
        {
            if (!(ctx.drawArea is null)) ctx.drawArea.Dispose();
            if (mainPictureBox.Size.Width == 0 && mainPictureBox.Size.Height == 0) return;
            ctx.drawArea = new Bitmap(mainPictureBox.Size.Width, mainPictureBox.Size.Height);
            mainPictureBox.Image = ctx.drawArea;
            drawOnPictureBox();
        }
        
        

        // Method that calls to draw specific vertixces and lines
        private void drawOnPictureBox()
        {

            ctx.drawAllObjects();

            mainPictureBox.Refresh();
        }

        private void addAPoint(int X, int Y)
        {
            components.Point actPoint = new components.Point(X, Y);
            // If we are building poligon, check if new point is overlapping with already added
            if (ctx.buildingPoints.Count() > 0 && !(polygonEditor.components.Polygon.checkOverallping(actPoint, ctx.buildingPoints, ctx.tmpPoly.startingPoint) is null)) return;

            // Check if we are creating new poligon from scrach
            if (ctx.tmpPoly is null)
            {
                // If we are not build a poligon, we shopuld check if we have clocked on an edge
                if (addPointInTheMiddle(X, Y)) return;
                // Check if we are overlapping with existing poligon point
                if (!(polygonEditor.components.Polygon.checkOverallping(actPoint, polygonEditor.components.Polygon.GetPointsFrom(ctx.polygons)) is null)) return;
                ctx.tmpPoly = new components.Polygon(actPoint);
                ctx.buildingPoints.Add(actPoint);
            }
            else
            {
                ctx.movingPoint = null;
                if (ctx.tmpPoly.addNewPoint(actPoint, polygonEditor.components.Polygon.GetPointsFrom(ctx.polygons)))
                {
                    ctx.buildingPoints.Add(actPoint);

                    if (ctx.tmpPoly.isPoligonComplet())
                    {
                        ctx.polygons.Add(ctx.tmpPoly);
                        ctx.tmpPoly = null;
                        ctx.buildingPoints.Clear();
                        drawOnPictureBox();
                    }
                    else
                    {
                        ctx.tmpPoly.Draw(ctx.drawArea);
                        mainPictureBox.Refresh();
                    }
                }
            }
        }
        private void moveAPoint(int X, int Y)
        {
            components.Point actPoint = new components.Point(X, Y);
            // Moving a point we check which point is selected
            polygonEditor.components.Point closest = polygonEditor.components.Point.findClosest(polygonEditor.components.Polygon.GetPointsFrom(ctx.polygons), actPoint);

            if (!(closest is null))
            {
                ctx.holdingPoint = closest;

                ctx.movingPoint = actPoint;

                drawOnPictureBox();
            }
        }
        private void resetOnRMB()
        {
            ctx.resetAllActivity();

            drawOnPictureBox();
        }
        private void moveAnEdge(int X, int Y)
        {
            components.Point actPoint = new components.Point(X, Y);
            polygonEditor.components.Line closest = polygonEditor.components.Line.findFirstOnLine(polygonEditor.components.Polygon.GetLinesFrom(ctx.polygons), actPoint);
            if (!(closest is null))
            {
                ctx.holdingLine = closest;

                ctx.movingPoint = actPoint;

                drawOnPictureBox();
            }

        }
        // We are moving poligon if we grab it by line
        private void movePoligon(int X, int Y)
        {
            components.Point actPoint = new components.Point(X, Y);
            polygonEditor.components.Line closest = polygonEditor.components.Line.findFirstOnLine(polygonEditor.components.Polygon.GetLinesFrom(ctx.polygons), actPoint);
            if (!(closest is null))
            {
                foreach (var p in ctx.polygons)
                {
                    if (p.containsLine(closest))
                    {
                        ctx.holdingPolygon = p;
                        break;
                    }
                }

                if (ctx.holdingPolygon is null) throw new InvalidOperationException("No poligons set");

                ctx.movingPoint = actPoint;

                drawOnPictureBox();
            }
        }
        // Removing point
        private void deletePoint(int X, int Y)
        {
            components.Point actPoint = new components.Point(X, Y);
            polygonEditor.components.Point closest = polygonEditor.components.Point.findClosest(polygonEditor.components.Polygon.GetPointsFrom(ctx.polygons), actPoint);

            if (!(closest is null))
            {
                foreach(var p in ctx.polygons)
                {
                    if (p.containsPoint(closest))
                    {
                        if(p.Count <= 2)
                        {
                            ctx.polygons.Remove(p);
                            for (int i = 0; i < ctx.relations.Count; i++)
                            {
                                if (ctx.relations[i].isThisPointInRelation(closest))
                                {
                                    ctx.relations[i].deleteLabel();
                                    ctx.relations.Remove(ctx.relations[i]);
                                }
                            }
                            return;
                        }
                        else
                        {
                            p.deletePoint(closest);
                            for (int i = 0; i < ctx.relations.Count; i++)
                            {
                                if (ctx.relations[i].isThisPointInRelation(closest))
                                {
                                    ctx.relations[i].deleteLabel();
                                    ctx.relations.Remove(ctx.relations[i]);
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
            polygonEditor.components.Line closest = polygonEditor.components.Line.findFirstOnLine(polygonEditor.components.Polygon.GetLinesFrom(ctx.polygons), actPoint);
            if (!(closest is null))
            {
                foreach (var p in ctx.polygons)
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
            polygonEditor.components.Line closest = polygonEditor.components.Line.findFirstOnLine(polygonEditor.components.Polygon.GetLinesFrom(ctx.polygons), actPoint);
            if (!(closest is null))
            {
                if (!(ctx.activeLine is null)) ctx.activeLine.selected = false;
                ctx.activeLine = null;
                closest.selected = true;
                drawOnPictureBox();
                int ret = polygonEditor.misc.inputDialog.ShowDialog("Fixed length relation", "Please input desired length", closest.displayLength);
                if (ret <= 0)
                {
                    closest.selected = false;
                    return;
                }
                closest.selected = false;
                ctx.relations.Add(new polygonEditor.misc.lengthRelation(closest, ret));

                Polygon.FixPoligons(ctx.polygons, ctx.relations, null);

                drawOnPictureBox();
            }
        }

        // Adding parallel relation
        private void addRelationParallel(int X, int Y)
        {
            components.Point actPoint = new components.Point(X, Y);
            polygonEditor.components.Line closest = polygonEditor.components.Line.findFirstOnLine(polygonEditor.components.Polygon.GetLinesFrom(ctx.polygons), actPoint);

            // Check if it is a second sine selected
            if (ctx.activeLine is null)
            {
                if (!(closest is null))
                {
                    closest.selected = true;
                    ctx.activeLine = closest;
                    drawOnPictureBox();
                }
            }
            else
            {
                if (!(closest is null))
                {
                    if (closest.Pt1 == ctx.activeLine.Pt1 || closest.Pt1 == ctx.activeLine.Pt2 || closest.Pt2 == ctx.activeLine.Pt1 || closest.Pt2 == ctx.activeLine.Pt2) return;
                    closest.selected = true;
                    drawOnPictureBox();

                    // Add paralell relation
                    ctx.relations.Add(new angleRelation(ctx.activeLine, closest));

                    Polygon.FixPoligons(ctx.polygons, ctx.relations, null);

                    ctx.activeLine.selected = closest.selected = false;
                    ctx.activeLine = null;
                    drawOnPictureBox();
                }
            }
        }
        // Deleting relations on line
        private void deleteRelations(int X, int Y)
        {
            components.Point actPoint = new components.Point(X, Y);
            polygonEditor.components.Line closest = polygonEditor.components.Line.findFirstOnLine(polygonEditor.components.Polygon.GetLinesFrom(ctx.polygons), actPoint);
            if (!(closest is null))
            { 
                Predicate<IRelation> constainsClosest = r => r.isThisLineInRelation(closest);
                foreach (var rel in ctx.relations.FindAll(constainsClosest)) rel.deleteLabel();
                
                ctx.relations.RemoveAll(constainsClosest);

                drawOnPictureBox();
            }
        }

        private void clickOnPictureBox(object sender, MouseEventArgs e)
        {
            // Deside which button was pressed
            switch (e.Button) { 
                case MouseButtons.Left:
                    // Chceck what mode is active
                    switch (ctx.activeMode)
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
            polygonEditor.components.Point tmp = ctx.movingPoint;
            ctx.movingPoint = new components.Point(e.X, e.Y);

            if (!(ctx.holdingPoint is null))
            {
                ctx.holdingPoint.movePoint(ctx.movingPoint);
                Polygon.FixPoligons(ctx.polygons, ctx.relations, ctx.holdingPoint);
            }
            else if (!(ctx.holdingLine is null) && !(tmp is null))
            {
                ctx.holdingLine.moveLine(tmp, ctx.movingPoint);
                Polygon.FixPoligons(ctx.polygons, ctx.relations, ctx.holdingPoint);
            }
            else if (!(ctx.holdingPolygon is null) && !(tmp is null))
            {
                ctx.holdingPolygon.movePoligon(tmp, ctx.movingPoint);
            }

            drawOnPictureBox();

            mainPictureBox.Refresh();
        }

        private void endOfClickOnPictureBox(object sender, MouseEventArgs e)
        {
            ctx.resetAllHolds();
            Polygon.FixPoligons(ctx.polygons, ctx.relations, null);
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
            ctx.activeMode = misc.enums.mode.addingPoint;
        }

        private void movaAPointMenuItem_Click(object sender, EventArgs e)
        {
            resetContextMenu();
            movaAPointMenuItem.Checked = true;
            ctx.activeMode = misc.enums.mode.movingPoint;
        }

        private void moveAnEdgeMenuItem_Click(object sender, EventArgs e)
        {
            resetContextMenu();
            moveAnEdgeMenuItem.Checked = true;
            ctx.activeMode = misc.enums.mode.movingEdge;
        }

        private void moveAPoligonMenuItem_Click(object sender, EventArgs e)
        {
            resetContextMenu();
            moveAPoligonMenuItem.Checked = true;
            ctx.activeMode = misc.enums.mode.movingPoligon;
        }

        private void deleteAPointMenuItem_Click(object sender, EventArgs e)
        {
            resetContextMenu();
            deleteAPointMenuItem.Checked = true;
            ctx.activeMode = misc.enums.mode.deletingPoint;
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
            ctx.activeMode = misc.enums.mode.addingRelationLength;
        }

        private void selectParallelLinesMenuItem_Click(object sender, EventArgs e)
        {
            resetContextMenu();
            addARelationMenuItem.Checked = true;
            selectParallelLinesMenuItem.Checked = true;
            ctx.activeMode = misc.enums.mode.addingRelationParallel;
        }

        private void deleteRelationsMenuItem_Click(object sender, EventArgs e)
        {
            resetContextMenu();
            deleteRelationsMenuItem.Checked = true;
            ctx.activeMode = misc.enums.mode.deletingRelations;
        }
    }
}
