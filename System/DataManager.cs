using Godot;

public partial class DataManager : Node
{
    public static DataManager Instance { get; private set; }
    public static GameplaySettings GameplaySettings { get; private set; }
    public static StyleSettings StyleSettings { get; private set; }
    public string SelectedSongFolderPath { get; set; }
    public int SelectedChartId { get; set; }

    public override void _Ready()
    {
        Instance = this;
        GameplaySettings = GD.Load<GameplaySettings>("res://System/GameplaySettings.tres");
        StyleSettings = GD.Load<StyleSettings>("res://System/StyleSettings.tres");

		SelectedSongFolderPath = "res://Assets/Songs/563";
		SelectedChartId = 2044;
    }
}
