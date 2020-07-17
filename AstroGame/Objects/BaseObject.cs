using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AstroGame
{
    interface ICollision
    {
        bool Collision(ICollision obj);
        Rectangle Rect { get; }
    }

    abstract class BaseObject:ICollision
    {
        protected Point position; // Текущая позиция
        protected Point direction; // Направление движения (целевая позиция)
        protected Size size; // Размер объекта

        // Прямоугольная область объекта
        public Rectangle Rect
        {
            get
            {
                return new Rectangle(position, size);
            }
        }

        // Базовый конструктор объекта
        public BaseObject(Point position, Point direction, Size size)
        {
            this.position = position;
            this.direction = direction;
            this.size = size;
        }
        
        public abstract void Draw(); // Отрисовка объекта на игровом поле
        public abstract void Update(); // Обновление поведения объекта
        public abstract void Reset(); // Сброс позиции объекта

        // Проверка пересечения с другим объектом ICollision
        public bool Collision(ICollision obj)
        {
            return this.Rect.IntersectsWith(obj.Rect);
        }
    }
}
