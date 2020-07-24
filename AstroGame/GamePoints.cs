using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroGame
{
    class GamePoints
    {
        public string    Caption { get; private set; }   = "Очки";
        public int       Value   { get; private set; }   = 0;

        public string    GetPointsToString()             => Caption + ": " + Value.ToString();
        public void      PointsLowUp()                   => Value += 100;
        public void      PointsMiddleUp()                => Value += 200;
        public void      PointsHightUp()                 => Value += 300;
        public void      Reset()                         => Value = 0;
    }
}
