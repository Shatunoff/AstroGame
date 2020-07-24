using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroGame
{
    static class GamePoints
    {
        static public string    Caption { get; private set; }   = "Очки";
        static public int       Value   { get; private set; }   = 0;

        static public string    GetPointsToString()             => Caption + ": " + Value.ToString();
        static public void      PointsLowUp()                   => Value += 100;
        static public void      PointsMiddleUp()                => Value += 200;
        static public void      PointsHightUp()                 => Value += 300;
    }
}
