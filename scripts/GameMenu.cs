using Godot;
using System;

public partial class GameMenu : Node
{
	[Export]
	Button startButton;
	[Export]
	Button settingsButton;
	[Export]
	Button aboutButton;
	[Export]
	Button quitButton;

    public override void _Ready()
    {
        base._Ready();

		quitButton.Pressed += () => GetTree().Quit();
		
		startButton.Pressed += () => GameWorld.Instance.ChangeScene("res://levels/level_1.tscn");
    }

}
