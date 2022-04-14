using AccTools.SharedMemory.Models;

namespace AccTools.SharedMemory {
    public class StaticInfoEventArgs : EventArgs {
        public StaticInfo StaticInfo { get; private set; }

        public StaticInfoEventArgs(StaticInfo staticInfo) {
            StaticInfo = staticInfo;
        }
    }
}
