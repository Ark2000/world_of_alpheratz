using Godot;

public partial class CloudPoofEffect : AnimatedSprite2D
{
	public override void _Ready()
	{
		AnimationFinished += () => {
			GD.Print("[INFO] " + Name + " animation finished");
			QueueFree();
		};
	}
}
