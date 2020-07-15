using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace AstroGame
{
    static class Game
    {
        static BufferedGraphicsContext context;
        static public BufferedGraphics buffer;

        //Свойства
        //Ширина и высота игрового поля
        static public int Width { get; set; }
        static public int Height { get; set; }
        static public BaseObject[] objs;
        static public Star[] stars;
        static Image background = Image.FromFile(@"Images\fon.jpg");


        static Game()
        {

        }

        // Создать графический буфер для элемента, привязываем его к полю buffer
        static public void Init(Form form)
        {
            // Графическое устройство для вывода графики
            Graphics g;
            // предоставляет доступ к главному буферу графического контекста для текущего приложения
            context = BufferedGraphicsManager.Current;
            g = form.CreateGraphics(); // Создаем объект - поверхность рисования и связываем его с формой
            // Запоминаем размеры формы
            Width = form.Width;
            Height = form.Height;

            buffer = context.Allocate(g, new Rectangle(0, 0, Width, Height));
        }

        // Создать игровые элементы
        static public void Load()
        {
            objs = new BaseObject[30];
            stars = new Star[30];

            for (int i = 0; i < objs.Length; i++)
                objs[i] = new BaseObject(new Point(300, i * 20), new Point(-i, -i), new Size(10, 10));
            for (int i = 0; i < stars.Length; i++)
                stars[i] = new Star(new Point(150, i * 20), new Point(-i, 0), new Size(5, 5));

            Timer timer = new Timer();
            timer.Interval = 100;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            Update();
            Draw();
        }

        static public void Draw()
        {
            //buffer.Graphics.Clear(Color.Black);
            buffer.Graphics.DrawImage(background, 0, 0);

            foreach (BaseObject obj in objs)
                obj.Draw();

            foreach (Star star in stars)
                star.Draw();

            buffer.Render();
        }

        static public void Update()
        {
            foreach (BaseObject obj in objs)
                obj.Update();
            foreach (Star star in stars)
                star.Update();
        }
    }
}
