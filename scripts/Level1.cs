using Godot;

public partial class Level1 : Node2D
{
    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("1"))
        {
            Vector2 mousePosition = GetGlobalMousePosition();
            GameWorld.Instance.SpawnCloudPoofEffect(mousePosition);
        }
    }

    public override void _Ready()
    {
        GameWorld.Instance.CoinCollected += () => GD.Print("[INFO] Coin collected!");
    }
}
