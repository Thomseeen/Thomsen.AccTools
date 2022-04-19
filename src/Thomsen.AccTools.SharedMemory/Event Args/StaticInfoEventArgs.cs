
using Thomsen.AccTools.SharedMemory.Models;

namespace AccTools.SharedMemory {
    public delegate void StaticInfoUpdatedHandler(object sender, StaticInfoEventArgs e);

    public class StaticInfoEventArgs : EventArgs {
        public StaticInfo StaticInfo { get; private set; }

        public StaticInfoEventArgs(StaticInfo staticInfo) {
            StaticInfo = staticInfo;
        }
    }
}
