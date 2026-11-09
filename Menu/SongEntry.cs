using Godot;
using System;

public partial class SongEntry : TextureRect
{

	[Signal]
    public delegate void ClickedEventHandler(int id);
	int id;

	[Export] Texture2D unselectedTexture;
	[Export] Texture2D selectedTexture;

	RichTextLabel titleLabel;

	Tween selectedTween;
	Tween hoverTween;

	Vector2 initialClickPos = Vector2.Up;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		titleLabel = GetNode<RichTextLabel>("SongTitle");
		Texture = unselectedTexture;

		MouseEntered += OnHover;
		MouseExited += OnUnHover;
	}

	public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
        {
			if (mouseEvent.Pressed) {
				initialClickPos = mouseEvent.GlobalPosition;
			} else if (initialClickPos != Vector2.Up) {
				if ((mouseEvent.GlobalPosition - initialClickPos).LengthSquared() < 2500) EmitSignal(SignalName.Clicked, id);
				initialClickPos = Vector2.Up;
			}
        }
    }

	public void SetId(int _id) {
		id = _id;
		titleLabel.Text = id.ToString();
	}

	public void Selected(bool tween = true) {
		if (selectedTween != null && selectedTween.IsRunning()) {
            selectedTween.Kill();
        }

		Texture = selectedTexture;

		if (!tween) {
			Position = new Vector2(0f, Position.Y);
			Modulate = Colors.White;
			return;
		}

		selectedTween = CreateTween().SetParallel(true);

        selectedTween.TweenProperty(this, "position:x", 2f, 0.4f)
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);

        selectedTween.TweenProperty(this, "modulate:a", 1f, 0.1f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.In);
	}

	public void UnSelected(bool tween = true) {
		if (selectedTween != null && selectedTween.IsRunning()) {
            selectedTween.Kill();
        }

		Texture = unselectedTexture;

		if (!tween) {
			Position = new Vector2(30f, Position.Y);
			Modulate = new Color(1f, 1f, 1f, 0.7f);
			return;
		}

		selectedTween = CreateTween().SetParallel(true);

        selectedTween.TweenProperty(this, "position:x", 30f, 0.4f)
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);

        selectedTween.TweenProperty(this, "modulate:a", 0.7f, 0.1f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.In);
	}

	void OnHover() {
		if (hoverTween != null && hoverTween.IsRunning()) {
            hoverTween.Kill();
        }

		hoverTween = CreateTween().SetParallel(true);

        hoverTween.TweenProperty(this, "modulate", new Color(0.8f, 0.8f, 0.8f, Modulate.A), 0.05f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.In);
	}

	void OnUnHover() {
		if (hoverTween != null && hoverTween.IsRunning()) {
            hoverTween.Kill();
        }

		hoverTween = CreateTween().SetParallel(true);

        hoverTween.TweenProperty(this, "modulate", new Color(1f, 1f, 1f, Modulate.A), 0.1f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.In);
	}
}
