using Godot;
using System;

public partial class Playfield : Node2D
{

	[Export] Sprite2D backgroundSprite;

	public NoteManager noteManager;
	public HitField hitField;
	public GameManager gameManager;

	GameplaySettings _settings;

	// Called when the node enters the scene tree for the first time.
	public void Initialise(GameManager _gameManager)
	{
		_settings = DataManager.GameplaySettings;
		gameManager = _gameManager;

		noteManager = GetNode<NoteManager>("NoteManager");
		hitField = GetNode<HitField>("Hitfield");

		noteManager.Initialise(gameManager.musicManager, hitField);

		backgroundSprite.Scale = new Vector2((float)_settings.playfieldWidth / backgroundSprite.Texture.GetWidth(), 1f);

		Vector2 viewportSize = GetViewportRect().Size;

		hitField.Position = new Vector2(0, viewportSize.Y / 2 - _settings.receptorPosY);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
