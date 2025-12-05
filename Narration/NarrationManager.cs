using Godot;

namespace HonestJam8.Narration {

    public partial class NarrationManager : Node {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta) {
        }

        [Signal]
        public delegate void SpawnCondemnedEventHandler();

        public void OnAreaTriggered(AreaName areaName) {
            var test = "";
        }
    }
}
