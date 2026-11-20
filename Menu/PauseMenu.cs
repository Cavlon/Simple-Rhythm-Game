using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	MenuItem[] menuItems = new MenuItem[3];
	Control menuHolder;

	GameManager gameManager;

	public int selectedId = 0;

	Tween fadeTween;

	public void Initialise(GameManager _gameManager)
	{
		gameManager = _gameManager;

		menuHolder = GetNode<Control>("Control");
		Node itemHolder = menuHolder.GetNode("Items");

		menuItems[0] = itemHolder.GetNode<MenuItem>("Continue");
		menuItems[1] = itemHolder.GetNode<MenuItem>("Retry");
		menuItems[2] = itemHolder.GetNode<MenuItem>("Return");

		menuItems[0].Clicked += Continue;
		menuItems[1].Clicked += Retry;
		menuItems[2].Clicked += Return;

		for (int i = 0; i < 3; i++) {
			int ind = i;
			menuItems[i].MouseEntered += () => Selected(ind);
		}

		Selected(0);
	}

	void Continue() {
		gameManager.Pause();
	}

	void Retry() {
		MusicManager.Instance.StopAudio();
        SceneManager.ChangeScene(SceneManager.Scene.Game, 1f);
	}

	void Return() {
		MusicManager.Instance.StopAudio();
		SceneManager.ChangeScene(SceneManager.Scene.SongSelect, 1f);
	}

	public void Selected(int id) {
		if (id == -1 || id == 3) return;
		menuItems[selectedId].OnUnHover();
		menuItems[id].OnHover();
		selectedId = id;
	}

	public void Select() {
		switch (selectedId) {
			case 0:
				Continue();
				break;
			case 1:
				Retry();
				break;
			case 2:
				Return();
				break;
		}
	}

	public void MenuFade(bool fadeIn) {
		if (fadeTween != null && fadeTween.IsRunning()) {
            fadeTween.Kill();
        }

		float fade = 1f;

		if (!fadeIn) {
			fade = 0f;
		} else Visible = true;

		fadeTween = CreateTween();

        fadeTween.TweenProperty(menuHolder, "modulate:a", fade, 0.1f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.In);
		
		if (!fadeIn) fadeTween.TweenCallback(Callable.From(() => Visible = false));
	}
}
