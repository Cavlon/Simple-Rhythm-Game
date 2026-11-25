using Godot;
using System;

public partial class ChartManager : Node
{
	NoteManager noteManager;
	HitField hitField;
	UIManager uiManager;

	GameplaySettings _gameSettings;
	QuaChart chart;
	int notePointer = 0;

	int lookAhead = 0;
	int skipThresh;
	
	public int chartLength;

	// Called when the node enters the scene tree for the first time.
	public void Initialise(NoteManager _noteManager, HitField _hitField, UIManager _uimanager, QuaChart _chart)
	{
		_gameSettings = DataManager.GameplaySettings;

		noteManager = _noteManager;
		hitField = _hitField;
		uiManager = _uimanager;

		chart = _chart;

		lookAhead = (int)((hitField.Position.Y - noteManager.Position.Y) * 1000 / _gameSettings.scrollSpeed);
        _ = MusicManager.Instance.StartAudio(-lookAhead - _gameSettings.startTime);

		chartLength = chart.HitObjects[^1].StartTime;
		skipThresh = chart.HitObjects[0].StartTime - lookAhead - _gameSettings.startTime - _gameSettings.skipThresh;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		while (notePointer < chart.HitObjects.Count && MusicManager.Instance.songTime + lookAhead >= chart.HitObjects[notePointer].StartTime) {
			noteManager.SpawnNote(chart.HitObjects[notePointer].Lane-1, chart.HitObjects[notePointer].StartTime);
			notePointer++;
		}
	}

	public bool SkipCheck() {
		return chart.HitObjects[0].StartTime - _gameSettings.skipThresh > 0;
	}

	public void SkipIntro() {
		if (MusicManager.Instance.songTime >= skipThresh) return;

		GD.Print("Skipped Intro");
		uiManager.SkipAnim();
		MusicManager.Instance.SeekAudio(skipThresh);
	}
}
