using Godot;

[GlobalClass]
public partial class StyleSettings : Resource
{
    [ExportGroup("Playfield")]
    [Export] public int playfieldWidth = 500;
    [Export] public int playfieldPadding = -30;
    [Export] public float playfieldOpacity = 1f;

    [ExportGroup("Background")]
    [Export] public float backgroundOpacity = 0.2f;

    [ExportGroup("Receptors")]
    [Export] public float receptorScale = 1f;

	[ExportGroup("Judgements")]
	[Export] public float judgementPosY = 0f;
	[ExportGroup("Combo")]
	[Export] public float comboPosY = 0f;
	[Export] public int comboSize = 70;
}
