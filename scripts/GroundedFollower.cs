using Godot;

// For camera usage. Follows the player, but only on the Y axis when the player is on the floor.
public partial class GroundedFollower : Node2D
{

	[Export]
	public CharacterBody2D target;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

		Vector2 newPosition = Position;

		// Always follow X
		newPosition.X = target.Position.X;
		
		// Only follow Y when target is on floor
		if (target.IsOnFloor())
		{
			newPosition.Y = target.Position.Y;
		}

		Position = newPosition;
    }

}
