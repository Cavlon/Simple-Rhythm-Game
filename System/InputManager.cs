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

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		for (int i = 0; i < 4; i++) {
			if (buttonDown[i]) {
				if (Input.IsActionJustReleased($"key_{i}")) {
					hitField.ReceptorUp(i);
					buttonDown[i] = false;
				}
			} else {
				if (Input.IsActionJustPressed($"key_{i}")) {
					noteManager.LaneInput(i);
					hitField.ReceptorDown(i);
					buttonDown[i] = true;
				}
			}
		}

	}
}
