# Example
An example program is available at [Thomsen.AccTools.Tester](https://github.com/Thomseeen/Thomsen.AccTools/tree/master/src/Thomsen.AccTools.Tester).

```c#
private static async Task Main(string[] args) {
    CancellationTokenSource cts = new();

    // Initialize the API
    using AccSharedMemory acc = new();

    // Subscribe the event for GameStatusChanged and write some example data to stdout
    acc.GameStatusChanged += (sender, e) => {
        Console.WriteLine($"Game status changed to {e.GameStatus}.");
    };

    // Subscribe the event for StaticInfoUpdated and write some example data to stdout
    acc.StaticInfoUpdated += (sender, e) => {
        Console.WriteLine($"Static info updated: {e.StaticInfo.CarModel} on {e.StaticInfo.Track}.");
    };
    
    // Subscribe the event for PhysicsUpdated and write some example data to stdout
    acc.PhysicsUpdated += (sender, e) => {
        Console.WriteLine($"Physics updated: Speed: {e.Physics.SpeedKmh}.");
    };

    // Subscribe the event for GraphicsUpdated and write some example data to stdout
    acc.GraphicsUpdated += (sender, e) => {
        Console.WriteLine($"Graphics updated: In Pits: {e.Graphics.IsInPit}.");
    };

    // Subscribe to ctrl+c event on the console to cancel the program
    Console.CancelKeyPress += (sender, e) => {
        Console.WriteLine("Cancelling...");

        cts.Cancel();
    };

    // Wait for connection to the game (game startup)
    Console.WriteLine("Connecting...");

    await acc.ConnectAsync(cts.Token);

    Console.WriteLine("... Connected");
    
    // Wait till program canceled
    if (!cts.Token.IsCancellationRequested) {
        cts.Token.WaitHandle.WaitOne();
    }
}
```

There are 4 events on the API to listen to. Each events supplies different information from the game.
If you are searching for some specific value, simply search through the `GameStatus`, `StaticInfo`, `Physics` and `Graphics` classes.

You can change the intervals (in milliseconds) at which each of the data structures is checked in the shared memory (and hence the frequency at which the event are fired), by i.e. setting:
```c#
acc.PhysicsUpdateInterval = 1000;
```