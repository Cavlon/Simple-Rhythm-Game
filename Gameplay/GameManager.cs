using Godot;
using System;

public partial class GameManager : Node
{

	public Playfield playfield;
	public ChartManager chartManager;
	public InputManager inputManager;
	public MusicManager musicManager;
	public GameUIManager uiManager;

	QuaChart chartData;

	float accuracySum = 0f;
	int notesProcessed = 0;
	float accuracy;
	float[] judgementAccuracies = [100f, 98.25f, 65f, 0f];
	(string, Color)[] judgementStyles = [("Marvelous", Colors.White), ("Perfect", new Color("e4b33e")), ("Great", new Color("39CFB1")), ("Miss", new Color("FC6880"))];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Parser parser = new();
		chartData = parser.ParseQua(ProjectSettings.GlobalizePath(DataManager.Instance.SelectedSongFolderPath), DataManager.Instance.SelectedChartId);

		playfield = GetNode<Playfield>("Playfield");
		chartManager = GetNode<ChartManager>("ChartManager");
		inputManager = GetNode<InputManager>("InputManager");
		musicManager = GetNode<MusicManager>("MusicManager");
		uiManager = GetNode<GameUIManager>("UI");

		playfield.Initialise(this);
		musicManager.Initialise();
		inputManager.Initialise(playfield.noteManager, playfield.hitField);
		chartManager.Initialise(musicManager, playfield.noteManager, playfield.hitField, chartData);
		uiManager.Initialise(musicManager, chartManager);

		uiManager.SetSongDetails(chartData.SongTitle, chartData.Artist, chartData.Creator);

		playfield.noteManager.NoteJudged += ProcessNote;

		Engine.MaxFps = 240;
        DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
	}

	void ProcessNote(int judgement, float errorTime) {
		notesProcessed++;
		accuracySum += judgementAccuracies[judgement];

		accuracy = accuracySum / notesProcessed;
		uiManager.SetAccuracy(accuracy);
		uiManager.SetJudgement(judgementStyles[judgement].Item1, judgementStyles[judgement].Item2);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
