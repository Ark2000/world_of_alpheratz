using Godot;
using System;

public partial class Killer : Area2D
{

	[Signal]
	public delegate void MobHurtEventHandler();

	public override void _Ready()
	{
		BodyEntered += (Node2D body) => {
			if (body is PlatformerPlayer)
			{
				PlatformerPlayer player = body as PlatformerPlayer;
				player.Hurt();
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
