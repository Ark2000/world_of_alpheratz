using Godot;
using System.Collections.Generic;

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
    public static string ConfigSection_Misc = "misc";
    public static string Misc_SE = "se_volume";
    public static string Misc_BGM = "bgm_volume";
    public static string Misc_MenuOpacity = "menu_opacity";
    public static string Misc_WindowScale = "window_scale";

    private static Dictionary<string, Key> defaultKeybindings = new Dictionary<string, Key>()
    {
        { InputAction_Up, Key.W },
        { InputAction_Down, Key.S },
        { InputAction_Left, Key.A },
        { InputAction_Right, Key.D },
        { InputAction_Interact, Key.E },
    };

    [Signal]
    public delegate void CoinCollectedEventHandler();

    [Signal]
    public delegate void PlayerHurtEventHandler();

    [Signal]
    public delegate void SecreteAreaFoundEventHandler();

    [Export]
    public ColorRect colorRect;

    public ConfigFile configFile = new ConfigFile();

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

    public void ChangeScene(string nextScenePath, float duration = 0.4f)
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
        Error err = configFile.Load(ConfigFilePath);

        if (err != Error.Ok)
        {
            // Apply default configs

            foreach(var keybinding in defaultKeybindings)
            {
                configFile.SetValue(ConfigSection_Keybindings, keybinding.Key, (long)keybinding.Value);
            }

            ResetMisc();
        }

        // Apply keybindings
        InputEventKey keyEvent;

        keyEvent = new InputEventKey() {Keycode = ((Key)(long)configFile.GetValue(ConfigSection_Keybindings, InputAction_Up)), Pressed = true};
        InputMap.AddAction(InputAction_Up);
        InputMap.ActionAddEvent(InputAction_Up, keyEvent);
        InputMap.ActionAddEvent("ui_up", keyEvent);

        keyEvent = new InputEventKey() {Keycode = ((Key)(long)configFile.GetValue(ConfigSection_Keybindings, InputAction_Down)), Pressed = true};
        InputMap.AddAction(InputAction_Down);
        InputMap.ActionAddEvent(InputAction_Down, keyEvent);
        InputMap.ActionAddEvent("ui_down", keyEvent);

        keyEvent = new InputEventKey() {Keycode = ((Key)(long)configFile.GetValue(ConfigSection_Keybindings, InputAction_Left)), Pressed = true};
        InputMap.AddAction(InputAction_Left);
        InputMap.ActionAddEvent(InputAction_Left, keyEvent);
        InputMap.ActionAddEvent("ui_left", keyEvent);

        keyEvent = new InputEventKey() {Keycode = ((Key)(long)configFile.GetValue(ConfigSection_Keybindings, InputAction_Right)), Pressed = true};
        InputMap.AddAction(InputAction_Right);
        InputMap.ActionAddEvent(InputAction_Right, keyEvent);
        InputMap.ActionAddEvent("ui_right", keyEvent);

        InputMap.AddAction(InputAction_Interact);
        InputMap.ActionAddEvent(InputAction_Interact, new InputEventKey() {Keycode = ((Key)(long)configFile.GetValue(ConfigSection_Keybindings, InputAction_Interact)), Pressed = true});

        // Looks weird for a non pixel-perfect game.
        ApplyResolution();
    }

    public void ApplyResolution()
    {
        int windowScale = (int)configFile.GetValue(ConfigSection_Misc, Misc_WindowScale);
        DisplayServer.WindowSetSize(new Vector2I(432, 312) * windowScale);
    }

    public void ResetMisc()
    {
        configFile.SetValue(ConfigSection_Misc, Misc_SE, 0.6f);
        configFile.SetValue(ConfigSection_Misc, Misc_BGM, 0.6f);
        configFile.SetValue(ConfigSection_Misc, Misc_MenuOpacity, 0.5f);
        configFile.SetValue(ConfigSection_Misc, Misc_WindowScale, 1);
    }

    public void ResetKeys()
    {
        foreach (var keybinding in defaultKeybindings)
        {
            RemapKey(keybinding.Key, keybinding.Value);
        }
    }

    public void RemapKey(string inputAction, Key newKey)
    {
        if (!InputMap.HasAction(inputAction))
        {
            GD.PushError("Input action " + inputAction + " does not exist!");
            return;
        }

        InputEventKey eventKey = new InputEventKey() {Keycode = newKey, Pressed = true};
        InputMap.ActionEraseEvent(inputAction, InputMap.ActionGetEvents(inputAction)[0]);
        InputMap.ActionAddEvent(inputAction, eventKey);

        // Remap UI actions
        if (inputAction == InputAction_Up)
        {
            GD.Print(InputMap.ActionGetEvents("ui_up"));
            InputMap.ActionEraseEvent("ui_up", InputMap.ActionGetEvents("ui_up")[InputMap.ActionGetEvents("ui_up").Count - 1]);
            InputMap.ActionAddEvent("ui_up", eventKey);
        }
        else if (inputAction == InputAction_Down)
        {
            InputMap.ActionEraseEvent("ui_down", InputMap.ActionGetEvents("ui_down")[InputMap.ActionGetEvents("ui_down").Count - 1]);
            InputMap.ActionAddEvent("ui_down", eventKey);
        }
        else if (inputAction == InputAction_Left)
        {
            InputMap.ActionEraseEvent("ui_left", InputMap.ActionGetEvents("ui_left")[InputMap.ActionGetEvents("ui_left").Count - 1]);
            InputMap.ActionAddEvent("ui_left", eventKey);
        }
        else if (inputAction == InputAction_Right)
        {
            InputMap.ActionEraseEvent("ui_right", InputMap.ActionGetEvents("ui_right")[InputMap.ActionGetEvents("ui_right").Count - 1]);
            InputMap.ActionAddEvent("ui_right", eventKey);
        }

        configFile.SetValue(ConfigSection_Keybindings, inputAction, (long) newKey);
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
