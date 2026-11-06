using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.Emit;

public partial class NoteManager : Node2D
{

	[Export] PackedScene noteScene;
	[Export] HitField hitField;
	[Export] Node2D noteHolder;

	GameplaySettings _settings;

	public int scrollSpeed;

	int[] spawnPositions = new int[4];

	Note[] notePool;
	Queue<int>[] activeNotes = new Queue<int>[4];
	int pullInd = 0;
	int poolCount;

	int hitFieldCenter;
	float[] judgements = new float[4];
	string[] judgmentNames = ["Marvelous", "Perfect", "Great", "Miss"];

	public override void _Ready()
	{
		_settings = DataManager.GameplaySettings;
		scrollSpeed = _settings.scrollSpeed;

		int paddedWidth = _settings.playfieldWidth - _settings.playfieldPadding * 2;

		int spacing = paddedWidth/5;
		int pos = -_settings.playfieldWidth/2 + _settings.playfieldPadding + spacing;
		for (int i = 0; i < 4; i++) {
			activeNotes[i] = [];
			spawnPositions[i] = pos;
			pos += spacing;
		}

		poolCount = _settings.notePoolSize;
		notePool = new Note[poolCount];

		for (int i = 0; i < poolCount; i++) {
			Note note = noteScene.Instantiate<Note>();
			noteHolder.AddChild(note);
			note.Scale = Vector2.One * _settings.receptorScale;
			notePool[i] = note;
		}

		hitFieldCenter = (int)hitField.Position.Y - _settings.hitOffset;

		float scale = _settings.scrollSpeed / 1000f;

		judgements[0] = _settings.marvelousWindow * scale;
        judgements[1] = _settings.perfectWindow * scale;
        judgements[2] = _settings.greatWindow * scale;
        judgements[3] = _settings.missWindow * scale;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("spawn_note")) SpawnNote(0);

		for (int i = 0; i < 4; i++)
        {
            while (activeNotes[i].Count > 0)
            {
                int poolIndex = activeNotes[i].Peek();
                Note note = notePool[poolIndex];

                if (note.Position.Y < 350) 
                    break;

				GD.Print(judgmentNames[3]);
                activeNotes[i].Dequeue();
                note.Disable();
                poolCount++;
            }
        }
	}

	public void SpawnNote(int lane) {
		if (poolCount == 0) {
			GD.PrintErr("Note Pool Empty");
		}

		activeNotes[lane].Enqueue(pullInd);
		notePool[pullInd].Spawn(spawnPositions[lane], Position.Y);

		pullInd = (pullInd + 1) % _settings.notePoolSize;
		poolCount--;
	}

	public void DestroyNote(int lane) {
		if (activeNotes[lane].Count == 0) return;

		int poolIndex = activeNotes[lane].Peek();
		Note note = notePool[poolIndex];

		activeNotes[lane].Dequeue();
		note.Disable();
		poolCount++;
	}

	public void LaneInput(int lane) {
		if (activeNotes[lane].Count == 0) return;

		int poolIndex = activeNotes[lane].Peek();
		Note note = notePool[poolIndex];

		float dist = Math.Abs(note.Position.Y - hitFieldCenter);

		for (int i = 0; i < 4; i++) {
			if (dist < judgements[i]) {
				GD.Print(judgmentNames[i]);
				activeNotes[lane].Dequeue();
				note.Disable();
				poolCount++;
				return;
			}
		}
	}
}
