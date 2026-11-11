using Godot;
using System.IO;

public partial class GameManager : Node
{

	public Playfield playfield;
	public ChartManager chartManager;
	public InputManager inputManager;
	public GameUIManager uiManager;

	QuaChart chartData;

	float accuracySum = 0f;
	int notesProcessed = 0;
	float accuracy;
	int combo = 0;

	float[] judgementAccuracies = [100f, 98.25f, 65f, 0f];
	(string, Color)[] judgementStyles = [("Marvelous", Colors.White), ("Perfect", new Color("e4b33e")), ("Great", new Color("39CFB1")), ("Miss", new Color("FC6880"))];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Parser parser = new();
		chartData = parser.ParseQuaChart(DataManager.Instance.MapPath);

		InitialiseManagers();

		uiManager.SetSongDetails(chartData.SongTitle, chartData.Artist, chartData.Creator);

		playfield.noteManager.NoteJudged += ProcessNote;

		SetBackground();

		Engine.MaxFps = DataManager.GameplaySettings.maxFps;
        DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	void ProcessNote(int judgement, float errorTime) {
		notesProcessed++;
		accuracySum += judgementAccuracies[judgement];

		if (judgement < 3) combo++;
		else combo = 0;

		accuracy = accuracySum / notesProcessed;
		uiManager.SetAccuracy(accuracy);
		uiManager.SetJudgement(judgementStyles[judgement].Item1, judgementStyles[judgement].Item2);
		uiManager.SetCombo(combo);
	}

	void InitialiseManagers() {
		playfield = GetNode<Playfield>("Playfield");
		chartManager = GetNode<ChartManager>("ChartManager");
		inputManager = GetNode<InputManager>("InputManager");
		uiManager = GetNode<GameUIManager>("UI");

		playfield.Initialise();
		inputManager.Initialise(playfield.noteManager, playfield.hitField);
		chartManager.Initialise(playfield.noteManager, playfield.hitField, chartData);
		uiManager.Initialise(chartManager);
	}

	void SetBackground() {
		TextureRect background = GetNode("BackgroundLayer").GetNode<TextureRect>("Background");
		background.Texture = ImageTexture.CreateFromImage(Image.LoadFromFile(DataManager.Instance.BackgroundPath));
		background.Modulate = new Color(Colors.White, DataManager.StyleSettings.backgroundOpacity);
	}
}
