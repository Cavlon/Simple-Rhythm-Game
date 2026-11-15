using System.Threading.Tasks;
using Godot;
public partial class SceneManager : Node
{
    // Singleton instance
    public static SceneManager Instance { get; private set; }

	[Export] PackedScene gameScene;

	[Export] RichTextLabel mainLabel;
	[Export] RichTextLabel subLabel;

	[Export] ColorRect shaderRect;
	
	CanvasLayer transitionLayer;
	ShaderMaterial shaderMaterial;

    public override void _Ready()
    {
        Instance = this;
		shaderMaterial = (ShaderMaterial)shaderRect.Material;

		transitionLayer = GetNode<CanvasLayer>("CanvasLayer");
    }

    public static async void ChangeScene(Scene scene, float duration, string mainText = "", string subText = "")
    {
        PackedScene nextScene;

        switch (scene) {
			case Scene.Game:
				nextScene = Instance.gameScene;
				break;
			default:
				return;
		}

		await Instance.Transition(nextScene, duration, mainText, subText);
    }

	async Task Transition(PackedScene scene, float duration, string mainText, string subText) {
		transitionLayer.Visible = true;

		mainLabel.Text = mainText;
		subLabel.Text = subText;

		shaderMaterial.SetShaderParameter("progress", 0f);
		shaderMaterial.SetShaderParameter("invert", true);

		Tween transOut = CreateTween();
        transOut.TweenProperty(shaderRect, "material:shader_parameter/progress", 1.5f, duration)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.Out);
        await ToSignal(transOut, Tween.SignalName.Finished);

		GetTree().ChangeSceneToPacked(scene);

		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		shaderMaterial.SetShaderParameter("progress", 0f);
		shaderMaterial.SetShaderParameter("invert", false);

        Tween transIn = CreateTween();
        transIn.TweenProperty(shaderRect, "material:shader_parameter/progress", 1.5f, duration)
			.SetDelay(0.15f)
			.SetTrans(Tween.TransitionType.Back)
			.SetEase(Tween.EaseType.Out);
        await ToSignal(transIn, Tween.SignalName.Finished);

        transitionLayer.Visible = false;
	}

	public enum Scene {
		MainMenu,
		SongSelect,
		Game
	}
}
