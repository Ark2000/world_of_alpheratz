using Godot;
using System;

public partial class HurtPlayer : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += (Node2D body) => {
			PlatformerPlayer player = body as PlatformerPlayer;
			player.Hurt();
		};
	}
}
