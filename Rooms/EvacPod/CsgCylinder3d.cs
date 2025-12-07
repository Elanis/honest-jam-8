using Godot;

using HonestJam8.Narration;

public partial class CsgCylinder3d : CsgCylinder3D {
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        AddUserSignal("PlayerTrigger");
        Connect("PlayerTrigger", new Godot.Callable(this, nameof(PlayerTrigger)));
    }

    private void PlayerTrigger() {
        var narrationManager = GetNode<NarrationManager>("../../NarrationManager");
        narrationManager.OnButtonTriggered(ButtonName.EvacPod);
    }
}
