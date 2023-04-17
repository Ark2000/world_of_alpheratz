using Godot;

public partial class SecretArea : Area2D
{
    public override void _Ready()
    {
        Modulate = Modulate with { A = 1.0f };
        BodyEntered += (Node2D body) => FadeOut();
        BodyExited += (Node2D body) => FadeIn();
    }

    public void FadeOut()
    {
        CreateTween().TweenProperty(this, "modulate:a", 0.2f, 0.5f);
    }

    public void FadeIn()
    {
        CreateTween().TweenProperty(this, "modulate:a", 1.0f, 0.5f);
    }
}
