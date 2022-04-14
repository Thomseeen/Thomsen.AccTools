using AccTools.SharedMemory.Models;

namespace AccTools.SharedMemory {
    public class PhysicsEventArgs : EventArgs {
        public Physics Physics { get; private set; }

        public PhysicsEventArgs(Physics physics) {
            Physics = physics;
        }
    }
}
