using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroGame
{
    class GameLevels
    {
        private int asteroidSpeed;
        private int starSpeed;
        private int bulletSpeed;

        public event Action LevelIsChanged; // Событие изменения уровня

        // Текущий уровень
        public int CurrentLevel { get; private set; } 

        // Астероиды
        public int AsteroidCount { get; private set; }
        public int AsteroidSpeed
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
        public int StarCount { get; private set; }
        public int StarSpeed
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
        public int BulletSpeed
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
        public int HealBoxCount { get; set; }

        public GameLevels()
        {
            CurrentLevel = 1;

            AsteroidCount = 1;
            AsteroidSpeed = 6;

            StarCount = 30;
            StarSpeed = 7;

            BulletSpeed = 20;

            HealBoxCount = 1;

            LevelIsChanged += GameLevels_LevelIsChanged;
        }

        private void GameLevels_LevelIsChanged()
        {
            // Изменения каждые N уровней
            if (CurrentLevel % 1 == 0)
            {
                StarCount++;
            }
            if (CurrentLevel % 5 == 0)
            {
                AsteroidCount++;
                StarSpeed++;
            }
            if (CurrentLevel % 9 == 0)
            {
                BulletSpeed++;
                AsteroidSpeed++;
                HealBoxCount++;
            }
        }

        public void NextLevel()
        {
            CurrentLevel++;
            LevelIsChanged?.Invoke();
        }

        public string GetCurrentLevelToString()
        {
            return $"Уровень: {CurrentLevel}";
        }
    }
}
