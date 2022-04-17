using AccTools.SharedMemory.Models;

using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Timers;

namespace AccTools.SharedMemory {
    public enum ConnectionState {
        Disconnected,
        Connecting,
        Connected
    }

    public class AccSharedMemory : IDisposable {
        #region Private Fields
        private bool _disposed;

        private MemoryMappedFile? _physicsMemory;
        private MemoryMappedFile? _graphicsMemory;
        private MemoryMappedFile? _staticInfoMemory;

        private GameStatus _gameStatus = GameStatus.OFF;
        private readonly System.Timers.Timer _physicsTimer = new(1000);
        private readonly System.Timers.Timer _graphicsTimer = new(10);
        private readonly System.Timers.Timer _staticInfoTimer = new(1000);
        #endregion Private Fields

        #region Properties
        public ConnectionState Status { get; private set; } = ConnectionState.Disconnected;

        public double PhysicsUpdateInterval {
            get => _physicsTimer.Interval;
            set => _physicsTimer.Interval = value;
        }

        public double GraphicsUpdateInterval {
            get => _graphicsTimer.Interval;
            set => _graphicsTimer.Interval = value;
        }

        public double StaticInfoUpdateInterval {
            get => _staticInfoTimer.Interval;
            set => _staticInfoTimer.Interval = value;
        }
        #endregion Properties

        #region Delegates
        #endregion Delegates

        #region Events
        public event PhysicsUpdatedHandler? PhysicsUpdated;

        public event GraphicsUpdatedHandler? GraphicsUpdated;

        public event StaticInfoUpdatedHandler? StaticInfoUpdated;

        public event GameStatusChangedHandler? GameStatusChanged;
        #endregion Events

        #region Constructors
        public AccSharedMemory() { }

        public AccSharedMemory(double physicsUpdateInterval, double graphicsUpdateInterval, double staticInfoUpdateInterval) {
            PhysicsUpdateInterval = physicsUpdateInterval;
            GraphicsUpdateInterval = graphicsUpdateInterval;
            StaticInfoUpdateInterval = staticInfoUpdateInterval;
        }
        #endregion Constructors

        #region Public Methods
        public async Task ConnectAsync(CancellationToken token) {
            if (_disposed) {
                throw new ObjectDisposedException(nameof(AccSharedMemory));
            }

            if (Status == ConnectionState.Connected) {
                return;
            }

            if (Status == ConnectionState.Connecting) {
                throw new InvalidOperationException("Already connecting.");
            }

            Status = ConnectionState.Connecting;

            while (Status != ConnectionState.Connected) {
                try {
                    _physicsMemory = MemoryMappedFile.OpenExisting("Local\\acpmf_physics");
                    _graphicsMemory = MemoryMappedFile.OpenExisting("Local\\acpmf_graphics");
                    _staticInfoMemory = MemoryMappedFile.OpenExisting("Local\\acpmf_static");

                    Status = ConnectionState.Connected;

                    Physics physics = ReadMemory<Physics>(_physicsMemory);
                    OnPhysicsUpdated(physics);

                    Graphics graphics = ReadMemory<Graphics>(_graphicsMemory);
                    OnGraphicsUpdated(graphics);

                    StaticInfo staticInfo = ReadMemory<StaticInfo>(_staticInfoMemory);
                    OnStaticInfoUpdated(staticInfo);
                } catch (FileNotFoundException) {
                    try {
                        await Task.Delay(100, token);
                    } catch (TaskCanceledException) {
                        return;
                    }
                }
            }

            _physicsTimer.Elapsed += PhysicsTimer_Elapsed;
            _graphicsTimer.Elapsed += GraphicsTimer_Elapsed;
            _staticInfoTimer.Elapsed += StaticInfoTimer_Elapsed;

            _physicsTimer.Start();
            _graphicsTimer.Start();
            _staticInfoTimer.Start();
        }

        public void Disconnect() {
            if (_disposed) {
                throw new ObjectDisposedException(nameof(AccSharedMemory));
            }

            if (Status == ConnectionState.Disconnected) {
                return;
            }

            if (Status == ConnectionState.Connecting) {
                throw new InvalidOperationException("Can't disconnect while connecting.");
            }

            Status = ConnectionState.Disconnected;

            _physicsTimer.Elapsed -= PhysicsTimer_Elapsed;
            _graphicsTimer.Elapsed -= GraphicsTimer_Elapsed;
            _staticInfoTimer.Elapsed -= StaticInfoTimer_Elapsed;

            _physicsTimer.Stop();
            _graphicsTimer.Stop();
            _staticInfoTimer.Stop();

            _physicsMemory?.Dispose();
            _graphicsMemory?.Dispose();
            _staticInfoMemory?.Dispose();
        }
        #endregion Public Methods

        #region Private Methods
        private static T ReadMemory<T>(MemoryMappedFile memory) {
            using MemoryMappedViewStream? stream = memory.CreateViewStream();
            using BinaryReader reader = new(stream);

            int size = Marshal.SizeOf(typeof(T));
            byte[]? bytes = reader.ReadBytes(size);
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            try {
                T? data = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());

                if (data is null) {
                    throw new InvalidOperationException("Failed to read data from memory");
                }

                return data;
            } finally {
                handle.Free();
            }
        }

        private void OnPhysicsUpdated(Physics physics) {
            PhysicsUpdated?.Invoke(this, new PhysicsEventArgs(physics));
        }

        private void OnGraphicsUpdated(Graphics graphics) {
            if (graphics.Status != _gameStatus) {
                _gameStatus = graphics.Status;
                GameStatusChanged?.Invoke(this, new GameStatusEventArgs(graphics.Status));
            }

            GraphicsUpdated?.Invoke(this, new GraphicsEventArgs(graphics));
        }

        private void OnStaticInfoUpdated(StaticInfo staticInfo) {
            StaticInfoUpdated?.Invoke(this, new StaticInfoEventArgs(staticInfo));
        }
        #endregion Private Methods

        #region Event Handler
        private void PhysicsTimer_Elapsed(object? sender, ElapsedEventArgs e) {
            if (Status != ConnectionState.Connected || _physicsMemory is null) {
                return;
            }

            Physics physics = ReadMemory<Physics>(_physicsMemory);
            OnPhysicsUpdated(physics);
        }

        private void GraphicsTimer_Elapsed(object? sender, ElapsedEventArgs e) {
            if (Status != ConnectionState.Connected || _graphicsMemory is null) {
                return;
            }

            Graphics graphics = ReadMemory<Graphics>(_graphicsMemory);
            OnGraphicsUpdated(graphics);
        }

        private void StaticInfoTimer_Elapsed(object? sender, ElapsedEventArgs e) {
            if (Status != ConnectionState.Connected || _staticInfoMemory is null) {
                return;
            }

            StaticInfo staticInfo = ReadMemory<StaticInfo>(_staticInfoMemory);
            OnStaticInfoUpdated(staticInfo);
        }
        #endregion Event Handler

        #region IDisposable
        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    if (Status == ConnectionState.Connected) {
                        Disconnect();
                    }

                    _physicsTimer.Dispose();
                    _graphicsTimer.Dispose();
                    _staticInfoTimer.Dispose();

                    _physicsMemory?.Dispose();
                    _graphicsMemory?.Dispose();
                    _staticInfoMemory?.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable
    }
}
