namespace Alpheratz;

public partial class MobBub : Mob
{

    [Export]
    AnimatedSprite2D animatedSprite2D;
    [Export]
    CollisionShape2D collisionShape2D;
    [Export]
    Killer playerKiller;

    [Export]
    float maxSpeed = 32.0f;

    [Export]
    float xInput = -1.0f;

    [Export]
    float gravityScale = 0.5f;

    float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    bool checkWall = true;

    public override void _PhysicsProcess(double _delta)
    {
        float delta = ((float)_delta);

        Vector2 newVelocity = Velocity;
        newVelocity.Y += _gravity * delta * gravityScale;
        newVelocity.X = xInput * maxSpeed;
        Velocity = newVelocity;

        if (checkWall && IsOnWall())
        {
            xInput *= -1.0f;
            checkWall = false;
            CreateTween().TweenCallback(Callable.From(() => {
                checkWall = true;
            })).SetDelay(0.1f);
        }

        MoveAndSlide();

        UpdateSprite();
    }

    public void UpdateSprite()
    {
        animatedSprite2D.FlipH = xInput > 0.0f;

        if (!IsInstanceValid(collisionShape2D))
        {
            animatedSprite2D.Play("hurting");
            return;
        }
        animatedSprite2D.Play("running");
    }

    public override void Perish()
    {
        animatedSprite2D.Play("hurting");
        playerKiller.QueueFree();
        collisionShape2D.QueueFree();
        CreateTween().TweenCallback(Callable.From(() => {
            QueueFree();
            GameWorld.Instance.SpawnCloudPoofEffect(GlobalPosition + new Vector2(0.0f, -8.0f));
        })).SetDelay(0.5f);
        Velocity = Velocity with { Y = -128.0f };
    }
}
