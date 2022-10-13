﻿using polygonEditor.components;
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
    internal class angleRelation : polygonEditor.misc.IRelation, IDisposable
    {
        private int modifier = 100;
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
            l1.labels.Remove(label);
            l1.labels.Remove(label);
        }

        public void Fix(Line l, components.Point movingPoint, IEnumerable<IRelation> relations)
        {
            l.fixForAngle(this, movingPoint, relations);
        }

        public bool isThisLineInRelation(Line l)
        {
            return l == l1 || l == l2;
        }

        public bool isThisPointInRelation(polygonEditor.components.Point p)
        {
            return l1.Pt1 == p || l1.Pt2 == p || l2.Pt1 == p || l2.Pt2 == p;
        }

        public float Score()
        {
            return modifier * Math.Abs((float)(l1.atan - l2.atan));
        }
        public float ScoreWithChanges(int dx, int dy, components.Line moveLine, components.Point movingPoint)
        {

            if (l2 == moveLine)
                if (l2.Pt1 == movingPoint) return modifier * Math.Abs((float)(l1.atan - (float)Math.Atan((l2.Pt2.y - l2.Pt1.y - dy) / (l2.Pt2.x - l2.Pt1.x - dx))));
                else return modifier * Math.Abs((float)(l1.atan - (float)Math.Atan((l2.Pt2.y + dy - l2.Pt1.y) / (l2.Pt2.x + dx - l2.Pt1.x))));
            else
                if (l1.Pt1 == movingPoint) return modifier * Math.Abs((float)(l2.atan - (float)Math.Atan((l1.Pt2.y - l1.Pt1.y - dy) / (l1.Pt2.x - l1.Pt1.x - dx))));
            else return modifier * Math.Abs((float)(l2.atan - (float)Math.Atan((l1.Pt2.y + dy - l1.Pt1.y) / (l1.Pt2.x + dx - l1.Pt1.x))));

        }
        public void moveByChange(int dx, int dy, components.Line moveLine, components.Point movingPoint)
        {
            if (l2 == moveLine)
            {
                if (l2.Pt2 == movingPoint)
                {
                    l2.Pt2.movePointByDelta(dx, dy);
                }
                else
                {
                    l2.Pt1.movePointByDelta(dx, dy);
                }
            }
            else
            {
                if (l1.Pt2 == movingPoint)
                {
                    l1.Pt2.movePointByDelta(dx, dy);
                }
                else
                {
                    l1.Pt1.movePointByDelta(dx, dy);
                }
            }

        }

    }
}
