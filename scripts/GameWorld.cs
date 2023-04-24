using Godot;

// Singleton
public partial class GameWorld : Node
{

    public static string ConfigFilePath = "user://config.cfg";
    public static string ConfigSection_Keybindings = "keybindings";
    public static string InputAction_Up = "up";
    public static string InputAction_Down = "down";
    public static string InputAction_Left= "left";
    public static string InputAction_Right = "right";
    public static string InputAction_Interact = "interact";

    [Signal]
    public delegate void CoinCollectedEventHandler();

    [Signal]
    public delegate void PlayerHurtEventHandler();

    [Signal]
    public delegate void PlayerDiedEventHandler();
    [Signal]
    public delegate void SecreteAreaFoundEventHandler();

    [Export]
    public ColorRect colorRect;

    public ConfigFile configFile;

    public static GameWorld Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        LoadConfig();

        InputEventKey keyEvent = new InputEventKey();
        keyEvent.Keycode = Key.W;
        keyEvent.Pressed = true;
        InputMap.AddAction("test_action");
        InputMap.ActionAddEvent("test_action", keyEvent);

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

    public void ChangeScene(string nextScenePath, float duration = 1.0f)
    {
        colorRect.Color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        Tween tween = CreateTween();
        tween.TweenProperty(colorRect, "color:a", 1.0f, duration);
        tween.TweenCallback(Callable.From(() => GetTree().ChangeSceneToFile(nextScenePath)));
        tween.TweenInterval(duration);
        tween.TweenProperty(colorRect, "color:a", 0.0f, duration);
    }

    public void LoadConfig()
    {
        configFile = new ConfigFile();
        Error err = configFile.Load(ConfigFilePath);

        if (err != Error.Ok)
        {
            // Create a new config file
            configFile.SetValue(ConfigSection_Keybindings, InputAction_Up, (long) Key.W);
            configFile.SetValue(ConfigSection_Keybindings, InputAction_Down, (long) Key.S);
            configFile.SetValue(ConfigSection_Keybindings, InputAction_Left, (long) Key.A);
            configFile.SetValue(ConfigSection_Keybindings, InputAction_Right, (long) Key.D);
            configFile.SetValue(ConfigSection_Keybindings, InputAction_Interact, (long) Key.E);
        }

        // Apply keybindings
        InputMap.AddAction(InputAction_Up);
        InputMap.ActionAddEvent(InputAction_Up, new InputEventKey() {Keycode = ((Key)(long)configFile.GetValue(ConfigSection_Keybindings, InputAction_Up)), Pressed = true});
        InputMap.AddAction(InputAction_Down);
        InputMap.ActionAddEvent(InputAction_Down, new InputEventKey() {Keycode = ((Key)(long)configFile.GetValue(ConfigSection_Keybindings, InputAction_Down)), Pressed = true});
        InputMap.AddAction(InputAction_Left);
        InputMap.ActionAddEvent(InputAction_Left, new InputEventKey() {Keycode = ((Key)(long)configFile.GetValue(ConfigSection_Keybindings, InputAction_Left)), Pressed = true});
        InputMap.AddAction(InputAction_Right);
        InputMap.ActionAddEvent(InputAction_Right, new InputEventKey() {Keycode = ((Key)(long)configFile.GetValue(ConfigSection_Keybindings, InputAction_Right)), Pressed = true});
        InputMap.AddAction(InputAction_Interact);
        InputMap.ActionAddEvent(InputAction_Interact, new InputEventKey() {Keycode = ((Key)(long)configFile.GetValue(ConfigSection_Keybindings, InputAction_Interact)), Pressed = true});
    }

    public void SaveConfig()
    {
        configFile.Save(ConfigFilePath);
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            // Save config on exit
            SaveConfig();
        }
    }
}
