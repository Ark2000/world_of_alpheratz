using Godot;

public partial class TitleScene : Control
{
	[Export]
	PlatformerPlayer player;
	[Export]
	GameMenu menu;
	[Export]
	Control logo;

	public override void _Ready()
	{
		player.scriptedInput = true;
		player.x_input = 1.0f;
		player.PlayerStateChanged += (PlayerState oldState, PlayerState newState) => {
			if (oldState == PlayerState.Falling)
			{
				menu.ShowMenu(menu.topMenu);
				CreateTween().TweenProperty(logo, "modulate:a", 1.0f, 0.5f);
			}
		};
		logo.Modulate = logo.Modulate with {A = 0};


		CreateTween().SetLoops().TweenCallback(Callable.From(() => {
			if (GD.Randf() > 0.5f)
			{
				player.emoBubble.PlayEmo(3);
			}
		})).SetDelay(5.0f);

	}

}
