using Godot;
using System;

public partial class RemapButton : Button
{
	[Export]
	// See InputAction_xxx defined in GameWorld.cs
	string inputAction;

    public override void _Ready()
    {
		UpdateState();
		SetProcessUnhandledKeyInput(false);
		Toggled += OnButtonToggled;
		AddToGroup("remap_buttons");
	}

	public void UpdateState()
	{
		// Read the key from config file
		Key key = (Key)(long)GameWorld.Instance.configFile.GetValue(GameWorld.ConfigSection_Keybindings, inputAction);
		Text = "Key " + OS.GetKeycodeString(key);

	}

	public override void _UnhandledKeyInput(InputEvent @event)
	{
		InputEventKey keyEvent = @event as InputEventKey;
		ButtonPressed = false;
		Text = "Key " + OS.GetKeycodeString(keyEvent.Keycode);
		GameWorld.Instance.RemapKey(inputAction, keyEvent.Keycode);
	}

	private void OnButtonToggled(bool buttonPressed)
	{
		SetProcessUnhandledKeyInput(buttonPressed);
		if (buttonPressed)
		{
			Text = "Press a key";
		}
		else 
		{
			ReleaseFocus();
		}
	}
}
