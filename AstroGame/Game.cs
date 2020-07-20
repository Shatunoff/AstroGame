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
        static Bullet bullet;
        static Ship ship;

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

            form.KeyDown += Form_KeyDown;
            Ship.action += Ship_action;
        }

        private static void Ship_action(string obj)
        {
            Console.WriteLine(obj);
        }

        private static void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up) ship.Move(new Point(0, -5));
            if (e.KeyCode == Keys.Down) ship.Move(new Point(0, 5));
            if (e.KeyCode == Keys.Left) ship.Move(new Point(-5, 0));
            if (e.KeyCode == Keys.Right) ship.Move(new Point(5, 0));
            if (e.KeyCode == Keys.Space) bullet = new Bullet(new Point((ship.Position.X + ship.Size.Width / 2), ship.Position.Y), new Point(0, 5));
        }

        // Создать игровые элементы
        static public void Load()
        {
            stars = new Star[30];

            for (int i = 0; i < stars.Length; i++)
                stars[i] = new Star(new Point(i*15, Rnd.Next(0, Height) - Height), new Point(0, 8));

            asteroid = new Asteroid(new Point(Rnd.Next(0, Width), Rnd.Next(0, Height) - Height), new Point(0, 8));

            ship = new Ship(new Point(0, Width / 2), new Point(0, 0));
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
            buffer.Graphics.DrawImage(background, 0, 0);

            foreach (Star star in stars)
                star.Draw();

            asteroid.Draw();
            bullet?.Draw();
            ship.Draw();
            buffer.Render();
        }

        static public void Update()
        {
            ship.Update();
            bullet?.Update();

            asteroid.Update();
            if (bullet != null)
            {
                if (asteroid.Collision(bullet))
                {
                    asteroid.Reset();
                    bullet = null;
                }
            }

            foreach (Star star in stars)
            {
                star.Update();
                if (bullet != null)
                {
                    if (star.Collision(bullet))
                    {
                        star.Reset();
                        bullet = null;
                    }
                }
            }

        }
    }
}
