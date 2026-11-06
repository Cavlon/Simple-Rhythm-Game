using Godot;
using System;

public partial class HitField : Node2D
{

	[Export] Receptor[] receptors;
	[Export] Texture2D[] upTextures;
	[Export] Texture2D[] downTextures;
	[Export] Sprite2D hitline;

	GameplaySettings _settings;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_settings = DataManager.GameplaySettings;

		int paddedWidth = _settings.playfieldWidth - _settings.playfieldPadding * 2;

		int spacing = paddedWidth/5;
		int pos = -_settings.playfieldWidth/2 + _settings.playfieldPadding + spacing;
		for (int i = 0; i < 4; i++) {
			receptors[i].Position = new Vector2(pos, 0);
			receptors[i].Scale = Vector2.One * _settings.receptorScale;
			pos += spacing;
		}
	}

	public void ReceptorDown(int lane) {
		receptors[lane].SetTexture(downTextures[lane]);
	}

	public void ReceptorUp(int lane) {
		receptors[lane].SetTexture(upTextures[lane]);
	}
}
