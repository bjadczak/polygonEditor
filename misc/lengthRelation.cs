using polygonEditor.components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polygonEditor.misc
{
    internal class lengthRelation : polygonEditor.misc.IRelation, IDisposable
    {
        public polygonEditor.components.Line l { get; private set; }
        public float length { get; private set; }
        public lengthRelation(polygonEditor.components.Line l, float displayLength)
        {
            this.l = l;
            this.length = displayLength * displayLength;
        }
        public float Score()
        {
            return Math.Abs((float)(l.length - this.length));
        }
        public float ScoreForLine(polygonEditor.components.Line l)
        {
            return Math.Abs((float)(l.length - this.length));
        }

        public bool isThisLineInRelation(Line l)
        {
            return l == this.l;
        }

        private float scoreWithDelta(Line l, int dx1, int dy1, int dx2, int dy2)
        {
            float x1 = l.Pt1.x + dx1;
            float y1 = l.Pt1.y + dy1;
            float x2 = l.Pt2.x + dx2;
            float y2 = l.Pt2.y + dy2;

            return Math.Abs((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) - this.length);



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

        public float ScoreWithChange(int dx, int dy, polygonEditor.components.Point movingPoint)
        {
            // We check if movingPoint is on line if so, choose other
            if (l.Pt1 == movingPoint)
            {
                return this.scoreWithDelta(this.l, 0, 0, dx, dy);
            }
            else if (l.Pt2 == movingPoint)
            {
                return this.scoreWithDelta(this.l, dx, dy, 0, 0);
            }
            else
            {
                var scorePt1 = this.scoreWithDelta(this.l, dx, dy, 0, 0);
                var scorePt2 = this.scoreWithDelta(this.l, 0, 0, dx, dy);
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
                var scorePt1 = this.scoreWithDelta(this.l, dx, dy, 0, 0);
                var scorePt2 = this.scoreWithDelta(this.l, 0, 0, dx, dy);
                if (scorePt1 > scorePt2) l.Pt2.movePointByDelta(dx, dy);
                else l.Pt1.movePointByDelta(dx, dy);
            }
        }

        public void Fix(Line l, components.Point movingPoint, IEnumerable<IRelation> relations)
        {
            l.fixForLength(this, movingPoint, relations);
        }
    }
}