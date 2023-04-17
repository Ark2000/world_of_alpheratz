using Godot;

public partial class MobDevo : CharacterBody2D
{
    [Export]
    float jumpForce = 200.0f;

    [Export]
    float jumpInterval = 2.0f;

    [Export]
    float gravityScale = 0.5f;

    float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _Ready()
    {
        Tween tween = CreateTween().SetLoops();
        tween.TweenCallback(Callable.From(() => {
            Vector2 newVelocity = Velocity;
            newVelocity.Y += -jumpForce;
            Velocity = newVelocity;
        })).SetDelay(jumpInterval);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsOnFloor())
        {
            Vector2 newVelocity = Velocity;
            newVelocity.Y += _gravity * gravityScale * ((float)delta);
            Velocity = newVelocity;
        }

        MoveAndSlide();
    }
}
