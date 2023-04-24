using Godot;

public partial class InteractiveMessage : Area2D
{

    [Signal]
    public delegate void MessageActivatedEventHandler();

    [Export]
    Vector2 offset;
    [Export]
    float duration = 0.5f;
    [Export]
    Node2D target;

    private Vector2 initPosition;
    private bool isActive = false;

    public override void _Ready()
    {
        base._Ready();
        initPosition = target.Position;
        target.Modulate = target.Modulate with { A = 0.0f };

        BodyEntered += (Node2D body) => {
            isActive = true;
            PopUp();
        };

        BodyExited += (Node2D body) => {
            isActive = false;
            Fade();
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (isActive && Input.IsActionJustPressed("interact"))
        {
            EmitSignal(nameof(MessageActivated));
        }
    }

    public void PopUp()
    {
        Tween t = CreateTween();
        t.SetParallel();
        t.TweenProperty(target, "position", initPosition + offset, duration)
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);
        t.TweenProperty(target, "modulate:a", 1.0f, duration)
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);
    }

    public void Fade()
    {
        Tween t = CreateTween();
        t.SetParallel();
        t.TweenProperty(target, "position", initPosition, duration)
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);
        t.TweenProperty(target, "modulate:a", 0.0f, duration)
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);
    }
}
