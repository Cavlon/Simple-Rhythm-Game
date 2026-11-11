using System.Collections.Generic;
using System.IO;
using Godot;
using YamlDotNet.Serialization;

public class QuaChart
{
    [YamlMember(Alias = "Title")]
    public string SongTitle { get; set; }

    [YamlMember(Alias = "Artist")]
    public string Artist { get; set; }

	[YamlMember(Alias = "Creator")]
    public string Creator { get; set; }

	[YamlMember(Alias = "TimingPoints")]
    public List<TimingPoint> TimingPoints { get; set; }

    [YamlMember(Alias = "HitObjects")]
    public List<QuaNote> HitObjects { get; set; }
}

public class QuaDetails
{
    [YamlMember(Alias = "MapId")]
    public int MapId { get; set; }

    [YamlMember(Alias = "DifficultyName")]
    public string DifficultyName { get; set; }
}

public class QuaMapset
{
    [YamlMember(Alias = "MapSetId")]
    public int MapSetId { get; set; }

    [YamlMember(Alias = "Title")]
    public string SongTitle { get; set; }

    [YamlMember(Alias = "Artist")]
    public string Artist { get; set; }

	[YamlMember(Alias = "Creator")]
    public string Creator { get; set; }
    
    [YamlMember(Alias = "SongPreviewTime")]
    public int SongPreviewTime { get; set; }

    [YamlMember(Alias = "BackgroundFile")]
    public string BackgroundFile { get; set; }

    [YamlMember(Alias = "AudioFile")]
    public string AudioFile { get; set; }

    public int MapCount = 0;
    public int[] MapIds;
    public string[] Difficulties;
}

public class QuaNote
{
    [YamlMember(Alias = "StartTime")]
    public int StartTime { get; set; }

    [YamlMember(Alias = "Lane")]
    public int Lane { get; set; }
    
    [YamlMember(Alias = "EndTime")]
    public float EndTime { get; set; }
}

public class TimingPoint
{
    [YamlMember(Alias = "StartTime")]
    public float StartTime { get; set; }

    [YamlMember(Alias = "Bpm")]
    public float Bpm { get; set; }
}

public partial class Parser
{
	IDeserializer deserializer;

	public Parser() {
		deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .Build();
	}
	public QuaChart ParseQuaChart(string songPath) {
		string quaFile = File.ReadAllText(songPath);

		QuaChart chart = deserializer.Deserialize<QuaChart>(quaFile);

		GD.Print($"Loaded: {chart.SongTitle} by {chart.Artist}");
        GD.Print($"Total Notes Found: {chart.HitObjects.Count}");

		return chart;
	}

    public QuaMapset ParseQuaMapset(string mapsetPath) {
        QuaMapset mapset = null;
        string[] quaFiles = Directory.GetFiles(mapsetPath, "*.qua");

        for (int i = 0; i < quaFiles.Length; i++) {
            GD.Print($"Found file: {quaFiles[i]}");

            string quaFile = File.ReadAllText(quaFiles[i]);

            if (mapset is null) {
                mapset = deserializer.Deserialize<QuaMapset>(quaFile);
                mapset.MapIds = new int[quaFiles.Length];
                mapset.Difficulties = new string[quaFiles.Length];
            }
            
            QuaDetails map = deserializer.Deserialize<QuaDetails>(quaFile);

            mapset.MapIds[i] = map.MapId;
            mapset.Difficulties[i] = map.DifficultyName;
            mapset.MapCount++;
        }

		return mapset;
	}

    public List<QuaMapset> ParseAllMapsets(string songsFolder) {
        List<QuaMapset> mapsets = [];
        string[] directories = Directory.GetDirectories(songsFolder);

        for (int i = 0; i < directories.Length; i++) {
            QuaMapset mapset = ParseQuaMapset(directories[i]);
            if (mapset is not null) {
                mapsets.Add(mapset);
            }
        }

        return mapsets;
    }
}
