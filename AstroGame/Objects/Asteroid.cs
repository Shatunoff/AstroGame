using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AstroGame
{
    class Asteroid : BaseObject
    {
        public static event Action<Asteroid> asteroidReset;

        static Image[] images =
        {
            Image.FromFile(@"Images\meteor.png"),
            Image.FromFile(@"Images\space.png"),
            Image.FromFile(@"Images\spongebob.png")
        };

        Image image;

        public Asteroid(Point position, Point direction)
            : base(position, direction, new Size(64, 64))
        {
            image = images[Game.Rnd.Next(images.Length)];
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
            if (position.Y > Game.Height) RefreshPosition();
        }

        public void RefreshPosition()
        {
            position.X = Game.Rnd.Next(0, Game.Width);
            position.Y = Game.Rnd.Next(0, Game.Height) - Game.Height;
            image = images[Game.Rnd.Next(images.Length)];
        }

        public override void Reset()
        {
            asteroidReset?.Invoke(this);
        }
    }
}
