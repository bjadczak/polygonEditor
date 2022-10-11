﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace poligonEditor.components
{
    internal class Poligon : IDisposable
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
        public Poligon()
        {
            lines = new List<components.Line>();
            InstanceID = Guid.NewGuid();
            startingPoint = null;
            finishingPoint = null;
        }
        public Poligon(components.Point startingPoint)
        {
            lines = new List<components.Line>();
            InstanceID = Guid.NewGuid();
            finishingPoint = this.startingPoint = startingPoint;
        }

        public bool isPoligonComplet() => !(startingPoint is null) && !(finishingPoint is null) && lines.Count > 0 && startingPoint.InstanceID == finishingPoint.InstanceID;

        // We build our poligon by those two methods, adding points on the way
        public void addFirstPoint(components.Point pt)
        {
            if (isPoligonComplet()) throw new InvalidOperationException("Poligon already complet");
            if ((startingPoint is null)) throw new InvalidOperationException("Poligon already has starting point");

            startingPoint = finishingPoint = pt;
        }

        public bool addNewPoint(components.Point pt, IEnumerable<components.Point> allPoints)
        {
            if (isPoligonComplet()) throw new InvalidOperationException("Poligon already complet");

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
        public IEnumerable<poligonEditor.components.Line> GetLines()
        {
            foreach (components.Line l in lines)
            {
                yield return l;
            }
        }
        public static IEnumerable<poligonEditor.components.Line> GetLinesFrom(IEnumerable<poligonEditor.components.Poligon> poligons)
        {
            foreach(components.Poligon p in poligons)
            {
                foreach(poligonEditor.components.Line l in p.GetLines())
                {
                    yield return l;
                }
            }
        }
        public IEnumerable<poligonEditor.components.Point> GetPoints()
        {
            foreach (components.Line l in lines)
            {
                yield return l.Pt1;
            }
        }
        public static IEnumerable<poligonEditor.components.Point> GetPointsFrom(IEnumerable<poligonEditor.components.Poligon> poligons)
        {
            foreach (components.Poligon p in poligons)
            {
                foreach (poligonEditor.components.Point pt in p.GetPoints())
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

        public IEnumerable<poligonEditor.components.Line> getLinesWithPoint(components.Point point)
        {
            foreach (var l in lines)
            {
                if (l.Pt1 == point) yield return l;
                if (l.Pt2 == point) yield return l;
            }
        }

        public void movePoligon(poligonEditor.components.Point firstPoint, poligonEditor.components.Point secondPoint)
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

        public IEnumerable<poligonEditor.components.Line> GetLinesFromLine(poligonEditor.components.Line l)
        {
            bool[] visited = new bool[lines.Count];
            for (int i = 0; i < visited.Length; i++) visited[i] = false;
            //int count = visited.Length;

            bool isVisited(Line lookedAt)
            {
                bool isLine(Line lin)
                {
                    return lin == lookedAt;
                }

                int idx = lines.FindIndex(isLine);

                return visited[idx];

            }
            void setVisited(Line lookedAt)
            {
                bool isLine(Line lin)
                {
                    return lin == lookedAt;
                }

                int idx = lines.FindIndex(isLine);

                visited[idx] = true;

            }



            Stack<poligonEditor.components.Line> S = new Stack<Line>();

            S.Push(l);
            setVisited(l);

            while (S.Count > 0)
            {
                poligonEditor.components.Line tmp = S.Pop();
                yield return tmp;


                
                

                foreach(var lines in this.getLinesWithPoint(tmp.Pt1))
                {
                    if (!isVisited(lines))
                    {
                        setVisited(lines);
                        S.Push(lines);
                    }
                }
                foreach (var lines in this.getLinesWithPoint(tmp.Pt2))
                {
                    if (!isVisited(lines))
                    {
                        setVisited(lines);
                        S.Push(lines);
                    }
                }


            }

            

        }

        public void fixPoligon(components.Point movingPoint, IEnumerable<poligonEditor.misc.IRelation> relations)
        {
            // Find all relations that we care of in this poligon
            List<poligonEditor.misc.IRelation> poligonRelations = new List<poligonEditor.misc.IRelation>();
            foreach(var line in lines)
            {
                foreach (var rel in relations)
                {
                    if (rel.isThisLineInRelation(line) && !poligonRelations.Contains(rel)) poligonRelations.Add(rel);
                }
            }
            //if (!this.containsPoint(movingPoint)) movingPoint = lines[0].Pt1;
            Line activeLine = this.getLinesWithPoint(movingPoint).First();
            // We fix all lines that have relations
            const float threshold = 20.0f;
            float score = 0;
            foreach (var rel in poligonRelations) score += rel.Score();
            float prevScore = float.MaxValue;
            var startPoint = movingPoint;
            while (score > threshold && prevScore > score)
            {
                prevScore = score;
                score = 0;
                //movingPoint = startPoint;
                foreach(var rel in poligonRelations)
                {
                    foreach(var line in GetLinesFromLine(activeLine))
                    {
                        if (rel.isThisLineInRelation(line))
                        {
                            rel.Fix(line, line.Pt2 == movingPoint ? line.Pt1 : line.Pt2, poligonRelations);
                            break;
                        }
                        
                    }
                }
                foreach (var rel in poligonRelations) score += rel.Score();
                
                // We do this until we get better score that threashold or until we are making it worst
            }


        }

    }

}
