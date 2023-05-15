namespace Alpheratz;
using System.Collections.Generic;

public partial class CutsceneMode : CanvasLayer
{
    [Signal]
    public delegate void CutsceneSkippedEventHandler();
    [Export]
    Control topBar;
    [Export]
    Control bottomBar;

    private List<Tween> currentTweens = new List<Tween>();

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (Input.IsActionJustPressed(GameWorld.InputAction_Interact))
        {
            EmitSignal(nameof(CutsceneSkipped));
        }
    }

    public override void _Ready()
    {
        base._Ready();
        Hide();
        SetProcessInput(false);
    }

    public void EnableCutscene()
    {
        Show();
        SetProcessInput(true);

        ClearTweens();

        bottomBar.OffsetBottom = bottomBar.Size.Y;
        topBar.OffsetTop = -topBar.Size.Y;
        Tween t1 = CreateTween();
        t1.TweenProperty(bottomBar, "offset_bottom", 0.0f, 1.0f);
        Tween t2 = CreateTween();
        t2.TweenProperty(topBar, "offset_top", 0.0f, 1.0f);
        currentTweens.Add(t1);
        currentTweens.Add(t2);
    }

    public void DisableCutscene()
    {
        SetProcessInput(false);

        ClearTweens();

        Tween t1 = CreateTween();
        t1.TweenProperty(bottomBar, "offset_bottom", bottomBar.Size.Y, 1.0f);
        Tween t2 = CreateTween();
        t2.TweenProperty(topBar, "offset_top", -topBar.Size.Y, 1.0f);
    }

    public void ClearTweens()
    {
        foreach (Tween tween in currentTweens)
        {
            tween.Kill();
        }
        currentTweens.Clear();
    }
}
