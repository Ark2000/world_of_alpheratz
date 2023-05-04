namespace Alpheratz;

public partial class CollectableItem : Area2D
{
    [Export]
    CollisionShape2D collisionShape2D;

    static int coinCount = 0;
    static ulong lastCoinTimeMs = 0;

    public override void _Ready()
    {
        BodyEntered += Collected;
    }

    public void Collected(Node2D _body)
    {

        // There should be a minimum interval between two coin sfx
        ulong deltaMs = Time.GetTicksMsec() - lastCoinTimeMs;
        float sfxDelay = 0;
        ulong minInterval = 60;
        if (deltaMs < minInterval)
        {
            sfxDelay = (minInterval - deltaMs) * 0.001f;
            lastCoinTimeMs += minInterval;
        }
        else
        {
            lastCoinTimeMs = Time.GetTicksMsec();
        }
        CreateTween().TweenCallback(Callable.From(() => {
            GameWorld.Instance.PlaySFX("res://sounds/sfx_coin_double4.wav", coinCount % 10 * 0.1f + 1.0f);
            if (coinCount % 10 == 0)
            {
                GameWorld.Instance.PlaySFX("res://sounds/sfx_coin_cluster5.wav");
            }
        })).SetDelay(sfxDelay);

        coinCount += 1;

        GameWorld.Instance.EmitSignal(nameof(GameWorld.CoinCollected));
        collisionShape2D.QueueFree();
        Tween tween = CreateTween().SetParallel(true);
        tween.TweenProperty(this, "position", Position + Vector2.Up * 32.0f, 1.0f)
        .SetTrans(Tween.TransitionType.Sine);
        tween.TweenProperty(this, "modulate:a", 0.0f, 0.5f).SetDelay(0.5f);
        tween.SetSpeedScale(2.0f);
        tween.Finished += QueueFree;
    }
}
