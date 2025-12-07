using Godot;

using System;

public partial class Player : CharacterBody3D {
    [Export] public float Speed = 5.0f;
    [Export] public float JumpVelocity = 4.5f;
    [Export] public float MouseSensitivity = 0.05f;   // tweak to taste
    [Export] public float MaxPitchDegrees = 40f;    // prevent looking straight up/down
    [Export] public float CrouchingCameraHeightChange = 1f;

    private Node3D _cameraPivot;
    private Camera3D _camera;
    private CollisionShape3D _collisionShape3D;
    private bool isCrouching = false;
    private CapsuleShape3D _capsuleShape;

    private float _standingHeight;
    private float _crouchHeight;
    private Vector2 _rotationDeg; // x = yaw, y = pitch (both in degrees)

    public override void _Ready() {
        // Grab references
        _cameraPivot = GetNode<Node3D>("CameraPivot");
        _camera = _cameraPivot.GetNode<Camera3D>("Camera3D");
        _collisionShape3D = GetNode<CollisionShape3D>("CollisionShape3D");
        var shape = _collisionShape3D.Shape as CapsuleShape3D;
        if (shape == null)
            GD.PrintErr("Expected a CapsuleShape3D on the CollisionShape3D!");

        _capsuleShape = shape;
        _standingHeight = shape.Height;
        _crouchHeight = _standingHeight * 0.5f;

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
        if (Input.IsActionJustPressed("escape")) {
            GetTree().Quit();
            return;
        }

        // Handle action.
        if (Input.IsActionJustPressed("action")) {
            var raycast3D = GetNode<RayCast3D>("CameraPivot/RayCast3D");
            if (raycast3D.IsColliding()) {
                try {
                    var collider = raycast3D.GetCollider() as StaticBody3D;
                    var parent = collider.GetParent<GeometryInstance3D>();
                    if (parent.HasUserSignal("PlayerTrigger")) {
                        parent.EmitSignal("PlayerTrigger");
                    }
                } catch (Exception e) { GD.PrintErr(e.Message); }
            }
        }

        Vector3 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor()) {
            velocity += GetGravity() * (float)delta;
        }

        // Handle Jump.
        if (Input.IsActionJustPressed("jump") && IsOnFloor()) {
            velocity.Y = JumpVelocity;
        }

        // Crouch
        if (Input.IsActionPressed("crouch") && IsOnFloor()) {
            if (!isCrouching) {
                isCrouching = true;
                _capsuleShape.Height = _crouchHeight;
                _collisionShape3D.Position = new Vector3(0, _crouchHeight / 2f, 0);
                _cameraPivot.Position = new Vector3(_cameraPivot.Position.X, _cameraPivot.Position.Y - CrouchingCameraHeightChange, _cameraPivot.Position.Z);
            }
        } else {
            if (isCrouching) {
                isCrouching = false;
                _capsuleShape.Height = _standingHeight;
                _collisionShape3D.Position = new Vector3(0, _standingHeight / 2f, 0);
                _cameraPivot.Position = new Vector3(_cameraPivot.Position.X, _cameraPivot.Position.Y + CrouchingCameraHeightChange, _cameraPivot.Position.Z);
            }
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
