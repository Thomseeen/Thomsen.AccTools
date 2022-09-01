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
    private bool _disposed;

    private readonly PeriodicSharedMemoryPoller<Physics> _physics;
    private readonly PeriodicSharedMemoryPoller<Graphics> _graphics;
    private readonly PeriodicSharedMemoryPoller<StaticInfo> _staticInfo;

    /// <summary>
    /// Connection State of the API to the game
    /// </summary>
    public ConnectionState Status { get; private set; } = ConnectionState.Disconnected;

    /// <summary>
    /// Interval in milliseconds at which the Physics structure is checked and the matching event is fired
    /// </summary>
    public double PhysicsUpdateInterval {
        get => _physics.Interval;
        set => _physics.Interval = value;
    }

    /// <summary>
    /// Interval in milliseconds at which the Graphics structure is checked and the matching event is fired
    /// </summary>
    public double GraphicsUpdateInterval {
        get => _graphics.Interval;
        set => _graphics.Interval = value;
    }

    /// <summary>
    /// Interval in milliseconds at which the StaticInfo structure is checked and the matching event is fired
    /// </summary>
    public double StaticInfoUpdateInterval {
        get => _staticInfo.Interval;
        set => _staticInfo.Interval = value;
    }

    /// <summary>
    /// Event for Physics updates
    /// </summary>
    public event UpdatedHandler<Physics>? PhysicsUpdated {
        add => _physics.Updated += value;
        remove => _physics.Updated -= value;
    }

    /// <summary>
    /// Event for Graphics updates
    /// </summary>
    public event UpdatedHandler<Graphics>? GraphicsUpdated {
        add => _graphics.Updated += value;
        remove => _graphics.Updated -= value;
    }

    /// <summary>
    /// Event for StaticInfo updates
    /// </summary>
    public event UpdatedHandler<StaticInfo>? StaticInfoUpdated {
        add => _staticInfo.Updated += value;
        remove => _staticInfo.Updated -= value;
    }

    /// <summary>
    /// Creates a new instance of the API using defaults for the update intervals
    /// </summary>
    public AccSharedMemory() {
        _physics = new PeriodicSharedMemoryPoller<Physics>("Local\\acpmf_physics");
        _graphics = new PeriodicSharedMemoryPoller<Graphics>("Local\\acpmf_graphics");
        _staticInfo = new PeriodicSharedMemoryPoller<StaticInfo>("Local\\acpmf_static");
    }

    /// <summary>
    /// Creates a new instance of the API using the given update intervals
    /// </summary>
    /// <param name="physicsUpdateInterval"></param>
    /// <param name="graphicsUpdateInterval"></param>
    /// <param name="staticInfoUpdateInterval"></param>
    public AccSharedMemory(double physicsUpdateInterval, double graphicsUpdateInterval, double staticInfoUpdateInterval) : this() {
        PhysicsUpdateInterval = physicsUpdateInterval;
        GraphicsUpdateInterval = graphicsUpdateInterval;
        StaticInfoUpdateInterval = staticInfoUpdateInterval;
    }

    /// <summary>
    /// Asynchronously waits for the game to be started and then try to connect to its shared memory
    /// </summary>
    /// <param name="token">Cancel token to cancel the connection attempt</param>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException">If the API object has already been disposed</exception>
    /// <exception cref="InvalidOperationException">A shared memory file failed to open</exception>
    public async Task ConnectAsync(CancellationToken token) {
        if (_disposed) {
            throw new ObjectDisposedException(nameof(AccSharedMemory));
        }

        if (Status == ConnectionState.Connected) {
            return;
        }

        if (Status == ConnectionState.Connecting) {
            return;
        }

        Status = ConnectionState.Connecting;

        while (Status != ConnectionState.Connected) {
            try {
                _physics.Connect();
                _graphics.Connect();
                _staticInfo.Connect();

                Status = ConnectionState.Connected;
            } catch (FileNotFoundException) {
                try {
                    await Task.Delay(100, token);
                } catch (OperationCanceledException) {
                    return;
                }
            }
        }
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

        _physics.Disconnect();
        _graphics.Disconnect();
        _staticInfo.Disconnect();
    }

    /// <summary>
    /// Dispose internals like timers and shared memory handlers
    /// </summary>
    public void Dispose() {
        if (!_disposed) {
            if (Status == ConnectionState.Connected) {
                Disconnect();
            }

            _physics.Dispose();
            _graphics.Dispose();
            _staticInfo.Dispose();
        }

        _disposed = true;

        GC.SuppressFinalize(this);
    }
}
