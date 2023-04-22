using Godot;

public partial class Level1 : Node2D
{
    [Export]
    SpecialArea leftArea;
    [Export]
    Camera2D cam;

    public override void _PhysicsProcess(double delta)
    {
        // GD.Print(delta);
        if (Input.IsActionJustPressed("1"))
        {
            Vector2 mousePosition = GetGlobalMousePosition();
            GameWorld.Instance.SpawnCloudPoofEffect(mousePosition);
        }
    }

    public override void _Ready()
    {
        GameWorld.Instance.CoinCollected += () => GD.Print("[INFO] Coin collected!");

        leftArea.BodyEntered += (Node2D body) => {
            cam.LimitLeft -= 1000;
        };
        leftArea.BodyExited += (Node2D body) => {
            cam.LimitLeft += 1000;
        };
    }
}
