using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroGame
{
    class GameLevels
    {
        private double asteroidCount;
        private double asteroidSpeed;
        private double starCount;
        private double starSpeed;
        private double bulletSpeed;
        private double healboxCount;

        public static event Action LevelIsChanged; // Событие изменения уровня

        // Текущий уровень
        public int CurrentLevel { get; private set; } 

        // Астероиды
        public int AsteroidCount
        {
            get
            {
                return (int)asteroidCount;
            }
        }

        public int AsteroidSpeed
        {
            get
            {
                return (int)asteroidSpeed;
            }
        }

        // Звезды
        public int StarCount
        {
            get
            {
                return (int)starCount;
            }
        }

        public int StarSpeed
        {
            get
            {
                return (int)starSpeed;
            }
        }

        // Пули
        public int BulletSpeed
        {
            get
            {
                return (int)bulletSpeed;
            }
        }

        // Аптечка
        public int HealBoxCount
        {
            get
            {
                return (int)healboxCount;
            }
        }

        public GameLevels()
        {
            DefaultValues();
        }

        public void NextLevel()
        {
            CurrentLevel++;

            asteroidCount += 0.3;
            asteroidSpeed += 0.2;
            starCount += 1.0;
            starSpeed += 0.4;
            bulletSpeed += 0.1;
            healboxCount += 0.2;

            LevelIsChanged?.Invoke();
        }

        public string GetCurrentLevelToString()
        {
            return $"Уровень: {CurrentLevel}";
        }

        public void DefaultValues()
        {
            CurrentLevel = 1;
            asteroidCount = 1.0;
            asteroidSpeed = 6.0;
            starCount = 30.0;
            starSpeed = 7.0;
            bulletSpeed = 20.0;
            healboxCount = 1.0;
        }
    }
}
