
using Thomsen.AccTools.SharedMemory;

namespace Thomsen.AccTools.Tester {
    internal class Program {
        private static async Task Main(string[] args) {
            CancellationTokenSource cts = new();

            using AccSharedMemory acc = new();

            acc.GameStatusChanged += (sender, e) => {
                Console.WriteLine($"Game status changed to {e.GameStatus}.");
            };

            acc.StaticInfoUpdated += (sender, e) => {
                Console.WriteLine($"Static info updated. {e.StaticInfo.CarModel} on {e.StaticInfo.Track}.");
            };

            Console.CancelKeyPress += (sender, e) => {
                Console.WriteLine("Cancelling...");

                cts.Cancel();
            };

            Console.WriteLine("Connecting...");

            await acc.ConnectAsync(cts.Token);

            Console.WriteLine("... Connected");

            if (!cts.Token.IsCancellationRequested) {
                cts.Token.WaitHandle.WaitOne();
            }

            Environment.Exit(0);
        }
    }
}