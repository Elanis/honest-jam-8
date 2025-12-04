using Godot;

using System;

public partial class Player : CharacterBody3D {
    [Export] public float Speed = 5.0f;
    [Export] public float JumpVelocity = 4.5f;
    [Export] public float MouseSensitivity = 0.05f;   // tweak to taste
    [Export] public float MaxPitchDegrees = 40f;    // prevent looking straight up/down

    // Nodes
    private Node3D _cameraPivot;
    private Camera3D _camera;

    // Internal state
    private Vector2 _rotationDeg; // x = yaw, y = pitch (both in degrees)

    public override void _Ready() {
        // Grab references
        _cameraPivot = GetNode<Node3D>("CameraPivot");
        _camera = _cameraPivot.GetNode<Camera3D>("Camera3D");

        // Hide and capture the mouse cursor
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event) {
        // Handle mouse motion
        if (@event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured) {
            // Apply sensitivity and invert Y if you like (here Y is inverted)
            _rotationDeg.X -= mouseMotion.Relative.X * MouseSensitivity;
            _rotationDeg.Y -= mouseMotion.Relative.Y * MouseSensitivity;

            // Clamp pitch
            _rotationDeg.Y = Mathf.Clamp(_rotationDeg.Y, -MaxPitchDegrees, MaxPitchDegrees);

            // Apply rotations
            // Yaw rotates the whole player (around Y axis)
            RotationDegrees = new Vector3(0, _rotationDeg.X, 0);

            // Pitch rotates only the camera pivot (around X axis)
            _cameraPivot.RotationDegrees = new Vector3(_rotationDeg.Y, 0, 0);
        }
    }

    public override void _PhysicsProcess(double delta) {
        Vector3 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor()) {
            velocity += GetGravity() * (float)delta;
        }

        // Handle Jump.
        if (Input.IsActionJustPressed("jump") && IsOnFloor()) {
            velocity.Y = JumpVelocity;
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero) {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        } else {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
