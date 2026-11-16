using Godot;
using System.Threading.Tasks;

public static class AssetLoader
{
    public static async Task<T> LoadResourceAsync<T>(string path) where T : Resource
    {
        Error err = ResourceLoader.LoadThreadedRequest(path);
        if (err != Error.Ok) {
            GD.PrintErr($"Failed to request resource at {path}: {err}");
            return null;
        }

        SceneTree tree = (SceneTree)Engine.GetMainLoop();
        if (tree == null) return null;
		GD.Print("loading");


        while (ResourceLoader.LoadThreadedGetStatus(path) == ResourceLoader.ThreadLoadStatus.InProgress) {
            await tree.ToSignal(tree, SceneTree.SignalName.ProcessFrame);
        }

        if (ResourceLoader.LoadThreadedGetStatus(path) == ResourceLoader.ThreadLoadStatus.Loaded) {
			GD.Print("loaded");
            return (T)ResourceLoader.LoadThreadedGet(path);
        }

        return null;
    }
}
