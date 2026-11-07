using Godot;
using System.IO;

public partial class MusicManager : AudioStreamPlayer2D
{

	public int songTime = -1000;
	bool songStarted = false;

	// Called when the node enters the scene tree for the first time.
	public void Initialise()
	{
		string[] mp3Files = Directory.GetFiles(ProjectSettings.GlobalizePath(DataManager.Instance.SelectedSongFolderPath), "*.mp3");
		Stream = (AudioStream)GD.Load(mp3Files[0]);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!songStarted) {
			songTime += (int)(delta * 1000);

			if (songTime > 0) {
				songTime = 0;
				Play();
				songStarted = true;
			}
		} else {
			songTime = (int)((GetPlaybackPosition() + (float)(AudioServer.GetTimeSinceLastMix() - AudioServer.GetOutputLatency())) * 1000);
		}
	}
}
