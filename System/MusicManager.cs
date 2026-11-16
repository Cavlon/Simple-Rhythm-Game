using Godot;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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

	public async Task StartAudio(int startTime, CancellationToken token = default) {
		// Debounce
		await Task.Delay(100, token);

		AudioStreamMP3 stream = await Task.Run(() => {
			return AudioStreamMP3.LoadFromFile(DataManager.Instance.AudioPath);
		}, token);

		if (token.IsCancellationRequested) {
			stream.Dispose();
			return;
		}

		Stop();
    	Stream?.Dispose();

		Stream = stream;
		songTime = startTime;
		audioPlaying = false;

		if (startTime >= 0 && startTime < Stream.GetLength()*1000) {
			Play(startTime / 1000f);
			audioPlaying = true;
		}

		songPlaying = true;
	}

	public void StopAudio() {
		Stop();
		songTime = -10000;
		audioPlaying = false;
		songPlaying = false;
	}
}
