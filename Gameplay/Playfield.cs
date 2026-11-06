using Godot;
using System;

public partial class Playfield : Node2D
{

	[Export] Sprite2D backgroundSprite;
	[Export] Node2D hitField;

	GameplaySettings _settings;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_settings = DataManager.GameplaySettings;
		backgroundSprite.Scale = new Vector2((float)_settings.playfieldWidth / backgroundSprite.Texture.GetWidth(), 1f);
		hitField.Position = new Vector2(0, _settings.receptorPosY);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
