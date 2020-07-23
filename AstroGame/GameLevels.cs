using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroGame
{
    static class GameLevels
    {
        static private int asteroidSpeed;
        static private int starSpeed;
        static private int bulletSpeed;
        static private double timeBeforeHealBox; // Время до аптечки (секунд)

        static public event Action LevelIsChanged; // Событие изменения уровня

        // Текущий уровень
        static public int CurrentLevel { get; private set; } 

        // Астероиды
        static public int AsteroidCount { get; private set; }
        static public int AsteroidSpeed
        {
            get
            {
                return asteroidSpeed;
            }
            private set
            {
                if (value < 30) asteroidSpeed = value;
            }
        }

        // Звезды
        static public int StarCount { get; private set; }
        static public int StarSpeed
        {
            get
            {
                return starSpeed;
            }
            private set
            {
                if (value < 50) starSpeed = value;
            }
        }

        // Пули
        static public int BulletSpeed
        {
            get
            {
                return bulletSpeed;
            }
            private set
            {
                if (value < 60) bulletSpeed = value;
            }
        }

        // Аптечка
        static public int TimeBeforeHealBoxMS
        {
            get
            {
                return (int)(timeBeforeHealBox * 1000.0);
            }
        }

        static private double TimeBeforeHealBox
        {
            get
            {
                return timeBeforeHealBox;
            }
            set
            {
                if (value > 5.0) timeBeforeHealBox = value;
            }
        }

        static GameLevels()
        {
            CurrentLevel = 1;

            AsteroidCount = 1;
            AsteroidSpeed = 5;

            StarCount = 30;
            StarSpeed = 7;

            BulletSpeed = 20;

            TimeBeforeHealBox = 60.0;

            LevelIsChanged += GameLevels_LevelIsChanged;
        }

        private static void GameLevels_LevelIsChanged()
        {
            // Изменения каждые N уровней
            if (CurrentLevel % 1 == 0)
            {
                StarCount++;
            }
            if (CurrentLevel %2 == 0)
            {
                TimeBeforeHealBox--;
            }
            if (CurrentLevel % 3 == 0)
            {
                StarSpeed++;
            }
            if (CurrentLevel % 5 == 0)
            {
                AsteroidCount++;
            }
            if (CurrentLevel % 9 == 0)
            {
                BulletSpeed++;
                AsteroidSpeed++;
            }
        }

        static public void NextLevel()
        {
            CurrentLevel++;
            LevelIsChanged?.Invoke();
        }
    }
}
