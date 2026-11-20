using System.Diagnostics.Contracts;
using Godot;

public partial class DataManager : Node
{
    public static DataManager Instance { get; private set; }
    public static GameplaySettings GameplaySettings { get; private set; }
    public static StyleSettings StyleSettings { get; private set; }

    public int listInd;
    public string SongsFolder;
    public string MapsetPath;
    public string MapPath;
    public string BackgroundPath;
    public string AudioPath;

    public QuaMapset Mapset;

    public Texture2D backgroundTexture;

    public override void _Ready()
    {
        Instance = this;
        GameplaySettings = GD.Load<GameplaySettings>("res://System/GameplaySettings.tres");
        StyleSettings = GD.Load<StyleSettings>("res://System/StyleSettings.tres");

        SongsFolder = ProjectSettings.GlobalizePath("res://Assets/Songs");
        listInd = -1;
    }

    public void UpdateMapset(QuaMapset mapset) {
        Mapset = mapset;
        MapsetPath = SongsFolder.PathJoin(Mapset.MapSetId.ToString());
    }

    public void UpdateMap(int mapId) {
        MapPath = MapsetPath.PathJoin($"{mapId}.qua");
    }

    public void UpdateMapAssets(string backgroundFile, string audioFile) {
        BackgroundPath = Mapset.MapSetId.ToString().PathJoin(backgroundFile);
        AudioPath = MapsetPath.PathJoin(audioFile);
    }
}
