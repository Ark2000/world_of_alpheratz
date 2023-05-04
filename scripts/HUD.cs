namespace Alpheratz;

public partial class HUD : CanvasLayer
{
    [Export]
    Control heartProto;
    [Export]
    Container heartContainer;

    [Export]
    public ScoreBoard remainTime;

    [Export]
    public ScoreBoard collectedCoins;

    public override void _Ready()
    {
        base._Ready();
        heartProto.Hide();
        for (int i = 0; i < heartContainer.GetChildCount(); i++)
        {
            heartContainer.GetChild(i).QueueFree();
        }
    }

    public void SetHearts(int hearts, int maxHearts)
    {
        int heartCount = heartContainer.GetChildCount();
        if (maxHearts > heartCount)
        {
            for (int i = 0; i < maxHearts - heartCount; i++)
            {
                Control heart = heartProto.Duplicate() as Control;
                heart.Show();
                heartContainer.AddChild(heart);
            }
        }
        else if (maxHearts < heartCount)
        {
            for (int i = 1; i <= heartCount - maxHearts; i++)
            {
                heartContainer.GetChild(heartContainer.GetChildCount() - i).QueueFree();
                GD.Print($"[INFO] QueueFree: {heartContainer.GetChildCount() - i}");
            }
        }

        for (int i = 0; i < maxHearts; i++)
        {
            Control heart = heartContainer.GetChild<Control>(i);
            Control fullHeart = heart.GetChild<Control>(0);
            fullHeart.Visible = i < hearts;
        }
    }
}
