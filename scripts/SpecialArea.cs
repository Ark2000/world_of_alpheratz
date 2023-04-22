using Godot;

public partial class SpecialArea : Area2D
{
    [Export]
    CameraProxy cameraProxy;
    [Export]
    Node2D cameraFocus;
    public override void _Ready()
    {
        BodyEntered += (Node2D body) => {
            cameraProxy.target = cameraFocus;
        };
        BodyExited += (Node2D body) => {
            cameraProxy.target = body;
        };
    }
}
