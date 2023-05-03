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
				GameWorld.Instance.PlaySFX("res://sounds/sfx_movement_jump17_landing.wav");
				GD.Print("[INFO] Mob Perished!");
			}
		};
	}
}
