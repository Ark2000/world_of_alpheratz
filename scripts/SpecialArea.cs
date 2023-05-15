namespace Alpheratz;

public partial class SpecialArea : Area2D
{
    [Export]
    LynxCamera cam;
    [Export]
    Node2D cameraFocus;
    public override void _Ready()
    {
        BodyEntered += (Node2D body) => {
            cam.target = cameraFocus;
        };
        BodyExited += (Node2D body) => {
            cam.target = body;
        };
    }
}
