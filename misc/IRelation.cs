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
        bool isThisLineInRelation(poligonEditor.components.Line l);
    }
}
