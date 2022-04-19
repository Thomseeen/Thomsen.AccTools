
using Thomsen.AccTools.SharedMemory.Models;

namespace AccTools.SharedMemory {
    public delegate void GameStatusChangedHandler(object sender, GameStatusEventArgs e);

    public class GameStatusEventArgs : EventArgs {
        public GameStatus GameStatus { get; private set; }

        public GameStatusEventArgs(GameStatus status) {
            GameStatus = status;
        }
    }
}
