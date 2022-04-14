namespace AccTools.SharedMemory {
    #endregion Delegates

    public class AssettoCorsaNotStartedException : Exception {
        public AssettoCorsaNotStartedException()
            : base("Shared Memory not connected, is Assetto Corsa running and have you run assettoCorsa.Start()?") {
        }
    }
}
