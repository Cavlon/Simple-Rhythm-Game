using System;
using System.Threading.Tasks;
using Godot;

public interface IInitialisable {
    void Init(object data);
}

public partial class SceneManager : Node
{
    // Singleton instance
    public static SceneManager Instance { get; private set; }

	[Export] PackedScene gameScene;
	[Export] PackedScene songSelectScene;
	[Export] PackedScene resultsScene;

	[Export] RichTextLabel mainLabel;
	[Export] RichTextLabel subLabel;

	[Export] ColorRect shaderRect;
	
	CanvasLayer transitionLayer;
	ShaderMaterial shaderMaterial;
	Tween transitionTween;

    public override void _Ready()
    {
        Instance = this;
		shaderMaterial = (ShaderMaterial)shaderRect.Material;

		transitionLayer = GetNode<CanvasLayer>("CanvasLayer");
    }

	public static async void ChangeScene(Scene scene, float duration, string mainText = "", string subText = "", object data = null)
    {
		GD.Print($"Changing Scene to {scene}");
        PackedScene nextScene = Instance.GetScene(scene);
		if (nextScene == null) return;

		await Instance.Transition(nextScene, duration, mainText, subText, onInstantiate: node =>
		{
			if (data != null && node is IInitialisable initNode)
				initNode.Init(data);
		});
    }

	async Task Transition(PackedScene scene, float duration, string mainText, string subText, Action<Node> onInstantiate) {
		if (transitionTween != null && transitionTween.IsRunning()) {
            transitionTween.Kill();
        }

		transitionLayer.Visible = true;

		mainLabel.Text = mainText;
		subLabel.Text = subText;

		shaderMaterial.SetShaderParameter("progress", 0f);
		shaderMaterial.SetShaderParameter("invert", true);

		transitionTween = CreateTween();
        transitionTween.TweenProperty(shaderRect, "material:shader_parameter/progress", 1.5f, duration)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.Out);
        await ToSignal(transitionTween, Tween.SignalName.Finished);

		UpdateScene(scene, onInstantiate);

		GetTree().Paused = false;
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		shaderMaterial.SetShaderParameter("progress", 0f);
		shaderMaterial.SetShaderParameter("invert", false);

        transitionTween = CreateTween();
        transitionTween.TweenProperty(shaderRect, "material:shader_parameter/progress", 1.5f, duration)
			.SetDelay(0.15f)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.Out);
        await ToSignal(transitionTween, Tween.SignalName.Finished);

        transitionLayer.Visible = false;
	}

	void UpdateScene(PackedScene scene, Action<Node> onInstantiate) {
		Node nextScene = scene.Instantiate();
        onInstantiate?.Invoke(nextScene);

        Node oldScene = GetTree().CurrentScene;
        GetTree().Root.AddChild(nextScene);
        GetTree().CurrentScene = nextScene;
        oldScene?.QueueFree();
	}

	PackedScene GetScene(Scene scene) {
        return scene switch
        {
            Scene.Game => Instance.gameScene,
            Scene.SongSelect => Instance.songSelectScene,
            Scene.Results => Instance.resultsScene,
            _ => null,
        };
    }

	public enum Scene {
		MainMenu,
		SongSelect,
		Game,
		Results
	}
}
