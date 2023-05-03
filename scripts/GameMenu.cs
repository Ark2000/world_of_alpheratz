namespace Alpheratz;

using System.Collections.Generic;

public partial class GameMenu : CanvasLayer
{
	[Export]
	public Control topMenu;
	[Export]
	public Control settingsMenu;
	[Export]
	public Control keyConfigMenu;
	[Export]
	Button startButton;
	[Export]
	Button settingsButton;
	[Export]
	Button aboutButton;
	[Export]
	Button quitButton;
	[Export]
	Button settingsBackButton;
	[Export]
	Button keyConfigButton;
	[Export]
	Button keyConfigBackButton;
	[Export]
	Button resetKeysButton;
	[Export]
	Slider seVolumeSlider;
	[Export]
	Slider bgmVolumeSlider;
	[Export]
	Slider menuOpacitySlider;
	[Export]
	ButtonGroup windowScaleButtons;
	[Export]
	Button resetMiscButton;
	[Export]
	ColorRect background;

	[Export]
	bool pauseMenuMode = false;

	private readonly Stack<Control> menuStack = new Stack<Control>();
	private Control currentMenu;

    public override void _Ready()
    {
        base._Ready();

		// It's better to set up everything in script instead of in editor.
		seVolumeSlider.MinValue = 0.0f;
		seVolumeSlider.MaxValue = 1.0f;
		seVolumeSlider.Step = 0.1f;
		bgmVolumeSlider.MinValue = 0.0f;
		bgmVolumeSlider.MaxValue = 1.0f;
		bgmVolumeSlider.Step = 0.1f;
		menuOpacitySlider.MinValue = 0.0f;
		menuOpacitySlider.MaxValue = 1.0f;
		menuOpacitySlider.Step = 0.1f;

		UpdateMiscConfigUI();

		quitButton.Pressed += () => {
			GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
			GetTree().Quit();
		};
		startButton.Pressed += () => GameWorld.Instance.ChangeScene("res://scenes/level_1_intro.tscn");
		keyConfigButton.Pressed += () => ShowMenu(keyConfigMenu);
		settingsButton.Pressed += () => ShowMenu(settingsMenu);
		settingsBackButton.Pressed += () => Goback();
		keyConfigBackButton.Pressed += () => Goback();
		startButton.FocusNeighborTop = quitButton.GetPath();
		quitButton.FocusNeighborBottom = startButton.GetPath();
		aboutButton.Pressed += () => {
			OS.ShellOpen("https://github.com/Ark2000/world_of_alpheratz");
		};
		
		resetKeysButton.Pressed += () => {
			GameWorld.Instance.ResetKeys();
			// I don't like group calls. They are not type safe.
			GetTree().CallGroup("remap_buttons", nameof(RemapButton.UpdateState));
		};
		resetMiscButton.Pressed += () => {
			GameWorld.Instance.ResetMisc();
			UpdateMiscConfigUI();
		};
		seVolumeSlider.ValueChanged += (double value) => {
			GameWorld.Instance.configFile.SetValue(GameWorld.ConfigSection_Misc, GameWorld.Misc_SE, value);
        	AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"), Mathf.LinearToDb((float)value));
		};
		bgmVolumeSlider.ValueChanged += (double value) => {
			GameWorld.Instance.configFile.SetValue(GameWorld.ConfigSection_Misc, GameWorld.Misc_BGM, value);
			AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("BGM"), Mathf.LinearToDb((float)value));
		};
		menuOpacitySlider.ValueChanged += (double value) => {
			GD.Print("opa: ", value);
			GameWorld.Instance.configFile.SetValue(GameWorld.ConfigSection_Misc, GameWorld.Misc_MenuOpacity, value);
			ChangeMenuPanelOpacity((float) value);
		};
		windowScaleButtons.Pressed += (BaseButton button) => {
			GD.Print(button.Name);
			int value = int.Parse(button.Name);
			GameWorld.Instance.configFile.SetValue(GameWorld.ConfigSection_Misc, GameWorld.Misc_WindowScale, value);
			GameWorld.Instance.ApplyResolution();
		};
		HideAll();

		if (pauseMenuMode)
		{
			ProcessMode =ProcessModeEnum.Always;
			Hide();
			ShowMenu(topMenu);
		}
		background.Visible = pauseMenuMode;
    }

	public void HideAll()
	{
		topMenu.Hide();
		settingsMenu.Hide();
		keyConfigMenu.Hide();
	}

	public void ShowMenu(Control menu, bool goBack = false)
	{
		if (menu == currentMenu) return;
		currentMenu = menu;

		HideAll();
		menu.Show();
		menu.Scale = menu.Scale with {Y = 0.5f};
		Tween tween = CreateTween().SetParallel()
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Back);
		tween.TweenProperty(menu, "scale:y", 1.0f, 0.2f);
		CreateTween().TweenCallback(Callable.From(() => {
			if (menu == topMenu) startButton.GrabFocus();
		})).SetDelay(0.1f);

		if (!goBack) menuStack.Push(menu);
	}

	public void ChangeMenuPanelOpacity(float value)
	{
		StyleBoxFlat styleBox = (StyleBoxFlat)topMenu.Get("theme_override_styles/panel");
		styleBox.ShadowColor = styleBox.ShadowColor with {A = value };
	}

	public bool Goback()
	{
		bool isLast = false;
		if (menuStack.Count > 1)
		{
			menuStack.Pop();
			ShowMenu(menuStack.Peek(), true);
			isLast = true;
		}
		return isLast;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			if (!Goback() && pauseMenuMode)
			{
				GetTree().Paused = !GetTree().Paused;
				Visible = GetTree().Paused;
				if (GetTree().Paused)
				{
					GameWorld.Instance.PlaySFX("res://sounds/sfx_sounds_pause4_in.wav");
				}
				else
				{
					GameWorld.Instance.PlaySFX("res://sounds/sfx_sounds_pause4_out.wav");
				}
			}
		}
	}

	public void UpdateMiscConfigUI()
	{
		seVolumeSlider.Value = (double) GameWorld.Instance.configFile.GetValue(GameWorld.ConfigSection_Misc, GameWorld.Misc_SE);
		bgmVolumeSlider.Value = (double) GameWorld.Instance.configFile.GetValue(GameWorld.ConfigSection_Misc, GameWorld.Misc_BGM);
		menuOpacitySlider.Value = (double) GameWorld.Instance.configFile.GetValue(GameWorld.ConfigSection_Misc, GameWorld.Misc_MenuOpacity);
		ChangeMenuPanelOpacity((float)menuOpacitySlider.Value);
		int windowScale = (int) GameWorld.Instance.configFile.GetValue(GameWorld.ConfigSection_Misc, GameWorld.Misc_WindowScale);
		windowScaleButtons.GetButtons()[windowScale - 1].ButtonPressed = true;
	}
}
