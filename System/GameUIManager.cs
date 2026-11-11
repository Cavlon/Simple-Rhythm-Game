using Godot;
using System;

public partial class GameUIManager : CanvasLayer
{

	[Export] RichTextLabel accuracyLabel;
	[Export] RichTextLabel judgementLabel;
	[Export] RichTextLabel comboLabel;

	[Export] RichTextLabel songTitleLabel;
	[Export] RichTextLabel songArtistLabel;
	[Export] RichTextLabel charterLabel;

	[Export] RichTextLabel songProgressLabel;
	[Export] RichTextLabel songRemainingLabel;
	[Export] ProgressBar songProgressBar;

	StyleSettings _styleSettings;

	ChartManager chartManager;

	Tween judgementTween;
	Tween comboTween;
	Tween comboAlphaTween;

	public void Initialise(ChartManager _chartManager) {
		_styleSettings = DataManager.StyleSettings;

		chartManager = _chartManager;

		songProgressBar.MaxValue = chartManager.chartLength;
		judgementLabel.Position -= Vector2.Down * _styleSettings.judgementPosY;
		comboLabel.Position -= Vector2.Down * _styleSettings.comboPosY;

		comboLabel.AddThemeFontSizeOverride("font_size", _styleSettings.comboSize);
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

	public void SetCombo(int combo) {
		if (combo > 20) {
			comboLabel.Text = combo.ToString();

			comboLabel.Scale = Vector2.One * 0.8f;

			if (comboTween != null && comboTween.IsRunning()) {
				comboTween.Kill();
			}

			comboTween = CreateTween().SetParallel(true);

			comboTween.TweenProperty(comboLabel, "scale", Vector2.One, 0.12f)
				.SetTrans(Tween.TransitionType.Back)
				.SetEase(Tween.EaseType.Out);

			if (comboLabel.Modulate.A == 0f) {
				if (comboAlphaTween != null && comboAlphaTween.IsRunning()) {
					comboAlphaTween.Kill();
				}

				comboAlphaTween = CreateTween().SetParallel(true);

				comboAlphaTween.TweenProperty(comboLabel, "modulate:a", 1.0f, 0.1f)
					.SetTrans(Tween.TransitionType.Sine)
					.SetEase(Tween.EaseType.In);
			}

		} else if (comboLabel.Modulate.A == 1f) {
			if (comboAlphaTween != null && comboAlphaTween.IsRunning()) {
				comboAlphaTween.Kill();
			}

			comboAlphaTween = CreateTween().SetParallel(true);

			comboAlphaTween.TweenProperty(comboLabel, "modulate:a", 0.0f, 0.1f)
				.SetTrans(Tween.TransitionType.Sine)
				.SetEase(Tween.EaseType.In);
		}
	}

	public void SetSongDetails(string title, string artist, string charter) {
		songTitleLabel.Text = title;
		songArtistLabel.Text = artist;
	 	charterLabel.Text = $"Chart: {charter}";
	}

    public override void _Process(double delta)
    {
		songProgressBar.Value = MusicManager.Instance.songTime;

        int seconds = (int)(MusicManager.Instance.songTime / 1000f);
        int minutes = seconds / 60;
		seconds %= 60;

		if (MusicManager.Instance.songTime < 0) {
			songProgressLabel.Text = $"-{minutes:00}:{-seconds:00}";
		} else {
			songProgressLabel.Text = $"{minutes:00}:{seconds:00}";
		}

		int remaining = Math.Max(chartManager.chartLength - MusicManager.Instance.songTime, 0);
		seconds = (int)(remaining / 1000f);
        minutes = seconds / 60;
		seconds %= 60;

		songRemainingLabel.Text = $"-{minutes:00}:{seconds:00}";
    }
}
