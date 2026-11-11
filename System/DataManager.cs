using Godot;

public partial class DataManager : Node
{
    public static DataManager Instance { get; private set; }
    public static GameplaySettings GameplaySettings { get; private set; }
    public static StyleSettings StyleSettings { get; private set; }

    public string SongsFolder;
    public string MapsetPath;
    public string MapPath;
    public string BackgroundPath;
    public string AudioPath;
    public int MapsetId;

    public override void _Ready()
    {
        Instance = this;
        GameplaySettings = GD.Load<GameplaySettings>("res://System/GameplaySettings.tres");
        StyleSettings = GD.Load<StyleSettings>("res://System/StyleSettings.tres");

        SongsFolder = ProjectSettings.GlobalizePath("res://Assets/Songs");
		UpdateMapset(563);
		UpdateMap(2044);
        UpdateMapAssets("Princess of Winter.png", "audio.mp3");
    }

    public void UpdateMapset(int mapsetId) {
        MapsetPath = SongsFolder.PathJoin(mapsetId.ToString());
        MapsetId = mapsetId;
    }

    public void UpdateMap(int mapId) {
        MapPath = MapsetPath.PathJoin($"{mapId}.qua");
    }

    public void UpdateMapAssets(string backgroundFile, string audioFile) {
        BackgroundPath = MapsetPath.PathJoin(backgroundFile);
        AudioPath = MapsetPath.PathJoin(audioFile);
    }
}
