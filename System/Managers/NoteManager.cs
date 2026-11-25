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

	int scrollSpeed;

	int[] spawnPositions = new int[4];
	int endPosY;

	Queue<(int, int)>[] activeNotes = new Queue<(int, int)>[4];

	Note[] notePool;
	int poolInd = 0;
	int poolCount;

	float[] judgements = new float[4];

	[Signal]
    public delegate void NoteJudgedEventHandler(int judgement, float errorTime);

	public void Initialise(int screenHeight)
	{
		_gameSettings = DataManager.GameplaySettings;
		_styleSettings = DataManager.StyleSettings;

		scrollSpeed = _gameSettings.scrollSpeed;

		endPosY = (int)Position.Y + screenHeight*2;

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
			note.Initialise(endPosY);
			note.Scale = Vector2.One * _styleSettings.receptorScale * 0.5f;
			notePool[i] = note;
		}

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
                (int, int) activeNote = activeNotes[i].Peek();
				int error = MusicManager.Instance.songTime - activeNote.Item2;

                if (error < judgements[2]) 
                    break;
				
				Note note = notePool[activeNote.Item1];

				EmitSignal(SignalName.NoteJudged, 3, judgements[3]);
                activeNotes[i].Dequeue();
                note.Kill();
                poolCount++;
            }
        }
	}

	public void SpawnNote(int lane, int hitTime) {
		if (poolCount == 0) {
			GD.PrintErr("Note Pool Empty");
			return;
		}

		activeNotes[lane].Enqueue((poolInd, hitTime));
		notePool[poolInd].Spawn(spawnPositions[lane], Position.Y, noteTextures[lane]);

		poolInd = (poolInd + 1) % _gameSettings.notePoolSize;
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

		int error = MusicManager.Instance.songTime - activeNote.Item2;
		int dist = Math.Abs(error);

		for (int i = 0; i < 4; i++) {
			if (dist < judgements[i]) {
				EmitSignal(SignalName.NoteJudged, i, error);

				activeNotes[lane].Dequeue();
				note.Disable();
				poolCount++;
				return;
			}
		}
	}
}
