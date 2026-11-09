using Godot;

[GlobalClass]
public partial class StyleSettings : Resource
{
    [ExportGroup("Playfield")]
    [Export] public int playfieldWidth = 500;
    [Export] public int playfieldPadding = -30;

    [ExportGroup("Receptors")]
    [Export] public float receptorScale = 1f;

	[ExportGroup("Judgements")]
	[Export] public float judgementPosY = 0f;
	[ExportGroup("Combo")]
	[Export] public float comboPosY = 0f;
	[Export] public int comboSize = 70;
}
