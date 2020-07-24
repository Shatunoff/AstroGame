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
        static BufferedGraphicsContext  context;
        static public BufferedGraphics  buffer;

        //Ширина и высота игрового поля
        static public int Width     { get; set; }
        static public int Height    { get; set; }

        // Управляющие элементы
        static public   Random          Rnd             = new Random();
        static private  Timer           gameTicker      = new Timer();
        static private  GameLevels      GAME_LEVELS     = new GameLevels();
        static private  GameControl     GAME_CONTROL    = new GameControl();

        // Игровые объекты
        static          Star[]          stars;
        static          Asteroid[]      asteroids;
        static          List<Bullet>    bullets;
        static          Ship            ship;

        // Информационные надписи
        static public string    EnergyText = "Энергия: ";
        static public int       EnergyValue;

        // Конфигурация уровней
        static private int  CONF_CURRENT_LEVEL   = GAME_LEVELS.CurrentLevel;
        static private int  CONF_ASTEROID_COUNT  = GAME_LEVELS.AsteroidCount;
        static private int  CONF_STAR_COUNT      = GAME_LEVELS.StarCount;
        static private int  CONF_ASTEROID_SPEED  = GAME_LEVELS.AsteroidSpeed;
        static private int  CONF_STAR_SPEED      = GAME_LEVELS.StarSpeed;
        static private int  CONF_BULLET_SPEED    = GAME_LEVELS.BulletSpeed;
        static private int  CONF_HEALBOX_COUNT   = GAME_LEVELS.HealBoxCount;

        // Остатки до перехода на следующий уровень
        static private int NOW_ASTEROID_COUNT;
        static private int NOW_STAR_COUNT;
        static private event Action NumberOfStarsOrAsteroidsHasChanged;

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

            // Управление кораблем с помощью мыши
            form.MouseMove += Form_MouseMove;
            form.MouseDown += Form_MouseDown;
            form.MouseEnter += Form_MouseEnter;
            form.MouseLeave += Form_MouseLeave;

            // Подписываемся на события
            GameControl.GameStateChanged += GameControl_GameStateChanged;
            Ship.EnergyChanged += Ship_EnergyChanged;
            Ship.OutOfEnergy += Ship_OutOfEnergy;
            Star.starReset += BaseObjectReset;
            Asteroid.asteroidReset += BaseObjectReset;
            NumberOfStarsOrAsteroidsHasChanged += Game_NumberOfStarsOrAsteroidsHasChanged;
        }

        // Реакция на изменение статуса игры
        private static void GameControl_GameStateChanged()
        {
            switch (GAME_CONTROL.GameState)
            {
                case GameState.PLAYING:
                    gameTicker.Start();
                    Cursor.Hide();
                    break;
                case GameState.RESTARTING:
                    gameTicker.Stop();
                    Cursor.Show();
                    break;
                case GameState.PAUSE:
                    gameTicker.Stop();
                    Cursor.Show();
                    break;
                case GameState.STOP:
                    gameTicker.Stop();
                    Cursor.Show();
                    break;
            }
        }

        private static void Form_MouseEnter(object sender, EventArgs e)
        {
            GAME_CONTROL.Start();
        }

        private static void Form_MouseLeave(object sender, EventArgs e)
        {
            GAME_CONTROL.Pause();
        }

        private static void Game_NumberOfStarsOrAsteroidsHasChanged()
        {
            if (NOW_STAR_COUNT == 0 && NOW_ASTEROID_COUNT == 0)
            {
                GAME_LEVELS.NextLevel();
                Load();
            }
        }

        // Уничтожение звезд, астероидов
        private static void BaseObjectReset(BaseObject obj)
        {
            bool stat = false;
            
            if (obj is Star)
            {
                stat = true;
                stars[Array.IndexOf(stars, (obj as Star))] = null;
                NOW_STAR_COUNT--;
            }
            if (obj is Asteroid)
            {
                stat = true;
                asteroids[Array.IndexOf(asteroids, (obj as Asteroid))] = null;
                NOW_ASTEROID_COUNT--;
            }

            if (stat) NumberOfStarsOrAsteroidsHasChanged?.Invoke();
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
            GAME_CONTROL.Stop();
        }

        //private static void GameStart()
        //{
        //    gameTicker.Start();
        //    gameState = GameState.PLAYING;
        //    Cursor.Hide();
        //}

        //private static void GameStop()
        //{
        //    gameTicker.Stop();
        //    Cursor.Show();
        //}

        //private static void RetryGame()
        //{
        //    GAME_LEVELS = new GameLevels();
        //    GameStart();
        //}

        //private static void GameOver()
        //{
        //    GameStop();
        //    gameState = GameState.STOP;

        //    if(MessageBox.Show($"Ваш корабль был уничтожен.\nУровень: {CONF_CURRENT_LEVEL}\nНабрано очков: {GamePoints.Value}", "Конец игры", MessageBoxButtons.RetryCancel) == DialogResult.Retry)
        //    {
        //        RetryGame();
        //        Load(true);
        //    }
        //    else
        //    {
        //        Application.Exit();
        //    }
        //}

        private static void Form_MouseDown(object sender, MouseEventArgs e)
        {
            // Выстрел по нажатию ЛКМ
            if (e.Button == MouseButtons.Left)
            {
                if (GAME_CONTROL.GameState == GameState.PLAYING)
                    bullets.Add(new Bullet(new Point((ship.Position.X + ship.Size.Width / 2), ship.Position.Y), new Point(0, CONF_BULLET_SPEED)));
            }
        }

        private static void Form_MouseMove(object sender, MouseEventArgs e)
        {
            // Движение вслед за мышью, если та в пределах игрового поля
            if ((e.X >= 0 && e.X <= Width) && (e.Y >= 0 && e.Y < Height)) ship.Move(e.Location);
        }

        // Создать игровые элементы
        static public void Load(bool newShip = false)
        {
            bullets = new List<Bullet>();
            stars = new Star[CONF_STAR_COUNT];
            asteroids = new Asteroid[CONF_ASTEROID_COUNT];

            NOW_STAR_COUNT = CONF_STAR_COUNT;
            NOW_ASTEROID_COUNT = CONF_ASTEROID_COUNT;

            for (int i = 0; i < stars.Length; i++)
                stars[i] = new Star(new Point(Rnd.Next(0, Width), Rnd.Next(0, Height) - Height), new Point(0, CONF_STAR_SPEED));

            for (int i = 0; i < asteroids.Length; i++)
                asteroids[i] = new Asteroid(new Point(Rnd.Next(0, Width), Rnd.Next(0, Height) - Height), new Point(0, CONF_ASTEROID_SPEED));

            if (ship == null || newShip)
                ship = new Ship(new Point(Width / 2, Height), new Point(0, 0));

            // Заполняем информационные переменные начальными данными
            EnergyValue = ship.Energy;

            // Обновление поведения игровых объектов
            //if (newShip) gameTicker = new Timer();
            gameTicker.Interval = 100;
            gameTicker.Tick += GameTimer_Tick;
            gameTicker.Start();
            GAME_CONTROL.Start();
        }

        private static void GameTimer_Tick(object sender, EventArgs e)
        {
            Update();
            Draw();
        }

        static public void Draw()
        {
            buffer.Graphics.DrawImage(background, 0, 0); // Фон
            // Информационные лейблы
            Game.buffer.Graphics.DrawString(EnergyText + EnergyValue.ToString(), SystemFonts.CaptionFont, Brushes.White, 0, 0);
            Game.buffer.Graphics.DrawString(GamePoints.GetPointsToString(), SystemFonts.CaptionFont, Brushes.White, 75, 0);
            Game.buffer.Graphics.DrawString(GAME_LEVELS.GetCurrentLevelToString(), SystemFonts.CaptionFont, Brushes.White, 200, 0);

            foreach (Star star in stars)
                star?.Draw();

            foreach (Bullet bullet in bullets)
                bullet?.Draw();

            foreach (Asteroid asteroid in asteroids)
                asteroid?.Draw();

            ship?.Draw();
            buffer.Render();
        }

        // Обновить состояние игровых объектов
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
