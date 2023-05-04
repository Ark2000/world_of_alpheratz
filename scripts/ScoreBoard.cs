namespace Alpheratz;

using Godot.Collections;

public partial class ScoreBoard : HBoxContainer
{
	[Export]
	public TextureRect number1;
	[Export]
	public TextureRect number2;
	[Export]
	public TextureRect number3;

	private static readonly Dictionary<int, Rect2> atlasRegions = new()
	{
		{0, new Rect2(16, 0, 8, 8)},
		{1, new Rect2(24, 0, 8, 8)},
		{2, new Rect2(32, 0, 8, 8)},
		{3, new Rect2(40, 0, 8, 8)},
		{4, new Rect2(48, 0, 8, 8)},
		{5, new Rect2(16, 8, 8, 8)},
		{6, new Rect2(24, 8, 8, 8)},
		{7, new Rect2(32, 8, 8, 8)},
		{8, new Rect2(40, 8, 8, 8)},
		{9, new Rect2(48, 8, 8, 8)},
	};

	public override void _Ready()
	{
		number1.PivotOffset = number1.Size / 2.0f;
		number2.PivotOffset = number2.Size / 2.0f;
		number3.PivotOffset = number3.Size / 2.0f;
	}

	public void SetNumber(int number)
	{
		if (number < 0)
		{
			number = 0;
		}
		else if (number > 999)
		{
			number = 999;
		}

		int hundreds = number / 100;
		int tens = (number - hundreds * 100) / 10;
		int ones = number - hundreds * 100 - tens * 10;

		AtlasTexture atlasTexture = number1.Texture as AtlasTexture;
		if (atlasTexture.Region != atlasRegions[hundreds])
		{
			BouncyAnimation(number1);
			atlasTexture.Region = atlasRegions[hundreds];
		}

		atlasTexture = number2.Texture as AtlasTexture;
		if (atlasTexture.Region != atlasRegions[tens])
		{
			BouncyAnimation(number2);
			atlasTexture.Region = atlasRegions[tens];
		}

		atlasTexture = number3.Texture as AtlasTexture;
		if (atlasTexture.Region != atlasRegions[ones])
		{
			BouncyAnimation(number3);
			atlasTexture.Region = atlasRegions[ones];
		}
	}

	private void BouncyAnimation(Control control)
	{
		Tween tween = CreateTween();
		tween.TweenProperty(control, "scale", Vector2.One * 1.25f, 0.1f);
		tween.TweenProperty(control, "scale", Vector2.One * 1.0f, 0.1f);
	}
}
