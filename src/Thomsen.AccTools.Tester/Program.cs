
using Thomsen.AccTools.SharedMemory;

namespace Thomsen.AccTools.Tester;

internal class Program {
    private static async Task Main(string[] args) {
        CancellationTokenSource cts = new();

        // Initialize the API
        using AccSharedMemory acc = new();

        // Subscribe the event for StaticInfoUpdated and write some example data to stdout
        acc.StaticInfoUpdated += (sender, e) => {
            Console.WriteLine($"Static info updated: {e.Data.CarModel} on {e.Data.Track}.");
        };

        // Subscribe the event for PhysicsUpdated and write some example data to stdout
        acc.PhysicsUpdated += (sender, e) => {
            Console.WriteLine($"Physics updated: Speed: {e.Data.SpeedKmh}.");
        };

        // Subscribe the event for GraphicsUpdated and write some example data to stdout
        acc.GraphicsUpdated += (sender, e) => {
            Console.WriteLine($"Graphics updated: In Pits: {e.Data.IsInPit}, In Pit Lane: {e.Data.IsInPitLane}, Flag: {e.Data.Flag}, LineOn: {e.Data.IdealLineOn}, Cars: {e.Data.ActiveCars}.");
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

        Environment.Exit(0);
    }
}