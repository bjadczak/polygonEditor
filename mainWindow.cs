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
            ctx.handleResize(mainPictureBox);
        }
        
        

        // Method that calls to draw specific vertixces and lines
        private void drawOnPictureBox()
        {

            ctx.drawAllObjects();

            mainPictureBox.Refresh();
        }

        private void addAPoint(int X, int Y)
        {
            polygonEditor.components.componentsManipulator.addAPoint(X, Y, ctx);

            mainPictureBox.Refresh();

        }
        private void moveAPoint(int X, int Y)
        {
            polygonEditor.components.componentsManipulator.moveAPoint(X, Y, ctx);

            mainPictureBox.Refresh();
        }
        private void resetOnRMB()
        {
            ctx.resetAllActivity();

            drawOnPictureBox();
        }
        private void moveAnEdge(int X, int Y)
        {

            polygonEditor.components.componentsManipulator.moveAnEdge(X, Y, ctx);

            mainPictureBox.Refresh();
        }
        // We are moving poligon if we grab it by line
        private void movePolygon(int X, int Y)
        {
            polygonEditor.components.componentsManipulator.moveAPolygon(X, Y, ctx);

            mainPictureBox.Refresh();
        }
        // Removing point
        private void deletePoint(int X, int Y)
        {
            polygonEditor.components.componentsManipulator.deleteAPoint(X, Y, ctx);

            drawOnPictureBox();
        }


        // Adding length relation
        private void addRelationLength(int X, int Y)
        {
            polygonEditor.components.componentsManipulator.addLengthRelation(X, Y, ctx, mainPictureBox);
        }

        // Adding parallel relation
        private void addRelationParallel(int X, int Y)
        {
            polygonEditor.components.componentsManipulator.addParallelRelation(X, Y, ctx, mainPictureBox);

        }
        // Deleting relations on line
        private void deleteRelations(int X, int Y)
        {
            polygonEditor.components.componentsManipulator.deleteARelation(X, Y, ctx);

            mainPictureBox.Refresh();
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
                                movePolygon(e.X, e.Y);
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

            polygonEditor.components.componentsManipulator.moveActivePoints(e.X, e.Y, ctx);
            
            drawOnPictureBox();

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
