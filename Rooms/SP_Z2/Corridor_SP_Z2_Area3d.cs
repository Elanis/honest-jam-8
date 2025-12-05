using Godot;

using HonestJam8.Narration;

namespace HonestJam8.Rooms {
    public partial class Corridor_SP_Z2_Area3d : RoomArea3DNarrationTrigger {
        private void _OnAreaEntered(Area3D _area3D) {
            OnAreaEntered(AreaName.Corridor_SP_Z2);
        }
    }
}
