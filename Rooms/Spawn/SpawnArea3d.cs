using Godot;

using HonestJam8.Narration;

namespace HonestJam8.Rooms {
    public partial class SpawnArea3d : RoomArea3DNarrationTrigger {
        private void _OnAreaEntered(Area3D _area3D) {
            OnAreaEntered(AreaName.Spawn);
        }
    }
}
