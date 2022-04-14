using AccTools.SharedMemory.Models;

namespace AccTools.SharedMemory {
    public delegate void PhysicsUpdatedHandler(object sender, PhysicsEventArgs e);

    public class PhysicsEventArgs : EventArgs {
        public Physics Physics { get; private set; }

        public PhysicsEventArgs(Physics physics) {
            Physics = physics;
        }
    }
}
