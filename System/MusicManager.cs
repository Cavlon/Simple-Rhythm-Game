using Godot;
using System.IO;

public partial class MusicManager : AudioStreamPlayer2D
{
	public static MusicManager Instance { get; private set; }

	public int songTime = -1000;

    public override void _Ready()
    {
        Instance = this;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (songTime < 0) {
			songTime += (int)(delta * 1000);

			if (songTime > 0) {
				songTime = 0;
				Play();
			}
		} else {
			songTime = (int)((GetPlaybackPosition() + (float)(AudioServer.GetTimeSinceLastMix() - AudioServer.GetOutputLatency())) * 1000);
		}
	}

	public void StartAudio(int startTime) {
		Stream = (AudioStream)GD.Load(DataManager.Instance.AudioPath);
		songTime = startTime;

		if (startTime > 0 && startTime < Stream.GetLength()*1000) {
			Play(startTime / 1000f);
		}
	}
}
