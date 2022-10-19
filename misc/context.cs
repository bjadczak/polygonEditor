using polygonEditor.components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polygonEditor.misc
{
    internal class context
    {
        // Actual object we are going draw on, like a "canvas"
        public Bitmap drawArea;

        // List of poligons we have on the screen
        public List<components.Polygon> polygons = new List<components.Polygon>();

        // Listo of references to points of polygons that is being build
        public List<components.Point> buildingPoints = new List<components.Point>();

        public components.Polygon tmpPoly = null;

        // Moste recent point where we saw mouse
        public components.Point movingPoint = null;

        // Point of poligon we are moving
        public components.Point holdingPoint = null;
        // Line of poligon we are moving
        public components.Line holdingLine = null;
        // Polygon we are moving
        public components.Polygon holdingPolygon = null;

        // Active mode of input
        public misc.enums.mode activeMode = misc.enums.mode.addingPoint;

        // List of all active relations
        public List<misc.IRelation> relations = new List<misc.IRelation>();

        // Active line
        public components.Line activeLine = null;

        public context(int width, int height)
        {
            // We create and attache our "canvas" to pictureBox
            drawArea = new Bitmap(width, height);
            

            createDefoultPolygons();
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

        public void drawAllObjects()
        {
            // Fill bitmap with white background
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.Clear(Color.White);
            }

            // Drawing poligon that is being build
            if (!(tmpPoly is null))
            {
                tmpPoly.Draw(drawArea);
                tmpPoly.DrawIncompleteLine(drawArea, movingPoint);
            }

            // Draw all poligons taht we have stored
            foreach (components.Polygon p in polygons)
            {
                p.Draw(drawArea);
            }
        }

        public void resetAllActivity()
        {
            holdingPoint = null;
            holdingLine = null;
            holdingPolygon = null;
            if (!(tmpPoly is null)) buildingPoints.Clear();
            tmpPoly = null;
            if (!(activeLine is null)) activeLine.selected = false;
            activeLine = null;
        }

        public void resetAllHolds()
        {
            holdingPoint = null;
            holdingLine = null;
            holdingPolygon = null;
        }


    }
}
