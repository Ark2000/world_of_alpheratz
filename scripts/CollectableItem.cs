using Godot;

public partial class CollectableItem : Area2D
{
    [Export]
    CollisionShape2D collisionShape2D;

    public override void _Ready()
    {
        BodyEntered += (Node2D Body) => Collected();
    }

    public void Collected()
    {
        collisionShape2D.QueueFree();
        Tween tween = CreateTween().SetParallel(true);
        tween.TweenProperty(this, "position", Position + Vector2.Up * 32.0f, 1.0f)
        .SetTrans(Tween.TransitionType.Sine);
        tween.TweenProperty(this, "modulate:a", 0.0f, 0.5f).SetDelay(0.5f);
        tween.SetSpeedScale(2.0f);
        tween.Finished += QueueFree;
    }
}
