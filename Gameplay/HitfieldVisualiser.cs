using Godot;
using System;

public partial class HitfieldVisualiser : Node2D
{

	GameplaySettings _gameSettings;
    StyleSettings _styleSettings;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_gameSettings = DataManager.GameplaySettings;
        _styleSettings = DataManager.StyleSettings;
		Position = new Vector2(0, -_gameSettings.hitOffset);
	}

	public override void _Draw()
    {
        if (!_gameSettings.displayHitfield) return;

        float scale = _gameSettings.scrollSpeed / 1000f;

        float marvelousSize = _gameSettings.marvelousWindow * scale;
        float perfectSize = _gameSettings.perfectWindow * scale;
        float greatSize = _gameSettings.greatWindow * scale;
        float missSize = _gameSettings.missWindow * scale;

        DrawRect(
            new Rect2(-_styleSettings.playfieldWidth/2, -missSize, _styleSettings.playfieldWidth, missSize * 2),
            new Color(1.0f, 0.0f, 0.0f, 0.05f)
        );

        DrawRect(
            new Rect2(-_styleSettings.playfieldWidth/2, -greatSize, _styleSettings.playfieldWidth, greatSize * 2),
            new Color(0.0f, 1.0f, 0.0f, 0.15f)
        );

        DrawRect(
            new Rect2(-_styleSettings.playfieldWidth/2, -perfectSize, _styleSettings.playfieldWidth, perfectSize * 2),
            new Color(1.0f, 1.0f, 0.0f, 0.25f)
        );

		DrawRect(
            new Rect2(-_styleSettings.playfieldWidth/2, -marvelousSize, _styleSettings.playfieldWidth, marvelousSize * 2),
            new Color(1.0f, 1.0f, 1.0f, 0.35f)
        );

        // 4. Draw the core receptor line right across the middle
        DrawLine(
            new Vector2(-_styleSettings.playfieldWidth/2, 0), 
            new Vector2(_styleSettings.playfieldWidth/2, 0), 
            Colors.Red, 
            2.0f
        );
    }
}
