using Godot;

public partial class Corridor_SP_Z2_Area3d : Area3D {
    private void _OnAreaEntered(Area3D area3D) {
        var narrationManager = GetNode<NarrationManager>("../../NarrationManager");
        narrationManager.OnAreaTriggered(HonestJam8.AreaName.Corridor_SP_Z2);
    }
}
