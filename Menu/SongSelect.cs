using Godot;
using System;
using System.Collections.Generic;

public partial class SongSelect : CanvasLayer
{

	SongList songList;

	TextureRect background;

	string backgroundPath = "";
	string audioPath = "";

	Dictionary<int, Texture2D> imageCache = [];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		songList = GetNode<SongList>("SongList");

		background = GetNode<TextureRect>("Background");
		background.Modulate = new Color(Colors.White, DataManager.StyleSettings.backgroundOpacity);

		songList.Initialise(this);
	}

	public Texture2D GetBackground(string backgroundPath, int mapsetId) {
		if (imageCache.TryGetValue(mapsetId, out var cachedTexture))
        {
            return cachedTexture;
        } else {
			Image img = Image.LoadFromFile(backgroundPath);

			int maxWidth = ProjectSettings.GetSetting("display/window/size/viewport_width").AsInt32();
			int maxHeight = ProjectSettings.GetSetting("display/window/size/viewport_height").AsInt32();

			Vector2I imgSize = img.GetSize();

			if (imgSize.X > maxWidth || imgSize.Y > maxHeight)
			{
				float widthRatio = (float)maxWidth / imgSize.X;
				float heightRatio = (float)maxHeight / imgSize.Y;
				float scaleFactor = Mathf.Min(widthRatio, heightRatio);

				int newWidth = Mathf.RoundToInt(imgSize.X * scaleFactor);
				int newHeight = Mathf.RoundToInt(imgSize.Y * scaleFactor);

				img.Resize(newWidth, newHeight, Image.Interpolation.Lanczos);
			}

			Texture2D texture = ImageTexture.CreateFromImage(img);
			imageCache[mapsetId] = texture;
			return texture;
		}
	}

	public void UpdateMapAssets(int songStart) {
		if (backgroundPath == DataManager.Instance.BackgroundPath) return;

		background.Texture = GetBackground(DataManager.Instance.BackgroundPath, DataManager.Instance.MapsetId);
		backgroundPath = DataManager.Instance.BackgroundPath;

		if (audioPath == DataManager.Instance.AudioPath) return;
		MusicManager.Instance.StartAudio(songStart);
	}
}
