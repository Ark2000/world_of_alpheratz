using Godot;

// Hurt player.
// Teleport player to a specific location.

public partial class Cliff : Area2D
{
    [Export]
    public Node2D respawnLocation;

	public override void _Ready()
	{
        BodyEntered += (Node2D body) => {
            PlatformerPlayer player = body as PlatformerPlayer;
            player.GlobalPosition = respawnLocation.GlobalPosition;
            player.Hurt(0.0f, 100.0f);
        };
	}
}
