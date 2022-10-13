using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polygonEditor.misc
{
    internal interface IRelation
    {
        float Score();
        bool isThisLineInRelation(polygonEditor.components.Line l);
        bool isThisPointInRelation(polygonEditor.components.Point p);
        void Fix(polygonEditor.components.Line l, components.Point movingPoint, IEnumerable<IRelation> relations);

        void deleteLabel();

    }
}
