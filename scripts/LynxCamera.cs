namespace Alpheratz;

public partial class LynxCamera : Camera2D
{
    [Export]
    public Node2D target;
    public Vector2 originalZoom;

    public override void _Ready()
    {
        base._Ready();
        originalZoom = Zoom;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        Vector2 pos = target.GlobalPosition;

        if (target is PlatformerPlayer)
        {
            PlatformerPlayer player = target as PlatformerPlayer;
            pos += player.animatedSprite.FlipH ? new Vector2(-32, 0) : new Vector2(32, 0);
            
            if (!player.IsOnFloor() && player.Velocity.Y < player.MAX_FALL_SPEED)
            {
                pos = pos with {Y = Position.Y};
            }
        }

        Vector2 targetZoom = (Vector2) target.GetMeta("Zoom", originalZoom);

        Position = Position.Lerp(pos, 0.172f);
        Zoom = Zoom.Lerp(targetZoom, 0.041f);
    }
}
