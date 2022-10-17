using polygonEditor.misc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace polygonEditor.components
{
    internal class Polygon : IDisposable
    {
        // Implementing Global Unique ID might be helpful in the future
        public Guid InstanceID { get; private set; }

        public List<components.Line> lines;

        public components.Point startingPoint { get; private set; }
        private components.Point finishingPoint;

        // Create pen for temporary line.
        Pen grayPen = new Pen(Color.Gray, components.Line.width);

        // Count of lines
        public int Count { get => lines.Count; }

        // Constructor for empty poligon and one with only one point (startingPoint)
        public Polygon()
        {
            lines = new List<components.Line>();
            InstanceID = Guid.NewGuid();
            startingPoint = null;
            finishingPoint = null;
        }
        // Constructor for creating default polygons on grid
        public Polygon(List<Line> lines)
        {
            this.lines = new List<components.Line>();
            InstanceID = Guid.NewGuid();
            startingPoint = null;
            finishingPoint = null;
            if (lines.Count < 2) throw new InvalidOperationException("Not enough lines in polygon constructor");
            startingPoint = finishingPoint = lines[0].Pt1;
            foreach(var line in lines)
            {
                this.lines.Add(line);
            }
        }
        public Polygon(components.Point startingPoint)
        {
            lines = new List<components.Line>();
            InstanceID = Guid.NewGuid();
            finishingPoint = this.startingPoint = startingPoint;
        }

        public bool isPoligonComplet() => !(startingPoint is null) && !(finishingPoint is null) && lines.Count > 0 && startingPoint.InstanceID == finishingPoint.InstanceID;

        // We build our poligon by those two methods, adding points on the way
        public void addFirstPoint(components.Point pt)
        {
            if (isPoligonComplet()) throw new InvalidOperationException("Polygon already complet");
            if ((startingPoint is null)) throw new InvalidOperationException("Polygon already has starting point");

            startingPoint = finishingPoint = pt;
        }

        public bool addNewPoint(components.Point pt, IEnumerable<components.Point> allPoints)
        {
            if (isPoligonComplet()) throw new InvalidOperationException("Polygon already complet");

            using (var tmp = checkOverallping(pt, allPoints))
            {
                if (!(tmp is null))
                    return false;
            }
            using (var tmp = checkOverallping(pt, this.GetPoints()))
            {
                if (!(tmp is null))
                {
                    if (tmp.InstanceID == startingPoint.InstanceID)
                    {
                        pt = startingPoint;
                    }
                    else return false;
                }
            }

            lines.Add(new components.Line(finishingPoint, pt));
            finishingPoint = pt;
            return true;

        }

        public static components.Point checkOverallping(components.Point pt, IEnumerable<components.Point> allPoints, components.Point exceptThis = null)
        {
            if (allPoints.Count() > 0)
            {
                foreach (components.Point p in allPoints)
                {
                    if (p.isOverlapping(pt))
                        if (!(exceptThis is null) && exceptThis == p) return null;
                        else return p;
                }
            }
            return null;
        }

        public void Draw(Bitmap drawArea)
        {
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                foreach (components.Line l in lines)
                {
                    l.Draw(drawArea);
                }
            }
        }

        public void Dispose()
        {
            foreach(components.Line l in lines) l.Dispose();
        }

        public void DrawIncompleteLine(Bitmap drawArea, components.Point pt)
        {
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.DrawLine(grayPen, (System.Drawing.Point)finishingPoint, (System.Drawing.Point)pt);
            }
        }
        public IEnumerable<polygonEditor.components.Line> GetLines()
        {
            foreach (components.Line l in lines)
            {
                yield return l;
            }
        }
        public static IEnumerable<polygonEditor.components.Line> GetLinesFrom(IEnumerable<polygonEditor.components.Polygon> polygons)
        {
            foreach(components.Polygon p in polygons)
            {
                foreach(polygonEditor.components.Line l in p.GetLines())
                {
                    yield return l;
                }
            }
        }
        public IEnumerable<polygonEditor.components.Point> GetPoints()
        {
            foreach (components.Line l in lines)
            {
                yield return l.Pt1;
            }
        }
        public static IEnumerable<polygonEditor.components.Point> GetPointsFrom(IEnumerable<polygonEditor.components.Polygon> polygons)
        {
            foreach (components.Polygon p in polygons)
            {
                foreach (polygonEditor.components.Point pt in p.GetPoints())
                {
                    yield return pt;
                }
            }
        }
        public bool containsLine(components.Line line) => lines.Contains(line);
        public bool containsPoint(components.Point point)
        {
            foreach(var l in lines)
            {
                if (l.Pt1 == point) return true; 
            }
            return false;
        }

        public void movePoligon(polygonEditor.components.Point firstPoint, polygonEditor.components.Point secondPoint)
        {
            foreach(var p in GetPoints())
            {
                p.movePointByDelta(secondPoint.x - firstPoint.x, secondPoint.y - firstPoint.y);
            }
        }

        public void deletePoint(components.Point point)
        {
            if (Count <= 2) throw new InvalidOperationException("Too few points in poligon");
            components.Line l1 = null, l2 = null;
            foreach(var l in lines)
            {
                if (l.Pt2 == point) l1 = l;
                if (l.Pt1 == point) l2 = l;
            }
            if (l1 is null || l2 is null) throw new InvalidOperationException("Uknown error");
            if (point == startingPoint) startingPoint = finishingPoint = l1.Pt1;
            l1.setPt2(l2.Pt2);
            lines.Remove(l2);
        }

        // Function that should fix relations in our set of polygons
        public static void FixPoligons(IEnumerable<components.Polygon> polygons, IEnumerable<misc.IRelation> relations, components.Point stationaryPoint)
        {
            const float threshold = 1.0f;
            float score = 0;
            float prevScore = float.MaxValue;

            float getScorePartial(misc.IRelation relation)
            {
                float scorePartial = 0;
                foreach (var rel in relations) if (rel != relation) scorePartial += rel.Score();
                return scorePartial;
            }
            void resetMoved()
            {
                foreach (var rel in relations) rel.setMove(false);
            }

            foreach (var rel in relations) score += rel.Score();

            while (score > threshold && prevScore > score)
            {
                prevScore = score;
                score = 0;

                foreach (var rel in relations)
                {
                    // We try to correct relation once
                    if(!rel.alreadyMoved())
                        foreach (var line in Polygon.GetLinesFrom(polygons))
                        {
                            if (rel.isThisLineInRelation(line))
                            {
                                float bestScore = float.MaxValue;
                                float startScore = rel.Score() + getScorePartial(rel);
                                int dx = 0, dy = 0;

                                // Check all possible directions and choose the best
                                foreach(int i in new List<int>() { 0, 1, -1})
                                    foreach (int j in new List<int>() { 0, 1, -1 })
                                    {
                                        var tmp = rel.scoreWithChange(i, j, line, stationaryPoint) + getScorePartial(rel);
                                        if (tmp < bestScore && tmp < startScore)
                                        {
                                            dx = i;
                                            dy = j;
                                            bestScore = tmp;
                                        }
                                    }

                                // Move if nessesary
                                if (bestScore < startScore)
                                {
                                    rel.moveByChange(dx, dy, line, stationaryPoint);
                                    rel.setMove(true);
                                }
                            }

                        }
                }

                resetMoved();


                foreach (var rel in relations) score += rel.Score();

                // We do this until we get better score that threashold or until we are making it worst
            }
        }

    }

}
