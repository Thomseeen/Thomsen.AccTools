using AccTools.SharedMemory;

using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Timers;

using Thomsen.AccTools.SharedMemory.Models;

namespace Thomsen.AccTools.SharedMemory;

public enum ConnectionState {
    Disconnected,
    Connecting,
    Connected
}

/// <summary>
/// API to interface with AC and ACC
/// </summary>
public sealed class AccSharedMemory : IDisposable {
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
    /// <summary>
    /// Connection State of the API to the game
    /// </summary>
    public ConnectionState Status { get; private set; } = ConnectionState.Disconnected;

    /// <summary>
    /// Interval in milliseconds at which the Physics structure is checked and the matching event is fired
    /// </summary>
    public double PhysicsUpdateInterval {
        get => _physicsTimer.Interval;
        set => _physicsTimer.Interval = value;
    }

    /// <summary>
    /// Interval in milliseconds at which the Graphics structure is checked and the matching event is fired
    /// </summary>
    public double GraphicsUpdateInterval {
        get => _graphicsTimer.Interval;
        set => _graphicsTimer.Interval = value;
    }

    /// <summary>
    /// Interval in milliseconds at which the StaticInfo structure is checked and the matching event is fired
    /// </summary>
    public double StaticInfoUpdateInterval {
        get => _staticInfoTimer.Interval;
        set => _staticInfoTimer.Interval = value;
    }
    #endregion Properties

    #region Delegates
    #endregion Delegates

    #region Events
    /// <summary>
    /// Event for Physics updates
    /// </summary>
    public event PhysicsUpdatedHandler? PhysicsUpdated;

    /// <summary>
    /// Event for Graphics updates
    /// </summary>
    public event GraphicsUpdatedHandler? GraphicsUpdated;

    /// <summary>
    /// Event for StaticInfo updates
    /// </summary>
    public event StaticInfoUpdatedHandler? StaticInfoUpdated;

    /// <summary>
    /// Event for GameStatus updates
    /// </summary>
    public event GameStatusChangedHandler? GameStatusChanged;
    #endregion Events

    #region Constructors
    /// <summary>
    /// Creates a new instance of the API using defaults for the update intervals
    /// </summary>
    public AccSharedMemory() { }

    /// <summary>
    /// Creates a new instance of the API using the given update intervals
    /// </summary>
    /// <param name="physicsUpdateInterval"></param>
    /// <param name="graphicsUpdateInterval"></param>
    /// <param name="staticInfoUpdateInterval"></param>
    public AccSharedMemory(double physicsUpdateInterval, double graphicsUpdateInterval, double staticInfoUpdateInterval) {
        PhysicsUpdateInterval = physicsUpdateInterval;
        GraphicsUpdateInterval = graphicsUpdateInterval;
        StaticInfoUpdateInterval = staticInfoUpdateInterval;
    }
    #endregion Constructors

    #region Public Methods
    /// <summary>
    /// Asynchronously waits for the game to be started and then try to connect to its shared memory
    /// </summary>
    /// <param name="token">Cancel token to cancel the connection attempt</param>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException">If the API object has already been disposed</exception>
    /// <exception cref="InvalidOperationException">If the API is already connected</exception>
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

    /// <summary>
    /// Disconnects the API from the game
    /// </summary>
    /// <exception cref="ObjectDisposedException">If the API object has already been disposed</exception>
    /// <exception cref="InvalidOperationException">If the API is trying to connect, cancel the connection attempt first</exception>
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
    /// <summary>
    /// Dispose internals like timers and shared memory handlers
    /// </summary>
    public void Dispose() {
        if (!_disposed) {
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

        GC.SuppressFinalize(this);
    }
    #endregion IDisposable
}
