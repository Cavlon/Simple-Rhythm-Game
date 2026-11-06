using Godot;
using System;

public partial class Note : Node2D
{

	Vector2 notePos;
	public bool alive;

	GameplaySettings _settings;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_settings = DataManager.GameplaySettings;
		Disable();
	}

    public override void _Process(double delta)
    {
		notePos.Y += _settings.scrollSpeed * (float)delta;
		Position = notePos;
    }

	public void Spawn(int spawnPosX, float spawnPosY) {
		Visible = true;
		alive = true;
		notePos.X = spawnPosX;
		notePos.Y = spawnPosY;
		Position = notePos;
		SetProcess(true);
	}

	public void Disable() {
		Visible = false;
		SetProcess(false);
	}
}
