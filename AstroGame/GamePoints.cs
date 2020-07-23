using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroGame
{
    static class GamePoints
    {
        static private int pointsValue = 0;

        static public int Value
        {
            get
            {
                return pointsValue;
            }
        }

        static public string Caption { get; set; } = "Очки";

        static public string GetPointsToString()
        {
            return Caption + ": " + pointsValue.ToString();
        }

        static public void PointsLowUp() => pointsValue += 100;
        static public void PointsMiddleUp() => pointsValue += 200;
        static public void PointsHightUp() => pointsValue += 300;
    }
}
