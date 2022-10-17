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
        float scoreWithChange(int dx, int dy, components.Line activeLine, components.Point stationaryPoint);
        void moveByChange(int dx, int dy, components.Line activeLine, components.Point stationaryPoint);
        bool isThisLineInRelation(polygonEditor.components.Line l);
        bool isThisPointInRelation(polygonEditor.components.Point p);

        void deleteLabel();

        bool alreadyMoved();
        void setMove(bool state);

    }
}
