namespace Alpheratz;

using System.Collections.Generic;

// Singleton
public partial class GameWorld : Node
{
    public static string ConfigFilePath { get; set; } = "user://config.cfg";
    public static string ConfigSection_Keybindings { get; set; } = "keybindings";
    public static string InputAction_Up { get; set; } = "up";
    public static string InputAction_Down { get; set; } = "down";
    public static string InputAction_Left { get; set; } = "left";
    public static string InputAction_Right { get; set; } = "right";
    public static string InputAction_Interact { get; set; } = "interact";
    public static string ConfigSection_Misc { get; set; } = "misc";
    public static string Misc_SE { get; set; } = "se_volume";
    public static string Misc_BGM { get; set; } = "bgm_volume";
    public static string Misc_MenuOpacity { get; set; } = "menu_opacity";
    public static string Misc_WindowScale { get; set; } = "window_scale";

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

    public ColorRect colorRect;
    public AudioStreamPlayer bgmPlayer1;
    public AudioStreamPlayer bgmPlayer2;

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

        colorRect = new ColorRect();
        colorRect.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        colorRect.MouseFilter = Control.MouseFilterEnum.Ignore;
        colorRect.Color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        AddChild(colorRect);

        bgmPlayer1 = new AudioStreamPlayer();
        bgmPlayer1.Bus = "BGM";
        bgmPlayer2 = new AudioStreamPlayer();
        bgmPlayer2.Bus = "BGM";
        AddChild(bgmPlayer1);
        AddChild(bgmPlayer2);

        GD.Print("[INFO] Game start!");
    }

    public void PlayBGM(string bgmPath, float fadeTime = 0.6f)
    {
        // Crossfade between two bgm players

        AudioStream newBGM = null;

        if (!string.IsNullOrEmpty(bgmPath))
        {
            newBGM = GD.Load<AudioStream>(bgmPath);
        }
        
        AudioStreamPlayer tmp = bgmPlayer1;
        bgmPlayer1 = bgmPlayer2;
        bgmPlayer2 = tmp;

        bgmPlayer1.Stream = newBGM;
        bgmPlayer1.Play();

        Tween tween = CreateTween().SetParallel();
        tween.TweenMethod(Callable.From((float linearDb) => bgmPlayer1.VolumeDb = Mathf.LinearToDb(linearDb)), 0.0f, 1.0f, fadeTime);
        tween.TweenMethod(Callable.From((float linearDb) => bgmPlayer2.VolumeDb = Mathf.LinearToDb(linearDb)), 1.0f, 0.0f, fadeTime);
    }

    public void PlaySFX(string sfxPath, float pitch = 1.0f, float volumeScale = 1.0f)
    {
        AudioStreamPlayer sfxPlayer = new AudioStreamPlayer();
        AddChild(sfxPlayer);
        sfxPlayer.Stream = GD.Load<AudioStream>(sfxPath);
        sfxPlayer.PitchScale = pitch;
        sfxPlayer.VolumeDb = Mathf.LinearToDb(volumeScale);
        sfxPlayer.Bus = "SFX";
        sfxPlayer.Play();
        sfxPlayer.Finished += sfxPlayer.QueueFree;
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

        // Apply sound settings
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("BGM"), Mathf.LinearToDb((float)configFile.GetValue(ConfigSection_Misc, Misc_BGM)));
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"), Mathf.LinearToDb((float)configFile.GetValue(ConfigSection_Misc, Misc_SE)));
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
