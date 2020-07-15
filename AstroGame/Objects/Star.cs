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

        public Star(Point position, Point direction, Size size)
            :base(position, direction, size)
        {

        }

        public override void Draw()
        {
            /*            base.Draw();
                        Game.buffer.Graphics.DrawLine(Pens.White, position.X, position.Y, position.X + size.Width, position.Y + size.Height);
                        Game.buffer.Graphics.DrawLine(Pens.White, position.X + size.Width, position.Y, position.X, position.Y + size.Height);*/
            Game.buffer.Graphics.DrawImage(image, position.X, position.Y, size.Width, size.Height);
        }
    }
}
