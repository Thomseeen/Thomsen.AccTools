using AccTools.SharedMemory.Models;

namespace AccTools.SharedMemory {
    public class GraphicsEventArgs : EventArgs {
        public Graphics Graphics { get; private set; }

        public GraphicsEventArgs(Graphics graphics) {
            Graphics = graphics;
        }
    }
}
