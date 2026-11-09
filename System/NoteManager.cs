using Godot;
using System;
using System.Collections.Generic;

public partial class NoteManager : Node2D
{

	[Export] PackedScene noteScene;
	[Export] Node2D noteHolder;
	[Export] Texture2D[] noteTextures;

	GameplaySettings _gameSettings;
	StyleSettings _styleSettings;
	MusicManager musicManager;

	int scrollSpeed;

	int[] spawnPositions = new int[4];
	Note[] notePool;
	Queue<(int, int)>[] activeNotes = new Queue<(int, int)>[4];
	int pullInd = 0;
	int poolCount;

	HitField hitField;
	int hitFieldCenter;
	float[] judgements = new float[4];

	[Signal]
    public delegate void NoteJudgedEventHandler(int judgement, float errorTime);

	public void Initialise(MusicManager _musicManager, HitField _hitfield)
	{
		_gameSettings = DataManager.GameplaySettings;
		_styleSettings = DataManager.StyleSettings;

		scrollSpeed = _gameSettings.scrollSpeed;
		musicManager = _musicManager;
		hitField = _hitfield;

		int paddedWidth = _styleSettings.playfieldWidth - _styleSettings.playfieldPadding * 2;

		int spacing = paddedWidth/5;
		int pos = -_styleSettings.playfieldWidth/2 + _styleSettings.playfieldPadding + spacing;
		for (int i = 0; i < 4; i++) {
			activeNotes[i] = [];
			spawnPositions[i] = pos;
			pos += spacing;
		}

		poolCount = _gameSettings.notePoolSize;
		notePool = new Note[poolCount];

		for (int i = 0; i < poolCount; i++) {
			Note note = noteScene.Instantiate<Note>();
			noteHolder.AddChild(note);
			note.Scale = Vector2.One * _styleSettings.receptorScale;
			notePool[i] = note;
		}

		hitFieldCenter = (int)hitField.Position.Y - _gameSettings.hitOffset;

		judgements[0] = _gameSettings.marvelousWindow;
        judgements[1] = _gameSettings.perfectWindow;
        judgements[2] = _gameSettings.greatWindow;
        judgements[3] = _gameSettings.missWindow;
	}

	public override void _Process(double delta)
	{
		for (int i = 0; i < 4; i++)
        {
            while (activeNotes[i].Count > 0)
            {
                int poolIndex = activeNotes[i].Peek().Item1;
                Note note = notePool[poolIndex];

                if (note.Position.Y < 400) 
                    break;

				EmitSignal(SignalName.NoteJudged, 3, -1);
                activeNotes[i].Dequeue();
                note.Disable();
                poolCount++;
            }
        }
	}

	public void SpawnNote(int lane, int startTime) {
		if (poolCount == 0) {
			GD.PrintErr("Note Pool Empty");
		}

		activeNotes[lane].Enqueue((pullInd, startTime));
		notePool[pullInd].Spawn(spawnPositions[lane], Position.Y, noteTextures[lane]);

		pullInd = (pullInd + 1) % _gameSettings.notePoolSize;
		poolCount--;
	}

	public void DestroyNote(int lane) {
		if (activeNotes[lane].Count == 0) return;

		int poolIndex = activeNotes[lane].Dequeue().Item1;
		Note note = notePool[poolIndex];

		note.Disable();
		poolCount++;
	}

	public void LaneInput(int lane) {
		if (activeNotes[lane].Count == 0) return;

		(int, int) activeNote = activeNotes[lane].Peek();
		Note note = notePool[activeNote.Item1];

		float dist = Math.Abs(musicManager.songTime - activeNote.Item2);

		for (int i = 0; i < 4; i++) {
			if (dist < judgements[i]) {
				EmitSignal(SignalName.NoteJudged, i, dist);

				activeNotes[lane].Dequeue();
				note.Disable();
				poolCount++;
				return;
			}
		}
	}
}
