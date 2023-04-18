using Godot;
using System;

public partial class Killer : Area2D
{

	[Signal]
	public delegate void MobHurtEventHandler();
	[Signal]
	public delegate void PlayerHurtEventHandler();

	public override void _Ready()
	{
		BodyEntered += (Node2D body) => {
			if (body is PlatformerPlayer)
			{
				PlatformerPlayer player = body as PlatformerPlayer;
				player.Hurt();
				EmitSignal(nameof(PlayerHurt));
				GD.Print("[INFO] Ouch!");
			}
			if (body is Mob)
			{
				Mob mob = body as Mob;
				mob.Perish();
				EmitSignal(nameof(MobHurt));
				GD.Print("[INFO] Mob Perished!");
			}
		};
	}
}
