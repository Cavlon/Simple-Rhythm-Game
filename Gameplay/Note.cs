using Godot;
using System;

public partial class Note : Sprite2D
{

	Vector2 notePos;
	int resetPosY;
	
	Color deadColour = new(0.4f, 0.4f, 0.4f, 0.75f);

	int scrollSpeed;
	GameplaySettings _gameSettings;

	// Called when the node enters the scene tree for the first time.
	public void Initialise(int _resetPosY)
	{
		scrollSpeed = DataManager.GameplaySettings.scrollSpeed;
		resetPosY = _resetPosY;
		Disable();
	}

    public override void _Process(double delta)
    {
		notePos.Y += scrollSpeed * (float)delta;

		if (notePos.Y >= resetPosY) {
			Disable();
			return;
		}

		Position = notePos;
    }

	public void Spawn(int spawnPosX, float spawnPosY, Texture2D noteTexture) {
		Visible = true;
		Modulate = Colors.White;

		notePos.X = spawnPosX;
		notePos.Y = spawnPosY;
		Position = notePos;

		Texture = noteTexture;
		SetProcess(true);
	}

	public void Disable() {
		Visible = false;
		SetProcess(false);
	}

	public void Kill() {
		Modulate = deadColour;
	}
}
