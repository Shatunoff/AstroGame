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

        //Ширина и высота игрового поля
        static public int Width { get; set; }
        static public int Height { get; set; }

        // Управляющие элементы
        static public Random Rnd = new Random();
        static Timer gameTicker = new Timer();

        // Игровые объекты
        static public Star[] stars;
        static public Asteroid asteroid;

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
            Width = form.Width - 15;
            Height = form.Height;

            buffer = context.Allocate(g, new Rectangle(0, 0, Width, Height));
        }

        // Создать игровые элементы
        static public void Load()
        {
            stars = new Star[30];

            for (int i = 0; i < stars.Length; i++)
                stars[i] = new Star(new Point(i*15, Rnd.Next(0, Height) - Height), new Point(0, 8), new Size(25, 25));

            asteroid = new Asteroid(new Point(Rnd.Next(0, Width), Rnd.Next(0, Height) - Height), new Point(0, 8), new Size(64, 64));

            // Обновление поведения игровых объектов
            gameTicker.Interval = 100;
            gameTicker.Tick += GameTimer_Tick;
            gameTicker.Start();
        }

        private static void GameTimer_Tick(object sender, EventArgs e)
        {
            Update();
            Draw();
        }

        static public void Draw()
        {
            //buffer.Graphics.Clear(Color.Black);
            buffer.Graphics.DrawImage(background, 0, 0);

            foreach (Star star in stars)
                star.Draw();

            asteroid.Draw();

            buffer.Render();
        }

        static public void Update()
        {
            foreach (Star star in stars)
                star.Update();

            asteroid.Update();
        }
    }
}
