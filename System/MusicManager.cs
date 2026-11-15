using Godot;
using System.IO;

public partial class MusicManager : AudioStreamPlayer2D
{
	public static MusicManager Instance { get; private set; }

	public int songTime = -10000;

	int latency;

	bool songPlaying = false;
	bool audioPlaying = false;

    public override void _Ready()
    {
        Instance = this;
		latency = (int)(AudioServer.GetOutputLatency() * 1000);
		GD.Print($"Latency: {latency}");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (songPlaying) {
			if (audioPlaying) {
				songTime = (int)((GetPlaybackPosition() + (float)AudioServer.GetTimeSinceLastMix()) * 1000) - latency;
			} else {
				songTime += (int)(delta * 1000);

				if (songTime > -latency) {
					GD.Print("Audio Started");
					audioPlaying = true;
					Play();
				}
			}
		}
	}

	public void StartAudio(int startTime) {
		Stream = (AudioStream)GD.Load(DataManager.Instance.AudioPath);
		songTime = startTime;
		audioPlaying = false;

		if (startTime >= 0 && startTime < Stream.GetLength()*1000) {
			Play(startTime / 1000f);
			audioPlaying = true;
		}

		songPlaying = true;
	}
}
