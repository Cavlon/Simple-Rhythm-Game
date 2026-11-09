using Godot;
using System;

public partial class HitField : Node2D
{

	[Export] Receptor[] receptors;
	[Export] Texture2D[] upTextures;
	[Export] Texture2D[] downTextures;

	StyleSettings _styleSettings;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_styleSettings = DataManager.StyleSettings;

		int paddedWidth = _styleSettings.playfieldWidth - _styleSettings.playfieldPadding * 2;

		int spacing = paddedWidth/5;
		int pos = -_styleSettings.playfieldWidth/2 + _styleSettings.playfieldPadding + spacing;
		for (int i = 0; i < 4; i++) {
			receptors[i].Position = new Vector2(pos, 0);
			receptors[i].Scale = Vector2.One * _styleSettings.receptorScale;
			receptors[i].SetTexture(upTextures[i]);
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
