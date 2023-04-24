using Godot;
using System;

public partial class RemapButton : Button
{
	[Export]
	// See InputAction_xxx defined in GameWorld.cs
	string inputAction;

    public override void _Ready()
    {
		// Read the key from the config file
		Key key = (Key)(long)GameWorld.Instance.configFile.GetValue(GameWorld.ConfigSection_Keybindings, inputAction);
		Text = "Key " + OS.GetKeycodeString(key);
		SetProcessUnhandledKeyInput(false);
		Toggled += OnButtonToggled;
	}

	public override void _UnhandledKeyInput(InputEvent @event)
	{
		InputEventKey keyEvent = @event as InputEventKey;
		ButtonPressed = false;
		Text = "Key " + OS.GetKeycodeString(keyEvent.Keycode);
		// Save the key to the config file
		GameWorld.Instance.configFile.SetValue(GameWorld.ConfigSection_Keybindings, inputAction, (long)keyEvent.Keycode);
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
