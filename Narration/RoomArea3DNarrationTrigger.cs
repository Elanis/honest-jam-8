using Godot;

namespace HonestJam8.Narration {
    public partial class RoomArea3DNarrationTrigger : Area3D {
        protected void OnAreaEntered(AreaName areaName) {
            var narrationManager = GetNode<NarrationManager>("../../NarrationManager");
            narrationManager.OnAreaTriggered(areaName);
        }
    }
}
