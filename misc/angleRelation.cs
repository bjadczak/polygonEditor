using polygonEditor.components;
using polygonEditor.misc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;

namespace polygonEditor.misc
{
    internal class angleRelation : IRelation, IDisposable
    {
        private int modifier = 10000;
        public components.Line l1 { get; private set; }
        public components.Line l2 { get; private set; }

        private static int _count = 1;
        public string label;
        public angleRelation(components.Line l1, components.Line l2)
        {
            label = String.Format("|| - {0}", (_count++).ToString());
            this.l1 = l1;
            this.l2 = l2;
            l1.labels.Add(label);
            l2.labels.Add(label);
        }
        public void Dispose()
        {
            this.deleteLabel();
        }

        public bool isThisLineInRelation(Line l)
        {
            return this.l1.Pt1 == l.Pt1 || this.l1.Pt1 == l.Pt2 || this.l1.Pt2 == l.Pt1 || this.l1.Pt2 == l.Pt2 ||
                this.l2.Pt1 == l.Pt1 || this.l2.Pt1 == l.Pt2 || this.l2.Pt2 == l.Pt1 || this.l2.Pt2 == l.Pt2;
        }

        public bool isThisPointInRelation(polygonEditor.components.Point p)
        {
            return l1.Pt1 == p || l1.Pt2 == p || l2.Pt1 == p || l2.Pt2 == p;
        }

        public float Score()
        {
            return modifier * Math.Abs((float)(l1.atan - l2.atan));
        }

        public void deleteLabel()
        {
            l1.labels.Remove(label);
            l2.labels.Remove(label);
        }

        private float scoreWithDelta(int dx1, int dy1, int dx2, int dy2, int dx3, int dy3, int dx4, int dy4)
        {
            return modifier * Math.Abs(
                (float)Math.Atan((l1.Pt2.y + dy2 - l1.Pt1.y - dy1) / (l1.Pt2.x + dx2 - l1.Pt1.x - dx2)) - 
                (float)Math.Atan((l2.Pt2.y + dy4 - l2.Pt1.y - dy3) / (l2.Pt2.x + dx4 - l2.Pt1.x - dx3)));
        }

        public float scoreWithChange(int dx, int dy, Line activeLine, components.Point stationaryPoint)
        {
            if (l1 == activeLine)
            {
                if (l1.Pt1 == stationaryPoint)
                {
                    return scoreWithDelta(0, 0, dx, dy, 0, 0, 0, 0);
                }
                else if (l1.Pt2 == stationaryPoint)
                {
                    return scoreWithDelta(dx, dy, 0, 0, 0, 0, 0, 0);
                }
                else if (stationaryPoint is null)
                {
                    return Math.Min(
                        scoreWithDelta(dx, dy, 0, 0, 0, 0, 0, 0),
                        scoreWithDelta(0, 0, dx, dy, 0, 0, 0, 0));
                }
                else
                {
                    return float.MaxValue;
                }
            }
            else if (l2 == activeLine)
            {
                if (l2.Pt1 == stationaryPoint)
                {
                    return scoreWithDelta(0, 0, 0, 0, 0, 0, dx, dy);
                }
                else if (l2.Pt2 == stationaryPoint)
                {
                    return scoreWithDelta(0, 0, 0, 0, dx, dy, 0, 0);
                }
                else if(stationaryPoint is null)
                {
                    return Math.Min(
                        scoreWithDelta(0, 0, 0, 0, dx, dy, 0, 0),
                        scoreWithDelta(0, 0, 0, 0, 0, 0, dx, dy));
                }
                else
                {
                    return float.MaxValue;
                }
            }
            else
            {
                return float.MaxValue;
            }
        }

        public void moveByChange(int dx, int dy, Line activeLine, components.Point stationaryPoint)
        {
            if (l1 == activeLine)
            {
                if (l1.Pt1 == stationaryPoint)
                {
                    l1.Pt2.movePointByDelta(dx, dy);
                }
                else if (l1.Pt2 == stationaryPoint)
                {
                    l1.Pt1.movePointByDelta(dx, dy);
                }
                else if (stationaryPoint is null)
                {
                    if(scoreWithDelta(dx, dy, 0, 0, 0, 0, 0, 0) < scoreWithDelta(0, 0, dx, dy, 0, 0, 0, 0))
                    {
                        l1.Pt1.movePointByDelta(dx, dy);
                    }
                    else
                    {
                        l1.Pt2.movePointByDelta(dx, dy);
                    }
                }
            }
            else if (l2 == activeLine)
            {
                if (l2.Pt1 == stationaryPoint)
                {
                    l2.Pt2.movePointByDelta(dx, dy);
                }
                else if (l2.Pt2 == stationaryPoint)
                {
                    l2.Pt1.movePointByDelta(dx, dy);
                }
                else if(stationaryPoint is null)
                {
                    if(scoreWithDelta(0, 0, 0, 0, dx, dy, 0, 0) < scoreWithDelta(0, 0, 0, 0, 0, 0, dx, dy))
                    {
                        l2.Pt1.movePointByDelta(dx, dy);
                    }
                    else
                    {
                        l2.Pt2.movePointByDelta(dx, dy);
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
