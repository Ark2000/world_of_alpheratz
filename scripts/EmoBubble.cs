public partial class EmoBubble : Sprite2D
{
	[Export]
	private AnimationPlayer _anim;

	public void PlayEmo(int idx)
	{
		Frame = idx;
		_anim.Stop();
		_anim.Play("pop");
	}
}
