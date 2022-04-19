namespace Thomsen.AccTools.SharedMemory {
    [Serializable]
    public class AccSharedMemoryException : Exception {
        public AccSharedMemoryException() { }

        public AccSharedMemoryException(string? message) : base(message) { }

        public AccSharedMemoryException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}