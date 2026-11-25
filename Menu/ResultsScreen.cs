using Godot;
using System;

public class Results {
    public float Error;
    public int MaxCombo;
    public float Accuracy;
}

public partial class ResultsScreen : CanvasLayer, IInitialisable
{
	Results results;

	[Export] Control statsContainer;

	RichTextLabel accuracyLabel;
	RichTextLabel comboLabel;
	RichTextLabel errorLabel;

	public void Init(object _results) {
		results = (Results)_results;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		accuracyLabel = statsContainer.GetNode<RichTextLabel>("AccuracyLabel");
		comboLabel = statsContainer.GetNode<RichTextLabel>("ComboLabel");
		errorLabel = statsContainer.GetNode<RichTextLabel>("ErrorLabel");

		accuracyLabel.Text = $"Accuracy: {results.Accuracy:0.#}%";
		comboLabel.Text = $"Max Combo: {results.MaxCombo}x";
		errorLabel.Text = $"Avg. Error: {results.Error:0.#}ms";

		SetBackground();
	}

	void SetBackground() {
		TextureRect background = GetNode<TextureRect>("Background");
		background.Texture = DataManager.Instance.backgroundTexture;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _UnhandledInput(InputEvent @event)
    {
		if (@event is InputEventMouseMotion || @event.IsEcho())
            return;

		if (@event.IsActionPressed("ui_cancel")) {
			SceneManager.ChangeScene(SceneManager.Scene.SongSelect, 1f);
		}
    }
}
