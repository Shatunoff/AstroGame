using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AstroGame
{
    class Star:BaseObject
    {
        static Image image = Image.FromFile(@"Images\star.png");

        public Star(Point position, Point direction)
            :base(position, direction, new Size(25, 25))
        {

        }

        public override void Draw()
        {
            Game.buffer.Graphics.DrawImage(image, position.X, position.Y, size.Width, size.Height);
        }

        // Обновление поведения объекта
        public override void Update()
        {
            // Изменение позиции по направлению
            position.X += direction.X;
            position.Y += direction.Y;

            // Обновить позицию при достижении нижней границы игрового поля
            if (position.Y > Game.Height) Reset();
        }

        public override void Reset()
        {
            position.X = Game.Rnd.Next(0, Game.Width);
            position.Y = Game.Rnd.Next(0, Game.Height) - Game.Height;
        }
    }
}
