using Godot;
using System.IO;

public partial class MusicManager : AudioStreamPlayer2D
{

	public float songTime = -1f;
	bool songStarted = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		string[] mp3Files = Directory.GetFiles(ProjectSettings.GlobalizePath(DataManager.Instance.SelectedSongFolderPath), "*.mp3");
		Stream = (AudioStream)GD.Load(mp3Files[0]);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!songStarted) {
			songTime += (float)delta;

			if (songTime > 0f) {
				songTime = 0;
				Play();
				songStarted = true;
			}
		} else {
			songTime = GetPlaybackPosition() + (float)AudioServer.GetTimeSinceLastMix() - (float)AudioServer.GetOutputLatency();
		}
	}

	public float GetSongPos()
    {
        return songTime;
    }
}
