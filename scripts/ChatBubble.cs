using Godot;
using System;

public partial class ChatBubble : Label
{
	public bool isPlaying = false;
	
	private Vector2 originalPosition;

	public override void _Ready()
	{
		Hide();
		originalPosition = Position;
	}

	public void PlayText(string text)
	{
		if (isPlaying) 
		{
			GD.PushError("ChatBubble is already playing.");
			return;
		}
		isPlaying = true;
		SetProcess(true);
		Position = originalPosition + Vector2.Down * 16;
		Text = text;
		VisibleRatio = 0.0f;
		Modulate = Modulate with {A = 0};
		Show();
		Tween tween = CreateTween();
		tween.TweenProperty(this, "position", originalPosition, 0.4f)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Back);
		tween.SetParallel(true);
		tween.TweenProperty(this, "modulate:a", 1.0f, 0.4f);
		tween.SetParallel(false);
		tween.SetParallel(true);
		tween.TweenProperty(this, "visible_ratio", 1.0f, 0.02f * text.Length);
		tween.SetParallel(false);
		tween.TweenInterval(1.2f);
		tween.TweenCallback(Callable.From(()=>{
			Hide();
			isPlaying = false;
			SetProcess(false);
		}));
	}

	public override void _Process(double delta)
	{
		// Update the size of the bubble
		Size = Vector2.Zero;
	}
}
