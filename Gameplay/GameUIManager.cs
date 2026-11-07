using Godot;
using System;

public partial class GameUIManager : CanvasLayer
{

	[Export] RichTextLabel accuracyLabel;
	[Export] RichTextLabel judgementLabel;

	[Export] RichTextLabel songTitleLabel;
	[Export] RichTextLabel songArtistLabel;
	[Export] RichTextLabel charterLabel;

	[Export] RichTextLabel songProgressLabel;
	[Export] RichTextLabel songRemainingLabel;
	[Export] ProgressBar songProgressBar;

	MusicManager musicManager;
	ChartManager chartManager;

	Tween judgementTween;

	public void Initialise(MusicManager _musicManager, ChartManager _chartManager) {
		musicManager = _musicManager;
		chartManager = _chartManager;
		songProgressBar.MaxValue = chartManager.chartLength;
	}

	public void SetAccuracy(float accuracy) {
		accuracyLabel.Text = $"{accuracy:0.#}%";
	}

	public void SetJudgement(string judgementText, Color textColour) {
		judgementLabel.Text = judgementText;
		judgementLabel.Scale = Vector2.One * 0.5f;
		judgementLabel.Modulate = new Color(textColour, 1f);

		if (judgementTween != null && judgementTween.IsRunning()) {
            judgementTween.Kill();
        }

		judgementTween = CreateTween().SetParallel(true);

        judgementTween.TweenProperty(judgementLabel, "scale", Vector2.One, 0.12f)
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);

        judgementTween.TweenProperty(judgementLabel, "modulate:a", 0.0f, 0.3f)
            .SetDelay(0.15f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.In);
	}

	public void SetSongDetails(string title, string artist, string charter) {
		songTitleLabel.Text = title;
		songArtistLabel.Text = artist;
	 	charterLabel.Text = $"Chart: {charter}";
	}

    public override void _Process(double delta)
    {
		songProgressBar.Value = musicManager.songTime;

        int seconds = (int)(musicManager.songTime / 1000f);
        int minutes = seconds / 60;
		seconds %= 60;

		if (musicManager.songTime < 0) {
			songProgressLabel.Text = $"-{minutes:00}:{-seconds:00}";
		} else {
			songProgressLabel.Text = $"{minutes:00}:{seconds:00}";
		}

		int remaining = Math.Max(chartManager.chartLength - musicManager.songTime, 0);
		seconds = (int)(remaining / 1000f);
        minutes = seconds / 60;
		seconds %= 60;

		songRemainingLabel.Text = $"-{minutes:00}:{seconds:00}";
    }
}
