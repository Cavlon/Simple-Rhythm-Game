using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public partial class SongSelect : CanvasLayer
{
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
	}

	// Update the background and audio through asynchronous loading
	public void UpdateMapAssets(int songStart) {
		if (backgroundPath != DataManager.Instance.BackgroundPath) {
			bgTokenSource?.Cancel();
			bgTokenSource?.Dispose();
			bgTokenSource = new CancellationTokenSource();
			CancellationToken token = bgTokenSource.Token;

			_ = UpdateBackground(token);
		}

		if (audioPath != DataManager.Instance.AudioPath) {
			audioTokenSource?.Cancel();
			audioTokenSource?.Dispose();
			audioTokenSource = new CancellationTokenSource();
			CancellationToken token = audioTokenSource.Token;

			_ = UpdateAudio(songStart, token);
		}
		
	}

	async Task UpdateBackground(CancellationToken token) {
		Image img = await ImageHandler.LoadImage(DataManager.Instance.BackgroundPath, viewportDims, token);
		DataManager.Instance.backgroundTexture = ImageTexture.CreateFromImage(img);
		background.Texture = DataManager.Instance.backgroundTexture;
		backgroundPath = DataManager.Instance.BackgroundPath;
	}

	async Task UpdateAudio(int songStart, CancellationToken token) {
		await MusicManager.Instance.StartAudio(songStart, token);
		audioPath = DataManager.Instance.AudioPath;
	}
}
