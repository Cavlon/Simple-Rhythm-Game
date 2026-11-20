using Godot;
using System;

public partial class InputManager : Node
{
	GameManager gameManager;
	PauseMenu pauseMenu;
	NoteManager noteManager;
	ChartManager chartManager;
	HitField hitField;

	GameplaySettings _settings;

	bool[] buttonDown = [false, false, false, false];
	private static readonly StringName[] LaneKeys =
    {
        new("key_0"),
        new("key_1"),
        new("key_2"),
        new("key_3")
    };

	// Called when the node enters the scene tree for the first time.
	public void Initialise(GameManager _gameManager, PauseMenu _pauseMenu, NoteManager _noteManager, HitField _hitField, ChartManager _chartManager)
	{
		_settings = DataManager.GameplaySettings;
		gameManager = _gameManager;
		pauseMenu = _pauseMenu;
		noteManager = _noteManager;
		chartManager = _chartManager;
		hitField = _hitField;
		ProcessMode = ProcessModeEnum.Always;
	}

	public override void _UnhandledInput(InputEvent @event)
    {
		if (@event is InputEventMouseMotion || @event.IsEcho())
            return;

		if (!GetTree().Paused) {
			for (int i = 0; i < 4; i++) {
				if (buttonDown[i]) {
					if (@event.IsActionReleased(LaneKeys[i])) {
						hitField.ReceptorUp(i);
						buttonDown[i] = false;
						return;
					}
				} else {
					if (@event.IsActionPressed(LaneKeys[i])) {
						noteManager.LaneInput(i);
						hitField.ReceptorDown(i);
						buttonDown[i] = true;
						return;
					}
				}
			}

			if (@event.IsActionPressed("ui_accept")) {
				chartManager.SkipIntro();
			}
		} else {
			if (@event.IsActionPressed("ui_down")) {
				pauseMenu.Selected(pauseMenu.selectedId+1);
				GetViewport().SetInputAsHandled();
			}
			
			if (@event.IsActionPressed("ui_up")) {
				pauseMenu.Selected(pauseMenu.selectedId-1);
				GetViewport().SetInputAsHandled();
			}

			if (@event.IsActionPressed("ui_accept")) {
				pauseMenu.Select();
			}
		}

		if (@event.IsActionPressed("ui_cancel")) {
			gameManager.Pause();
		}
    }
}
