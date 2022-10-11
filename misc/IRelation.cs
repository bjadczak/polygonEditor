using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poligonEditor.misc
{
    internal interface IRelation
    {
        float Score();
        float ScoreForLine(poligonEditor.components.Line l);
        float ScoreWithChange(int dx, int dy, poligonEditor.components.Point movingPoint);
        bool isThisLineInRelation(poligonEditor.components.Line l);
        bool isThisPointInRelation(poligonEditor.components.Point p);
        void Fix(poligonEditor.components.Line l, components.Point movingPoint, IEnumerable<IRelation> relations);

    }
}
