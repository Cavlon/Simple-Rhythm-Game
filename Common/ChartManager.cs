using Godot;
using System;

public partial class ChartManager : Node
{

	[Export] MusicManager musicManager;
	[Export] NoteManager noteManager;
	[Export] HitField hitField;

	GameplaySettings _settings;
	Parser parser;
	QuaChart chart;
	int notePointer = 0;

	float lookAhead = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_settings = DataManager.GameplaySettings;

		parser = new Parser();
		chart = parser.ParseQua(ProjectSettings.GlobalizePath(DataManager.Instance.SelectedSongFolderPath), DataManager.Instance.SelectedChartId);
		lookAhead = (hitField.Position.Y - noteManager.Position.Y) / _settings.scrollSpeed;
		musicManager.songTime -= lookAhead;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (notePointer < chart.HitObjects.Count && musicManager.GetSongPos()+lookAhead > chart.HitObjects[notePointer].StartTime/1000) {
			noteManager.SpawnNote(chart.HitObjects[notePointer].Lane-1);
			notePointer++;
		}
	}
}
