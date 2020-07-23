using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AstroGame
{
    class Ship : BaseObject
    {
        static public event Action OutOfEnergy; // Отсутствует энергия
        static public event Action EnergyChanged; // Энергия сократилась

        static Image image = Image.FromFile(@"Images\ship.png");

        private int energy = 100;
        public int Energy
        {
            get
            {
                return energy;
            }
            private set
            {
                if (value <= 0)
                {
                    energy = 0;
                    OutOfEnergy?.Invoke(); // Сообщить подписчикам события об отсутствии энергии
                }
                else
                {
                    energy = value;
                    EnergyChanged?.Invoke(); // Сообщить подписчикам события об изменении количества энергии
                }
            }
        } 

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
        }

        // Обновление поведения объекта
        public override void Update()
        {
            // Изменение позиции по направлению
            position.X += direction.X;
            position.Y += direction.Y;
        }

        public void Damage(int value)
        {
            Energy -= value;
        }

        public override void Reset()
        {

        }

        public void Move(Point direction)
        {
            position.X = direction.X - 25;
            position.Y = direction.Y - 25;
        }
    }
}
