using AccTools.SharedMemory.Models;

using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AccTools.SharedMemory {
    public enum ConnectionState {
        Disconnected,
        Connecting,
        Connected
    }

    public class AccSharedMemory {
        #region Private Fields
        private ConnectionState _status = ConnectionState.Disconnected;

        private MemoryMappedFile? _physicsMemory;
        private MemoryMappedFile? _graphicsMemory;
        private MemoryMappedFile? _staticInfoMemory;
        #endregion Private Fields

        #region Properties
        public TimeSpan PhysicsUpdateInterval { get; set; }

        public TimeSpan GraphicsUpdateInterval { get; set; }

        public TimeSpan StaticInfoUpdateInterval { get; set; }
        #endregion Properties

        #region Delegates
        public delegate void PhysicsUpdatedHandler(object sender, PhysicsEventArgs e);
        public delegate void GraphicsUpdatedHandler(object sender, GraphicsEventArgs e);
        public delegate void StaticInfoUpdatedHandler(object sender, StaticInfoEventArgs e);
        public delegate void GameStatusChangedHandler(object sender, GameStatusEventArgs e);
        #endregion Delegates

        #region Constructors
        public AccSharedMemory() { }
        public AccSharedMemory(TimeSpan physicsUpdateInterval, TimeSpan graphicsUpdateInterval, TimeSpan staticInfoUpdateInterval) {
            PhysicsUpdateInterval = physicsUpdateInterval;
            GraphicsUpdateInterval = graphicsUpdateInterval;
            StaticInfoUpdateInterval = staticInfoUpdateInterval;
        }
        #endregion Constructors

        #region Public Methods
        #endregion Public Methods

        #region Private Methods
        private void Connect() {
            if (_status != ConnectionState.Disconnected) {
                return;
            }

            _status = ConnectionState.Connecting;

            _physicsMemory = MemoryMappedFile.OpenExisting("Local\\acpmf_physics");
            _graphicsMemory = MemoryMappedFile.OpenExisting("Local\\acpmf_graphics");
            _staticInfoMemory = MemoryMappedFile.OpenExisting("Local\\acpmf_static");

            var physics = ReadMemory<Physics>(_physicsMemory);
            var graphics = ReadMemory<Graphics>(_graphicsMemory);
            var staticInfo = ReadMemory<StaticInfo>(_staticInfoMemory);

            _status = ConnectionState.Connected;
        }

        private T ReadMemory<T>(MemoryMappedFile memory) {
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
        #endregion Private Methods
    }
}
