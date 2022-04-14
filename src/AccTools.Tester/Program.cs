using AccTools.SharedMemory;

namespace AccTools.Tester {
    internal class Program {
        private static AutoResetEvent _isStopped = new(false);

        private static async Task<int> Main(string[] args) {
            AccSharedMemory acc = new();

            acc.GameStatusChanged += (sender, e) => {
                Console.WriteLine($"Game status changed to {e.GameStatus}.");
            };

            acc.StaticInfoUpdated += (sender, e) => {
                Console.WriteLine($"Static info updated. {e.StaticInfo.CarModel} on {e.StaticInfo.Track}.");
            };

            while (acc.Status != ConnectionState.Connected) {
                try {
                    acc.Connect();
                } catch (AccSharedMemoryException) {
                    await Task.Delay(100);
                }
            }

            Console.WriteLine("Connected.");

            Console.CancelKeyPress += (sender, e) => {
                acc.Disconnect();

                Console.WriteLine("Disconnected.");

                _isStopped.Set();
            };

            _isStopped.WaitOne();

            return 0;
        }
    }
}