public partial class Trail : Line2D
{
    [Export]
    Node2D target;

    public override void _Ready()
    {
        base._Ready();

        // Reset position
        Position = Vector2.Zero;

        // Keep adding points.
        CreateTween().SetLoops().TweenCallback(Callable.From(() => {
            AddPoint(target.GlobalPosition);
        })).SetDelay(0.05f);
        
        // Keep removing points.
        CreateTween().TweenCallback(Callable.From(()=>{
            CreateTween().SetLoops().TweenCallback(Callable.From(()=>{
                RemovePoint(0);
            })).SetDelay(0.05f);
        })).SetDelay(4.0f);

    }
}
