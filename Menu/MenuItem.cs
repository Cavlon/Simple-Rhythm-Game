using Godot;
using System;
public partial class MenuItem : Control
{
	[Signal]
    public delegate void ClickedEventHandler();

	[Export] Color unselectedColour;
	[Export] Color selectedColour;

	Tween hoverTween;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Modulate = unselectedColour;
		PivotOffsetRatio = Vector2.One * 0.5f;
	}

	public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
        {
			EmitSignal(SignalName.Clicked);
        }
    }

	public void OnHover() {
		if (hoverTween != null && hoverTween.IsRunning()) {
            hoverTween.Kill();
        }

		hoverTween = CreateTween().SetParallel(true);

        hoverTween.TweenProperty(this, "scale", Vector2.One * 1.2f, 0.2f)
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);

        hoverTween.TweenProperty(this, "modulate", selectedColour, 0.1f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.In);
	}

	public void OnUnHover() {
		if (hoverTween != null && hoverTween.IsRunning()) {
            hoverTween.Kill();
        }

		hoverTween = CreateTween().SetParallel(true);

        hoverTween.TweenProperty(this, "scale", Vector2.One, 0.2f)
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);

        hoverTween.TweenProperty(this, "modulate", unselectedColour, 0.1f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.In);
	}
}
