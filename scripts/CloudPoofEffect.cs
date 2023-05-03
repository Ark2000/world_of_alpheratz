using Godot;

public partial class CloudPoofEffect : AnimatedSprite2D
{
	public override void _Ready()
	{
		GameWorld.Instance.PlaySFX("res://sounds/sfx_sounds_impact11.wav");
		AnimationFinished += () => {
			GD.Print("[INFO] " + Name + " animation finished");
			QueueFree();
		};
	}
}
