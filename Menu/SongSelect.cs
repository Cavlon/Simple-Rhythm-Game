using Godot;
using System;
using System.Collections.Generic;

public partial class SongSelect : CanvasLayer
{

	[Export] int cacheLimit = 20;

	SongList songList;

	TextureRect background;

	string backgroundPath = "";
	string audioPath = "";

	Dictionary<string, Texture2D> imageCache = [];
	LinkedList<string> textureHistory = [];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		songList = GetNode<SongList>("SongList");

		background = GetNode<TextureRect>("Background");
		background.Modulate = new Color(Colors.White, DataManager.StyleSettings.backgroundOpacity);

		songList.Initialise(this);
	}

	// ADD LAZY LOADING WITH BACKGROUND THREADS
	public Texture2D GetBackground(string backgroundPath) {
		if (imageCache.TryGetValue(backgroundPath, out var cachedTexture))
        {
			textureHistory.Remove(backgroundPath);
        	textureHistory.AddFirst(backgroundPath);
            return cachedTexture;
        } else {
			Image img = Image.LoadFromFile(DataManager.Instance.SongsFolder.PathJoin(backgroundPath));

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

			if (textureHistory.Count >= cacheLimit) {
				LinkedListNode<string> oldestNode = textureHistory.Last;
				GD.Print($"Removing {oldestNode.Value}");
				imageCache.Remove(oldestNode.Value);
				textureHistory.RemoveLast();
			}

			GD.Print($"Adding {backgroundPath}");

			Texture2D texture = ImageTexture.CreateFromImage(img);
			LinkedListNode<string> newNode = new(backgroundPath);
			textureHistory.AddFirst(newNode);
			imageCache[backgroundPath] = texture;
			return texture;
		}
	}

	public void UpdateMapAssets(int songStart) {
		if (backgroundPath == DataManager.Instance.BackgroundPath) return;

		DataManager.Instance.backgroundTexture = GetBackground(DataManager.Instance.BackgroundPath);
		background.Texture = DataManager.Instance.backgroundTexture;
		backgroundPath = DataManager.Instance.BackgroundPath;

		if (audioPath == DataManager.Instance.AudioPath) return;
		MusicManager.Instance.StartAudio(songStart);
		audioPath = DataManager.Instance.AudioPath;
	}
}
