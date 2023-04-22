using Godot;

public partial class CameraProxy : Node2D
{
    [Export]
    public Node2D target;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        Position = Position.Lerp(target.Position, 0.2f);
    }
}
