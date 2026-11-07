using System.Collections.Generic;
using System.IO;
using Godot;
using YamlDotNet.Serialization;

public class QuaChart
{
    [YamlMember(Alias = "AudioFile")]
    public string AudioFile { get; set; }

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
	public QuaChart ParseQua(string songPath, int songId) {
		string quaFile = File.ReadAllText($"{songPath}/{songId}.qua");

		QuaChart chart = deserializer.Deserialize<QuaChart>(quaFile);

		GD.Print($"Loaded: {chart.SongTitle} by {chart.Artist}");
        GD.Print($"Total Notes Found: {chart.HitObjects.Count}");

		return chart;
	}
}
