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
        static public   Random         Rnd             = new Random();
        static private  Timer          gameTicker;
        static private  GameControl    GAME_CONTROL    = new GameControl();
        static private  GameLevels     GAME_LEVELS     = new GameLevels();
        static private  GamePoints     GAME_POINTS     = new GamePoints();

        // Игровые объекты
        static private Star[]          stars;
        static private Asteroid[]      asteroids;
        static private List<Bullet>    bullets;
        static private Ship            ship;

        // Информационные надписи
        static public string    EnergyText = "Энергия: ";
        static public int       EnergyValue;

        // Конфигурация уровней
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

            // Изменение количества оставшихся "в живых" звезд и астероидов
            NumberOfStarsOrAsteroidsHasChanged += Game_NumberOfStarsOrAsteroidsHasChanged;
            // Управление кораблем с помощью мыши
            form.MouseMove  += Form_MouseMove;
            form.MouseDown  += Form_MouseDown;
            form.MouseEnter += Form_MouseEnter;
            form.MouseLeave += Form_MouseLeave;
            form.LostFocus  += Form_MouseLeave;
            form.KeyDown += Form_KeyDown;

            GameControl.GameStateChanged    += GameControl_GameStateChanged;    // Изменение статуса игры
            GameLevels.LevelIsChanged       += GameLevels_LevelIsChanged;       // Изменение уровня сложности
            Ship.EnergyChanged              += Ship_EnergyChanged;              // Изменение энергии корабля
            Ship.OutOfEnergy                += Ship_OutOfEnergy;                // Отсутствие энергии у корабля
            Star.starReset                  += BaseObject_Reset;                // Уничтожение звезды
            Asteroid.asteroidReset          += BaseObject_Reset;                // Уничтожение астероида
        }

        private static void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                GAME_CONTROL.Restart();
            }
        }

        // Реакция на изменение статуса игры
        private static void GameControl_GameStateChanged()
        {
            switch (GAME_CONTROL.GameState)
            {
                case GameState.PLAYING:
                    gameTicker?.Start();
                    break;
                case GameState.RESTARTING:
                    Load(true);
                    GAME_POINTS.Reset();
                    GAME_LEVELS.DefaultValues();
                    break;
                case GameState.PAUSE:
                    break;
                case GameState.STOP:
                    Unload();
                    GameOverMessageShow();
                    break;
            }
        }

        // Реакция на смену уровня
        private static void GameLevels_LevelIsChanged()
        {
            Load();
        }

        private static void GameOverMessageShow()
        {
            if (MessageBox.Show(
                $"Игра окончена!\n{GAME_LEVELS.GetCurrentLevelToString()}\n{GAME_POINTS.GetPointsToString()}", 
                "Конец игры", 
                MessageBoxButtons.RetryCancel) == DialogResult.Retry)
                GAME_CONTROL.Restart();
            else
                Application.Exit();
        }

        private static void Form_MouseEnter(object sender, EventArgs e)
        { 
            if (GAME_CONTROL.GameState == GameState.PAUSE)
            {
                GAME_CONTROL.Start();
                Cursor.Hide();
            }
        }

        private static void Form_MouseLeave(object sender, EventArgs e)
        {
            if (GAME_CONTROL.GameState == GameState.PLAYING)
            {
                GAME_CONTROL.Pause();
                Cursor.Show();
            }
        }

        private static void Game_NumberOfStarsOrAsteroidsHasChanged()
        {
            if (NOW_STAR_COUNT == 0 && NOW_ASTEROID_COUNT == 0)
            {
                GAME_LEVELS.NextLevel();
            }
        }

        // Уничтожение звезд, астероидов
        private static void BaseObject_Reset(BaseObject obj)
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
                EnergyValue = ship.Energy;
        }

        // Отсутствие энергии у корабля
        private static void Ship_OutOfEnergy()
        {
            if (GAME_CONTROL.GameState == GameState.PLAYING)
                GAME_CONTROL.Stop();
        }

        private static void Form_MouseDown(object sender, MouseEventArgs e)
        {
            // Выстрел по нажатию ЛКМ
            if (e.Button == MouseButtons.Left)
            {
                if (GAME_CONTROL.GameState == GameState.PLAYING)
                    bullets?.Add(new Bullet(new Point((ship.Position.X + ship.Size.Width / 2), ship.Position.Y), new Point(0, CONF_BULLET_SPEED)));
            }
        }

        private static void Form_MouseMove(object sender, MouseEventArgs e)
        {
            // Движение вслед за мышью, если та в пределах игрового поля
            if ((e.X >= 0 && e.X <= Width) && (e.Y >= 0 && e.Y < Height)) ship?.Move(e.Location);
        }

        // Создать игровые элементы
        static public void Load(bool newGame = false)
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

            if (ship == null || newGame)
            {
                ship = new Ship(new Point(Width / 2, Height), new Point(0, 0));
                EnergyValue = ship.Energy;
            }

            gameTicker = new Timer();
            gameTicker.Interval = 100;
            gameTicker.Tick += GameTicker_Tick;
            gameTicker.Start();
            GAME_CONTROL.Start(); // Начать игру
        }

        static public void Unload()
        {
            bullets = null;
            stars = null;
            asteroids = null;
            NOW_STAR_COUNT = 0;
            NOW_ASTEROID_COUNT = 0;
            ship = null;
            gameTicker = null;
        }

        private static void GameTicker_Tick(object sender, EventArgs e)
        {
            Update();
            Draw();
        }

        static public void Draw()
        {
            buffer.Graphics.DrawImage(background, 0, 0); // Фон
            // Информационные лейблы
            Game.buffer.Graphics.DrawString(EnergyText + EnergyValue.ToString(), SystemFonts.CaptionFont, Brushes.White, 0, 0);
            Game.buffer.Graphics.DrawString(GAME_POINTS.GetPointsToString(), SystemFonts.CaptionFont, Brushes.White, 75, 0);
            Game.buffer.Graphics.DrawString(GAME_LEVELS.GetCurrentLevelToString(), SystemFonts.CaptionFont, Brushes.White, 200, 0);

            if (stars != null)
            {
                foreach (Star star in stars)
                    star?.Draw();
            }

            if (bullets != null)
            {
                foreach (Bullet bullet in bullets)
                    bullet?.Draw();
            }

            if (asteroids != null)
            {
                foreach (Asteroid asteroid in asteroids)
                    asteroid?.Draw();
            }

            ship?.Draw();
            buffer.Render();
        }

        //static public void DrawMenu()
        //{
        //    buffer.Graphics.DrawImage(background, 0, 0); // Фон
        //    Game.buffer.Graphics.DrawString("Нажмите F2 чтобы начать", SystemFonts.MenuFont, Brushes.Red, Width/2, Height/2);
        //    buffer.Render();
        //}

        // Обновить состояние игровых объектов
        static public void Update()
        {
            ship?.Update();

            // Проверка состояния пуль
            if (bullets != null) 
            { 
                foreach (Bullet bullet in bullets)
                {
                    if (bullet != null)
                    {
                        bullet.Update();
                        // Проверка на столкновение ПУЛЯ == АСТЕРОИД
                        if (asteroids != null)
                        {
                            foreach (Asteroid asteroid in asteroids)
                            {
                                if (asteroid != null)
                                {
                                    if (bullet.Collision(asteroid))
                                    {
                                        asteroid.Reset();
                                        GAME_POINTS.PointsHightUp();
                                        bullet.Reset();
                                    }
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
                                    GAME_POINTS.PointsMiddleUp();
                                    bullet.Reset();
                                }
                            }
                        }
                    }
                }
            }

            // Проверка состояния Астероидов
            if (asteroids != null)
            {
                foreach (Asteroid asteroid in asteroids)
                {
                    if (asteroid != null)
                    {
                        asteroid.Update();
                        // Проверка на столкновение АСТЕРОИД == КОРАБЛЬ
                        if (asteroid.Collision(ship))
                        {
                            asteroid.Reset(); // Сбросить  позицию астероида
                            GAME_POINTS.PointsMiddleUp();
                            ship.Damage(20);
                        }
                    }
                }
            }

            // Проверка состояния Звезд
            if (stars != null)
            {
                foreach (Star star in stars)
                {
                    if (star != null)
                    {
                        star.Update();
                        // Проверка на столкновение ЗВЕЗДА == КОРАБЛЬ
                        if (star.Collision(ship))
                        {
                            star.Reset(); // Сбросить позицию звезды
                            GAME_POINTS.PointsLowUp();
                            ship.Damage(10);
                        }
                    }
                }
            }
        }
    }
}
