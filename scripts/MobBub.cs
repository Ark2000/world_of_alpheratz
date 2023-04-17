using Godot;

public partial class MobBub : CharacterBody2D
{

    [Export]
    AnimatedSprite2D animatedSprite2D;

    [Export]
    float maxSpeed = 32.0f;

    [Export]
    float xInput = 1.0f;

    float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _PhysicsProcess(double _delta)
    {
        float delta = ((float)_delta);
        Vector2 newVelocity = Velocity;

        if (!IsOnFloor())
        {
            newVelocity.Y += _gravity * delta;
        }
        newVelocity.X = xInput * maxSpeed;

        Velocity = newVelocity;
        Vector2 prevPosition = Position;
        MoveAndSlide();
        float dx = Mathf.Abs(Position.X - prevPosition.X);
        if (dx < 0.5)
        {
            xInput *= -1;
        }
        animatedSprite2D.FlipH = xInput < 0.0f ? true : false;
    }
}
