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
        bool isThisLineInRelation(poligonEditor.components.Line l);
        bool isThisPointInRelation(poligonEditor.components.Point p);
        IEnumerable<poligonEditor.components.Line> getLinesInRelation();

    }
}
