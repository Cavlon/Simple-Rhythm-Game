using Godot;
using System;

public partial class Note : Node2D
{

	Vector2 notePos;
	public bool alive;
	
	Sprite2D sprite;

	GameplaySettings _gameSettings;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_gameSettings = DataManager.GameplaySettings;
		sprite = GetNode<Sprite2D>("Sprite2D");
		Disable();
	}

    public override void _Process(double delta)
    {
		notePos.Y += _gameSettings.scrollSpeed * (float)delta;
		Position = notePos;
    }

	public void Spawn(int spawnPosX, float spawnPosY, Texture2D noteTexture) {
		Visible = true;
		alive = true;
		notePos.X = spawnPosX;
		notePos.Y = spawnPosY;
		Position = notePos;
		sprite.Texture = noteTexture;
		SetProcess(true);
	}

	public void Disable() {
		Visible = false;
		SetProcess(false);
	}
}
