using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AstroGame
{
    class Ship : BaseObject
    {
        static public event Action<string> action;
        static Image image = Image.FromFile(@"Images\ship.png");

        public int Energy { get; set; } = 100;

        public Point Position
        {
            get
            {
                return position;
            }
        }

        public Size Size
        {
            get
            {
                return size;
            }
        }

        public Ship(Point position, Point direction)
            : base(position, direction, new Size(50, 50))
        {

        }

        public override void Draw()
        {
            Game.buffer.Graphics.DrawImage(image, position.X, position.Y, size.Width, size.Height);
            Game.buffer.Graphics.DrawString(Energy.ToString(), SystemFonts.CaptionFont, Brushes.White, position.X - 20, position.Y - size.Height / 3);
        }

        // Обновление поведения объекта
        public override void Update()
        {
            // Изменение позиции по направлению
            position.X += direction.X;
            position.Y += direction.Y;

            // Обновить позицию при достижении нижней границы игрового поля
            //if (position.Y > Game.Height) Reset();
        }

        public override void Reset()
        {
            //position.X = Game.Rnd.Next(0, Game.Width);
            //position.Y = Game.Rnd.Next(0, Game.Height) - Game.Height;
        }

        public void Move(Point direction)
        {
            position.X += direction.X;
            position.Y += direction.Y;

            if (position.X == 0) action("LEFT");
            if (position.X == Game.Width) action("RIGHT");
            if (position.Y == 0) action("TOP");
            if (position.Y == Game.Height) action("BOTTOM");
        }
    }
}
