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
        static Star[] stars;
        static Asteroid[] asteroids;
        static List<Bullet> bullets = new List<Bullet>();
        static Ship ship;

        // Информационные надписи
        static public string EnergyText = "Энергия: ";
        static public int EnergyValue;

        // Конфигурация коллекций
        const int COUNT_STARS = 40; // Количество звезд
        const int COUNT_ASTEROIDS = 2; // Количество астероидов

        static Image background = Image.FromFile(@"Images\fon.jpg");


        static Game()
        {

        }

        // Создать графический буфер для элемента, привязываем его к полю buffer
        static public void Init(Form form)
        {
            Graphics g;
            context = BufferedGraphicsManager.Current;
            g = form.CreateGraphics();
            Width = form.Width - 15;
            Height = form.Height;
            buffer = context.Allocate(g, new Rectangle(0, 0, Width, Height));

            // Скрыть курсор мыши в пределах игрового поля
            form.MouseEnter += delegate { Cursor.Hide(); };
            form.MouseLeave += delegate { Cursor.Show(); };

            // Управление кораблем с помощью мыши
            form.MouseMove += Form_MouseMove;
            form.MouseDown += Form_MouseDown;

            // Подписываемся на события корабля
            Ship.EnergyChanged += Ship_EnergyChanged;
            Ship.OutOfEnergy += Ship_OutOfEnergy;
            Bullet.bulletReset += Bullet_bulletReset;

        }

        private static void Bullet_bulletReset(Bullet obj)
        {
            obj = null;
        }

        // Изменение энергии корабля
        private static void Ship_EnergyChanged()
        {
            if (ship != null)
            {
                EnergyValue = ship.Energy;
            }
        }

        // Отсутствие энергии у корабля
        private static void Ship_OutOfEnergy()
        {
            GameOver();
        }

        private static void GameOver()
        {
            gameTicker?.Stop();
            Cursor.Show();
            MessageBox.Show($"Ваш корабль был уничтожен. Игра окончена.\nНабрано очков: {GamePoints.Value.ToString()}", "Конец игры");
        }

        private static void Form_MouseDown(object sender, MouseEventArgs e)
        {
            // Выстрел по нажатию ЛКМ
            if (e.Button == MouseButtons.Left)
            {
                bullets.Add(new Bullet(new Point((ship.Position.X + ship.Size.Width / 2), ship.Position.Y), new Point(0, 25)));
            }
        }

        private static void Form_MouseMove(object sender, MouseEventArgs e)
        {
            // Движение вслед за мышью, если та в пределах игрового поля
            if ((e.X >= 0 && e.X <= Width) && (e.Y >= 0 && e.Y < Height)) ship.Move(e.Location);
        }

        // Создать игровые элементы
        static public void Load()
        {
            stars = new Star[COUNT_STARS];
            asteroids = new Asteroid[COUNT_ASTEROIDS];

            for (int i = 0; i < stars.Length; i++)
                stars[i] = new Star(new Point(i*15, Rnd.Next(0, Height) - Height), new Point(0, 8));

            for (int i = 0; i < asteroids.Length; i++)
                asteroids[i] = new Asteroid(new Point(Rnd.Next(0, Width), Rnd.Next(0, Height) - Height), new Point(0, 8));

            ship = new Ship(new Point(0, Width / 2), new Point(0, 0));

            // Заполняем информационные переменные начальными данными
            EnergyValue = ship.Energy;

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
            Game.buffer.Graphics.DrawString(EnergyText + EnergyValue.ToString(), SystemFonts.CaptionFont, Brushes.White, 0, 0);
            Game.buffer.Graphics.DrawString(GamePoints.GetPointsToString(), SystemFonts.CaptionFont, Brushes.White, 100, 0);

            foreach (Star star in stars)
                star?.Draw();

            foreach (Bullet bullet in bullets)
                bullet?.Draw();

            foreach (Asteroid asteroid in asteroids)
                asteroid?.Draw();

            ship?.Draw();
            buffer.Render();
        }

        static public void Update()
        {
            ship?.Update();

            // Проверка состояния пуль
            foreach (Bullet bullet in bullets)
            {
                if (bullet != null)
                {
                    bullet.Update();
                    // Проверка на столкновение ПУЛЯ == АСТЕРОИД
                    foreach (Asteroid asteroid in asteroids)
                    {
                        if (asteroid != null)
                        {
                            if (bullet.Collision(asteroid))
                            {
                                asteroid.Reset();
                                GamePoints.PointsHightUp();
                                bullet.Reset();
                            }
                        }
                    }
                    // Проверка на столкновение ПУЛЯ == ЗВЕЗДА
                    foreach (Star star in stars)
                    {
                        if (star != null)
                        {
                            if (bullet.Collision(star))
                            {
                                star.Reset();
                                GamePoints.PointsMiddleUp();
                                bullet.Reset();
                            }
                        }
                    }
                }
            }

            // Проверка состояния Астероидов
            foreach (Asteroid asteroid in asteroids)
            {
                if (asteroid != null)
                {
                    asteroid.Update();
                    // Проверка на столкновение АСТЕРОИД == КОРАБЛЬ
                    if (asteroid.Collision(ship))
                    {
                        asteroid.Reset(); // Сбросить  позицию астероида
                        GamePoints.PointsMiddleUp();
                        ship.Damage(20);
                    }
                }
            }

            // Проверка состояния Звезд
            foreach (Star star in stars)
            {
                if (star != null)
                {
                    star.Update();
                    // Проверка на столкновение ЗВЕЗДА == КОРАБЛЬ
                    if (star.Collision(ship))
                    {
                        star.Reset(); // Сбросить позицию звезды
                        GamePoints.PointsLowUp();
                        ship.Damage(10);
                    }
                }
            }
        }
    }
}
