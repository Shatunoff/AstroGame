using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroGame
{
    public enum GameState
    {
        PLAYING, RESTARTING, PAUSE, STOP
    }

    class GameControl
    {
        public static event Action GameStateChanged;
        public GameState GameState { get; private set; }

        public void Start()
        {
            GameState = GameState.PLAYING;
            GameStateChanged?.Invoke();
        }

        public void Pause()
        {
            GameState = GameState.PAUSE;
            GameStateChanged?.Invoke();
        }

        public void Stop()
        {
            GameState = GameState.STOP;
            GameStateChanged?.Invoke();
        }

        public void Restart()
        {
            GameState = GameState.RESTARTING;
            GameStateChanged?.Invoke();
        }
    }
}
