namespace Alpheratz;

//Play Animation starts from random frame
public partial class RandomizedAnimatedSprite2D : AnimatedSprite2D
{
    public override void _Ready()
    {
        int framesCount = SpriteFrames.GetFrameCount(Animation);
        Frame = GD.RandRange(0, framesCount - 1);
    }
}
