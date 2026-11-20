using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

public static class ImageHandler
{
	static Dictionary<string, Texture2D> imageCache = [];
	static Dictionary<string, Task<Texture2D>> currentlyLoading = [];
	static LinkedList<string> textureHistory = [];

	static int cacheLimit = 20;
	static (int, int) thumbnailDims = (600, 150);

	public static Task<Texture2D> GetCachedImage(string imagePath, CancellationToken token = default) {
		if (imageCache.TryGetValue(imagePath, out var cachedTexture)) {
			textureHistory.Remove(imagePath);
        	textureHistory.AddFirst(imagePath);
            return Task.FromResult(cachedTexture);
		} 
		
		if (currentlyLoading.TryGetValue(imagePath, out var loadingTask)) {
			return loadingTask;		
        }
		
		Task<Texture2D> loadTask = CacheImage(imagePath, token);
		currentlyLoading[imagePath] = loadTask;
		return loadTask;
	}

	static async Task<Texture2D> CacheImage(string imagePath, CancellationToken token) {
		// Load and resize the image in the background
		Image img = await LoadImage(imagePath, thumbnailDims, token);

		// Adhere to the cache limit by removing the oldest entry
		if (textureHistory.Count >= cacheLimit) {
			LinkedListNode<string> oldestNode = textureHistory.Last;
			GD.Print($"Removing {oldestNode.Value}");
			imageCache.Remove(oldestNode.Value);
			textureHistory.RemoveLast();
		}

		GD.Print($"Adding {imagePath}");

		Texture2D texture = ImageTexture.CreateFromImage(img);
		LinkedListNode<string> newNode = new(imagePath);

		textureHistory.AddFirst(newNode);
		imageCache[imagePath] = texture;

		currentlyLoading.Remove(imagePath);
		return texture;
	}

	public static Task<Image> LoadImage(string imagePath, (int, int) maxDims, CancellationToken token) {
		return Task.Run(() => {
			Image img = Image.LoadFromFile(DataManager.Instance.SongsFolder.PathJoin(imagePath));

			if (token.IsCancellationRequested) {
                img.Dispose();
                return null;
            }

			Vector2I imgSize = img.GetSize();

			if (imgSize.X > maxDims.Item1 || imgSize.Y > maxDims.Item2) {
				float widthRatio = (float)maxDims.Item1 / imgSize.X;
				float heightRatio = (float)maxDims.Item2 / imgSize.Y;
				float scaleFactor = Mathf.Min(widthRatio, heightRatio);

				int newWidth = Mathf.RoundToInt(imgSize.X * scaleFactor);
				int newHeight = Mathf.RoundToInt(imgSize.Y * scaleFactor);

				img.Resize(newWidth, newHeight, Image.Interpolation.Lanczos);
			}

			return img;
		}, token);
	}
}
