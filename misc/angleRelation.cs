using poligonEditor.components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poligonEditor.misc
{
    internal class angleRelation : poligonEditor.misc.IRelation, IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Line> getLinesInRelation()
        {
            throw new NotImplementedException();
        }

        public bool isThisLineInRelation(Line l)
        {
            throw new NotImplementedException();
        }

        public bool isThisPointInRelation(Point p)
        {
            throw new NotImplementedException();
        }

        public void moveByChange(int dx, int dy, Point movingPoint)
        {
            throw new NotImplementedException();
        }

        public float Score()
        {
            throw new NotImplementedException();
        }

        public float ScoreForLine(Line l)
        {
            throw new NotImplementedException();
        }

        public float ScoreWithChange(int dx, int dy, poligonEditor.components.Point movingPoint)
        {
            throw new NotImplementedException();
        }
    }
}
