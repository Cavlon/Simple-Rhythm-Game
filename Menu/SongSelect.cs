using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public partial class SongSelect : CanvasLayer
{
	[Export] float debounceTime;
	float debounceTimer = 0;

	SongList songList;

	TextureRect background;

	string backgroundPath = "";
	string audioPath = "";

	(int, int) viewportDims;

	CancellationTokenSource bgTokenSource;
	CancellationTokenSource audioTokenSource;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		songList = GetNode<SongList>("SongList");

		background = GetNode<TextureRect>("Background");
		background.Modulate = new Color(Colors.White, DataManager.StyleSettings.backgroundOpacity);
		viewportDims = (ProjectSettings.GetSetting("display/window/size/viewport_width").AsInt32(), ProjectSettings.GetSetting("display/window/size/viewport_height").AsInt32());

		songList.Initialise(this);

		Input.MouseMode = Input.MouseModeEnum.Visible;
	}

    public override void _Process(double delta)
    {
        if (debounceTimer > 0) {
			debounceTimer -= (float)delta;

			if (audioPath != DataManager.Instance.AudioPath && debounceTimer <= 0) {
				SetAudio();	
			}
		}
    }

	// Update the background and audio through asynchronous loading
	public void UpdateMapAssets() {
		if (backgroundPath != DataManager.Instance.BackgroundPath) {
			backgroundPath = DataManager.Instance.BackgroundPath;

			bgTokenSource?.Cancel();
			bgTokenSource?.Dispose();
			bgTokenSource = new CancellationTokenSource();
			CancellationToken bgToken = bgTokenSource.Token;

			_ = UpdateBackground(bgToken);
		}

		debounceTimer = debounceTime;
	}

	void SetAudio() {
		audioPath = DataManager.Instance.AudioPath;

		audioTokenSource?.Cancel();
		audioTokenSource?.Dispose();
		audioTokenSource = new CancellationTokenSource();
		CancellationToken audioToken = audioTokenSource.Token;

		_ = UpdateAudio(audioToken);
	}

	async Task UpdateBackground(CancellationToken token) {
		Image img = await ImageHandler.LoadImage(DataManager.Instance.BackgroundPath, viewportDims, token);

		if (img == null) return;

		if (token.IsCancellationRequested) {
            img.Dispose();
            return;
        }

		DataManager.Instance.backgroundTexture = ImageTexture.CreateFromImage(img);
		background.Texture = DataManager.Instance.backgroundTexture;
	}

	async Task UpdateAudio(CancellationToken token) {
		await MusicManager.Instance.StartAudio(DataManager.Instance.Mapset.SongPreviewTime, token);
	}
}
