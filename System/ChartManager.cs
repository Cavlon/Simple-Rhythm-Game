using Godot;
using System;

public partial class ChartManager : Node
{
	public GameManager gameManager;

	MusicManager musicManager;
	NoteManager noteManager;
	HitField hitField;

	GameplaySettings _gameSettings;
	QuaChart chart;
	int notePointer = 0;

	int lookAhead = 0;
	
	public int chartLength;

	// Called when the node enters the scene tree for the first time.
	public void Initialise(NoteManager _noteManager, HitField _hitField, QuaChart _chart)
	{
		_gameSettings = DataManager.GameplaySettings;

		noteManager = _noteManager;
		hitField = _hitField;

		chart = _chart;

		lookAhead = (int)((hitField.Position.Y - noteManager.Position.Y) * 1000 / _gameSettings.scrollSpeed);
		MusicManager.Instance.StartAudio(-lookAhead-1000);

		chartLength = chart.HitObjects[^1].StartTime;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (notePointer < chart.HitObjects.Count && MusicManager.Instance.songTime + lookAhead > chart.HitObjects[notePointer].StartTime) {
			noteManager.SpawnNote(chart.HitObjects[notePointer].Lane-1, chart.HitObjects[notePointer].StartTime);
			notePointer++;
		}
	}
}
