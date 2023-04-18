using Godot;

// Singleton
public partial class GameWorld : Node
{
    [Signal]
    public delegate void CoinCollectedEventHandler();

    [Signal]
    public delegate void PlayerHurtEventHandler();

    [Signal]
    public delegate void PlayerDiedEventHandler();

    public static GameWorld Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        GD.Print("[INFO] Game start!");
    }

    public void SpawnCloudPoofEffect(Vector2 globalPosition, bool centered = true)
    {
        PackedScene _cloud_poof_effect = GD.Load<PackedScene>("res://scenes/cloud_poof_effect.tscn");
        CloudPoofEffect effect = _cloud_poof_effect.Instantiate<CloudPoofEffect>();
        effect.Position = globalPosition;
        if (!centered)
        {
            effect.Position += Vector2.One * 8.0f;
        }
        GetTree().Root.AddChild(effect);
        GD.Print("[INFO] Spawned cloud poof effect at " + globalPosition.ToString() + "");
    }
}
