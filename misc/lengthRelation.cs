using poligonEditor.components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poligonEditor.misc
{
    internal class lengthRelation : poligonEditor.misc.IRelation, IDisposable
    {
        public poligonEditor.components.Line l { get; private set; }
        public float length { get; private set; }
        public lengthRelation(poligonEditor.components.Line l, float displayLength)
        {
            this.l = l;
            this.length = displayLength * displayLength;
        }
        public float Score()
        {
            return Math.Abs((float)(l.length - this.length));
        }
        public float ScoreForLine(poligonEditor.components.Line l)
        {
            return Math.Abs((float)(l.length - this.length));
        }

        public bool isThisLineInRelation(Line l)
        {
            return l == this.l;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool isThisPointInRelation(Point p)
        {
            return l.Pt1 == p || l.Pt2 == p;
        }

        public IEnumerable<Line> getLinesInRelation()
        {
            yield return this.l;
        }

        public float ScoreWithChange(int dx, int dy, poligonEditor.components.Point movingPoint)
        {
            // We check if movingPoint is on line if so, choose other
            if(l.Pt1 == movingPoint)
            {
                poligonEditor.components.Line tmpLine = new poligonEditor.components.Line(new components.Point(l.Pt1), new components.Point(l.Pt2));
                tmpLine.Pt2.movePointByDelta(dx, dy);
                return this.ScoreForLine(tmpLine);
            }
            else if(l.Pt2 == movingPoint)
            {
                poligonEditor.components.Line tmpLine = new poligonEditor.components.Line(new components.Point(l.Pt1), new components.Point(l.Pt2));
                tmpLine.Pt1.movePointByDelta(dx, dy);
                return this.ScoreForLine(tmpLine);
            }
            else
            {
                poligonEditor.components.Line tmpLinePt1 = new poligonEditor.components.Line(new components.Point(l.Pt1), new components.Point(l.Pt2));
                poligonEditor.components.Line tmpLinePt2 = new poligonEditor.components.Line(new components.Point(l.Pt1), new components.Point(l.Pt2));
                tmpLinePt1.Pt1.movePointByDelta(dx, dy);
                tmpLinePt2.Pt2.movePointByDelta(dx, dy);
                var scorePt1 = this.ScoreForLine(tmpLinePt1);
                var scorePt2 = this.ScoreForLine(tmpLinePt2);
                if (scorePt1 > scorePt2) return scorePt2;
                else return scorePt1;
            }
        }

        public void moveByChange(int dx, int dy, Point movingPoint)
        {
            if (l.Pt1 == movingPoint)
            {
                l.Pt2.movePointByDelta(dx, dy);
            }
            else if (l.Pt2 == movingPoint)
            {
                l.Pt1.movePointByDelta(dx, dy);
            }
            else
            {
                poligonEditor.components.Line tmpLinePt1 = new poligonEditor.components.Line(new components.Point(l.Pt1), new components.Point(l.Pt2));
                poligonEditor.components.Line tmpLinePt2 = new poligonEditor.components.Line(new components.Point(l.Pt1), new components.Point(l.Pt2));
                tmpLinePt1.Pt1.movePointByDelta(dx, dy);
                tmpLinePt2.Pt2.movePointByDelta(dx, dy);
                var scorePt1 = this.ScoreForLine(tmpLinePt1);
                var scorePt2 = this.ScoreForLine(tmpLinePt2);
                if (scorePt1 > scorePt2) l.Pt2.movePointByDelta(dx, dy);
                else l.Pt1.movePointByDelta(dx, dy);
            }
        }
    }
}