using polygonEditor.components;
using polygonEditor.misc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;

namespace polygonEditor.misc
{
    internal class lengthRelation : IRelation, IDisposable
    {
        public polygonEditor.components.Line l { get; private set; }
        public float length { get; private set; }

        public string label;

        public lengthRelation(polygonEditor.components.Line l, float displayLength)
        {
            this.l = l;
            this.length = displayLength * displayLength;
            label = String.Format("{0}", (displayLength).ToString());
            l.labels.Add(label);
        }
        public float Score()
        {
            return Math.Abs((float)(l.length - this.length));
        }

        public bool isThisLineInRelation(Line l)
        {
            return this.l.Pt1 == l.Pt1 || this.l.Pt1 == l.Pt2 ||
                this.l.Pt2 == l.Pt1 || this.l.Pt2 == l.Pt2;
        }

        public void Dispose()
        {
            this.deleteLabel();
        }

        public bool isThisPointInRelation(polygonEditor.components.Point p)
        {
            return l.Pt1 == p || l.Pt2 == p;
        }


        public void deleteLabel()
        {
            l.labels.Remove(label);
        }

        private float scoreWithDelta(int dx1, int dy1, int dx2, int dy2)
        {
            return Math.Abs((l.Pt1.x + dx1 - l.Pt2.x - dx2) * (l.Pt1.x + dx1 - l.Pt2.x - dx2) + (l.Pt1.y + dy1 - l.Pt2.y - dy2) * (l.Pt1.y + dy1 - l.Pt2.y - dy2) - this.length);
        }

        public float scoreWithChange(int dx, int dy, Line activeLine, components.Point stationaryPoint)
        {
            if(l == activeLine)
            {
                if (l.Pt1 == stationaryPoint) 
                {
                    return scoreWithDelta(0, 0, dx, dy); 
                }
                else if (l.Pt2 == stationaryPoint)
                {
                    return scoreWithDelta(dx, dy, 0, 0);
                }
                else
                {
                    return Math.Min(
                        scoreWithDelta(dx, dy, 0, 0),
                        scoreWithDelta(0, 0, dx, dy)
                        );
                }
            }
            else
            {
                return float.MaxValue;
            }
        }

        public void moveByChange(int dx, int dy, Line activeLine, components.Point stationaryPoint)
        {
            if (l == activeLine)
            {
                if (l.Pt1 == stationaryPoint)
                {
                    l.Pt2.movePointByDelta(dx, dy);
                }
                else if (l.Pt2 == stationaryPoint)
                {
                    l.Pt1.movePointByDelta(dx, dy);
                }
                else
                {
                    if(scoreWithDelta(dx, dy, 0, 0) < scoreWithDelta(0, 0, dx, dy))
                    {
                        l.Pt1.movePointByDelta(dx, dy);
                    }
                    else
                    {
                        l.Pt2.movePointByDelta(dx, dy);
                    }
                }
            }
        }

        private bool _moved = false;
        public bool alreadyMoved()
        {
            return _moved;
        }

        public void setMove(bool state)
        {
            _moved = state;
        }
    }
}