namespace Alpheratz;

public partial class MobDevo : Mob
{

    [Export]
    AnimatedSprite2D animatedSprite2D;

    [Export]
    CollisionShape2D collisionShape2D;
    [Export]
    Killer playerKiller;

    [Export]
    float jumpForce = 200.0f;

    [Export]
    float jumpInterval = 2.0f;

    [Export]
    float initDelay = 0.0f;

    [Export]
    float gravityScale = 0.5f;

    float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _Ready()
    {
        CreateTween().TweenCallback(Callable.From(()=>{
            CreateTween().SetLoops().TweenCallback(Callable.From(() => {
            Vector2 newVelocity = Velocity;
            newVelocity.Y += -jumpForce;
            Velocity = newVelocity;
        })).SetDelay(jumpInterval);
    })).SetDelay(initDelay);

    }

    public override void _PhysicsProcess(double delta)
    {
        // Apply gravity
        Vector2 newVelocity = Velocity;
        newVelocity.Y += _gravity * gravityScale * ((float)delta);
        Velocity = newVelocity;

        UpdateSprite();

        MoveAndSlide();
    }

    public void UpdateSprite()
    {
        if (!IsInstanceValid(collisionShape2D))
        {
            animatedSprite2D.Play("hurting");
            return;
        }
        if (IsOnFloor())
        {
            animatedSprite2D.Play("standing");
        }
        else 
        {
            animatedSprite2D.Play("jumping");
        }
    }

    public override void Perish()
    {
        animatedSprite2D.Play("hurting");
        collisionShape2D.QueueFree();
        playerKiller.QueueFree();
        CreateTween().TweenCallback(Callable.From(() => {
            QueueFree();
            GameWorld.Instance.SpawnCloudPoofEffect(GlobalPosition + new Vector2(0.0f, -8.0f));
        })).SetDelay(0.5f);
        Velocity = Velocity with { Y = -128.0f };
    }
}
