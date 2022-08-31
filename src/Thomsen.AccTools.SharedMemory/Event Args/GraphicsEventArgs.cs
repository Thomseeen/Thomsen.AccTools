
using Thomsen.AccTools.SharedMemory.Models;

namespace AccTools.SharedMemory;

public delegate void GraphicsUpdatedHandler(object sender, GraphicsEventArgs e);

public class GraphicsEventArgs : EventArgs {
    public Graphics Graphics { get; private set; }

    public GraphicsEventArgs(Graphics graphics) {
        Graphics = graphics;
    }
}
