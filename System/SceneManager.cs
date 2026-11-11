using Godot;
public partial class SceneManager : Node
{
    // Singleton instance
    public static SceneManager Instance { get; private set; }

	[Export] PackedScene gameScene;

    public override void _Ready()
    {
        Instance = this;
    }

    public void ChangeScene(Scene scene)
    {
		switch (scene) {
			case Scene.Game:
				GetTree().ChangeSceneToPacked(gameScene);
				break;
		}
    }

	public enum Scene {
		MainMenu,
		SongSelect,
		Game
	}
}
