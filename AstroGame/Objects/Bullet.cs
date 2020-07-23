using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AstroGame
{
    class Bullet:BaseObject
    {
        public static event Action<Bullet> bulletReset;

        static Image image = Image.FromFile(@"Images\bullet.png");

        public Bullet(Point position, Point direction)
            :base(position, direction, new Size(15, 25))
        {
            
        }

        public override void Draw()
        {
            Game.buffer.Graphics.DrawImage(image, position.X, position.Y, size.Width, size.Height);
        }

        public override void Update()
        {
            // Изменение позиции по направлению
            position.X -= direction.X;
            position.Y -= direction.Y;

            // Выход за границы экрана
            if (position.Y <= 0) Reset();
        }

        public override void Reset()
        {
            bulletReset?.Invoke(this);
        }
    }
}
