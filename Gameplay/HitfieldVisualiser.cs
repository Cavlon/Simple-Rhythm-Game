using Godot;
using System;

public partial class HitfieldVisualiser : Node2D
{

	GameplaySettings _settings;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_settings = DataManager.GameplaySettings;
		Position = new Vector2(0, -_settings.hitOffset);
	}

	public override void _Draw()
    {
        if (!_settings.displayHitfield) return;

        float scale = _settings.scrollSpeed / 1000f;

        float marvelousSize = _settings.marvelousWindow * scale;
        float perfectSize = _settings.perfectWindow * scale;
        float greatSize = _settings.greatWindow * scale;
        float missSize = _settings.missWindow * scale;

        DrawRect(
            new Rect2(-_settings.playfieldWidth/2, -missSize, _settings.playfieldWidth, missSize * 2),
            new Color(1.0f, 0.0f, 0.0f, 0.05f)
        );

        DrawRect(
            new Rect2(-_settings.playfieldWidth/2, -greatSize, _settings.playfieldWidth, greatSize * 2),
            new Color(0.0f, 1.0f, 0.0f, 0.15f)
        );

        DrawRect(
            new Rect2(-_settings.playfieldWidth/2, -perfectSize, _settings.playfieldWidth, perfectSize * 2),
            new Color(1.0f, 1.0f, 0.0f, 0.25f)
        );

		DrawRect(
            new Rect2(-_settings.playfieldWidth/2, -marvelousSize, _settings.playfieldWidth, marvelousSize * 2),
            new Color(1.0f, 1.0f, 1.0f, 0.35f)
        );

        // 4. Draw the core receptor line right across the middle
        DrawLine(
            new Vector2(-_settings.playfieldWidth/2, 0), 
            new Vector2(_settings.playfieldWidth/2, 0), 
            Colors.Red, 
            2.0f
        );
    }
}
