using Godot;

public partial class FirstScene : Control
{
	public override void _Ready()
	{
		CreateTween().TweenCallback(Callable.From(() => {
		GameWorld.Instance.ChangeScene("res://scenes/title_scene.tscn", 0.5f);
		})).SetDelay(1.0f);
		
	}
}
