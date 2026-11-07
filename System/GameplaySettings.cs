using Godot;

[GlobalClass]
public partial class GameplaySettings : Resource
{
    [ExportGroup("Playfield")]
    [Export] public int playfieldWidth = 500;
    [Export] public int playfieldPadding = -30;

    [ExportGroup("Receptors")]
	[Export] public int receptorPosY = 270;
	[Export] public int hitOffset = 0;
    [Export] public float receptorScale = 1f;

    [ExportGroup("Notes")]
    [Export] public int scrollSpeed = 20;
    [Export] public int notePoolSize = 100;

    [ExportGroup("Judgements")]
    [Export] public float marvelousWindow = 18f;
    [Export] public float perfectWindow = 43f;
    [Export] public float greatWindow = 76f;
    [Export] public float missWindow = 164f;

    [ExportGroup("Debug")]
    [Export] public bool displayHitfield = false;
}
