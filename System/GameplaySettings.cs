using Godot;

[GlobalClass]
public partial class GameplaySettings : Resource
{
    [ExportGroup("Receptors")]
	[Export] public int receptorPosY = 270;
	[Export] public int hitOffset = 0;

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
