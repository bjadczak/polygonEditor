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
    }
}
