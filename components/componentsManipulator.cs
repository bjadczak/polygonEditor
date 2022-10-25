using polygonEditor.misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace polygonEditor.components
{
    // This class contains methods that will affect displayed components
    internal static class componentsManipulator
    {
        public static void addAPoint(int X, int Y, misc.context ctx)
        {
            components.Point actPoint = new components.Point(X, Y);
            // If we are building poligon, check if new point is overlapping with already added
            if (ctx.buildingPoints.Count() > 0 && !(polygonEditor.components.Polygon.checkOverallping(actPoint, ctx.buildingPoints, ctx.tmpPoly.startingPoint) is null)) return;

            // Check if we are creating new poligon from scrach
            if (ctx.tmpPoly is null)
            {
                // If we are not build a poligon, we shopuld check if we have clocked on an edge
                if (addPointInTheMiddle(X, Y, ctx)) return;
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
                        ctx.drawAllObjects();
                    }
                    else
                    {
                        ctx.tmpPoly.Draw(ctx.drawArea);
                    }
                }
            }

        }
        private static bool addPointInTheMiddle(int X, int Y, misc.context ctx)
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

                ctx.drawAllObjects();
                return true;
            }
            else return false;
        }
        
        public static void moveAPoint(int X, int Y, misc.context ctx)
        {
            components.Point actPoint = new components.Point(X, Y);
            // Moving a point we check which point is selected
            polygonEditor.components.Point closest = polygonEditor.components.Point.findClosest(polygonEditor.components.Polygon.GetPointsFrom(ctx.polygons), actPoint);

            if (!(closest is null))
            {
                ctx.holdingPoint = closest;

                ctx.movingPoint = actPoint;

                ctx.drawAllObjects();
            }
        }

        public static void moveAnEdge(int X, int Y, misc.context ctx)
        {
            components.Point actPoint = new components.Point(X, Y);
            polygonEditor.components.Line closest = polygonEditor.components.Line.findFirstOnLine(polygonEditor.components.Polygon.GetLinesFrom(ctx.polygons), actPoint);
            if (!(closest is null))
            {
                ctx.holdingLine = closest;

                ctx.movingPoint = actPoint;

                ctx.drawAllObjects();
            }
        }

        public static void moveAPolygon(int X, int Y, misc.context ctx)
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

                ctx.drawAllObjects();
            }
        }
        
        public static void deleteAPoint(int X, int Y, misc.context ctx)
        {
            components.Point actPoint = new components.Point(X, Y);
            polygonEditor.components.Point closest = polygonEditor.components.Point.findClosest(polygonEditor.components.Polygon.GetPointsFrom(ctx.polygons), actPoint);

            if (!(closest is null))
            {
                foreach (var p in ctx.polygons)
                {
                    if (p.containsPoint(closest))
                    {
                        if (p.Count <= 2)
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

        public static void addLengthRelation(int X, int Y, misc.context ctx, PictureBox mainPictureBox)
        {
            components.Point actPoint = new components.Point(X, Y);
            polygonEditor.components.Line closest = polygonEditor.components.Line.findFirstOnLine(polygonEditor.components.Polygon.GetLinesFrom(ctx.polygons), actPoint);
            if (!(closest is null))
            {
                if (!(ctx.activeLine is null)) ctx.activeLine.selected = false;
                ctx.activeLine = null;
                closest.selected = true;
                ctx.drawAllObjects();
                mainPictureBox.Refresh();
                int ret = polygonEditor.misc.inputDialog.ShowDialog("Fixed length relation", "Please input desired length", closest.displayLength);
                if (ret <= 0)
                {
                    closest.selected = false;
                    return;
                }
                closest.selected = false;
                ctx.relations.Add(new polygonEditor.misc.lengthRelation(closest, ret));

                Polygon.FixPoligons(ctx.polygons, ctx.relations, null);

                ctx.drawAllObjects();
                mainPictureBox.Refresh();
            }
        }

        public static void addParallelRelation(int X, int Y, misc.context ctx, PictureBox mainPictureBox)
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
                    ctx.drawAllObjects();
                    mainPictureBox.Refresh();
                }
            }
            else
            {
                if (!(closest is null))
                {
                    if (closest.Pt1 == ctx.activeLine.Pt1 || closest.Pt1 == ctx.activeLine.Pt2 || closest.Pt2 == ctx.activeLine.Pt1 || closest.Pt2 == ctx.activeLine.Pt2) return;
                    closest.selected = true;
                    ctx.drawAllObjects();
                    mainPictureBox.Refresh();

                    // Add paralell relation
                    ctx.relations.Add(new angleRelation(ctx.activeLine, closest));

                    Polygon.FixPoligons(ctx.polygons, ctx.relations, null);

                    ctx.activeLine.selected = closest.selected = false;
                    ctx.activeLine = null;
                    ctx.drawAllObjects();
                    mainPictureBox.Refresh();
                }
            }
        }

        public static void deleteARelation(int X, int Y, misc.context ctx)
        {
            components.Point actPoint = new components.Point(X, Y);
            polygonEditor.components.Line closest = polygonEditor.components.Line.findFirstOnLine(polygonEditor.components.Polygon.GetLinesFrom(ctx.polygons), actPoint);
            if (!(closest is null))
            {
                Predicate<IRelation> constainsClosest = r => r.isThisLineInRelation(closest);
                foreach (var rel in ctx.relations.FindAll(constainsClosest)) rel.deleteLabel();

                ctx.relations.RemoveAll(constainsClosest);

                ctx.drawAllObjects();
            }
        }

        public static void moveActivePoints(int X, int Y, misc.context ctx)
        {
            polygonEditor.components.Point tmp = ctx.movingPoint;
            ctx.movingPoint = new components.Point(X, Y);

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
            else if(!(tmp is null) && !(ctx.tmpCircle is null))
            {
                ctx.tmpCircle.MoveRadius(tmp);
            }
            if (!(tmp is null) && !(ctx.holdingCircle is null))
            {
                ctx.holdingCircle.moveCircle(ctx.movingPoint.x - tmp.x, ctx.movingPoint.y - tmp.y);
            }
        }

        public static void addCircle(int X, int Y, misc.context ctx)
        {
            components.Point actPoint = new components.Point(X, Y);

            if (ctx.tmpCircle is null)
            {
                components.Circle c = new Circle(actPoint);
                ctx.tmpCircle = c;
            }
            else
            {
                ctx.tmpCircle.finishMoving(actPoint);
                ctx.circles.Add(ctx.tmpCircle);
                ctx.tmpCircle = null;
            }


            ctx.drawAllObjects();

        }

        public static void moveCircle(int X, int Y, misc.context ctx)
        {
            components.Point actPoint = new components.Point(X, Y);
            polygonEditor.components.Circle closest = polygonEditor.components.Circle.findFirstOnLine(ctx.circles, actPoint);
            if (!(closest is null))
            {

                ctx.holdingCircle = closest;

                ctx.movingPoint = actPoint;

                ctx.drawAllObjects();
            }
        }
        public static void resizeCircle(int X, int Y, misc.context ctx)
        {
            components.Point actPoint = new components.Point(X, Y);
            polygonEditor.components.Circle closest = polygonEditor.components.Circle.findFirstOnLine(ctx.circles, actPoint);
            if (!(closest is null))
            {

                ctx.circles.Remove(closest);

                closest.enterResizing();

                ctx.tmpCircle = closest;

                ctx.movingPoint = actPoint;

                ctx.drawAllObjects();
            }
            else if(!(ctx.tmpCircle is null))
            {
                ctx.tmpCircle.finishMoving(actPoint);
                ctx.circles.Add(ctx.tmpCircle);
                ctx.tmpCircle = null;
            }
        }
        public static void removeCircle(int X, int Y, misc.context ctx)
        {
            components.Point actPoint = new components.Point(X, Y);
            polygonEditor.components.Circle closest = polygonEditor.components.Circle.findFirstOnLine(ctx.circles, actPoint);
            if (!(closest is null))
            {

                ctx.circles.Remove(closest);

                ctx.drawAllObjects();
            }
        }
    }

}
