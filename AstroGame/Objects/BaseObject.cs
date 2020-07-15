using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AstroGame
{
    class BaseObject
    {
        protected Point position;
        protected Point direction;
        protected Size size;

        public BaseObject(Point position, Point direction, Size size)
        {
            this.position = position;
            this.direction = direction;
            this.size = size;
        }

        // Отрисовка объекта на игровом поле
        public virtual void Draw()
        {
            Game.buffer.Graphics.DrawEllipse(Pens.Wheat, position.X, position.Y, size.Width, size.Height);
        }

        // Обновление поведения объекта
        public virtual void Update()
        {
            // Изменение позиции по направлению
            position.X = position.X + direction.X;
            position.Y = position.Y + direction.Y;

            // Инвертируем направление при достижении границ экрана
            if (position.X < 0 || position.X > Game.Width) 
                direction.X = -direction.X;

            if (position.Y < 0 || position.Y > Game.Height)
                direction.Y = -direction.Y;
        }
    }
}
