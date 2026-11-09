using Godot;
using System;

public partial class InputManager : Node
{
	NoteManager noteManager;
	HitField hitField;

	GameplaySettings _settings;

	bool[] buttonDown = [false, false, false, false];

	// Called when the node enters the scene tree for the first time.
	public void Initialise(NoteManager _noteManager, HitField _hitField)
	{
		_settings = DataManager.GameplaySettings;
		noteManager = _noteManager;
		hitField = _hitField;
	}

	public override void _UnhandledInput(InputEvent @event)
    {
		for (int i = 0; i < 4; i++) {
			if (buttonDown[i]) {
				if (@event.IsActionReleased($"key_{i}")) {
					hitField.ReceptorUp(i);
					buttonDown[i] = false;
				}
			} else {
				if (@event.IsActionPressed($"key_{i}")) {
					noteManager.LaneInput(i);
					hitField.ReceptorDown(i);
					buttonDown[i] = true;
				}
			}
		}
    }
}
