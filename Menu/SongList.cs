using Godot;
using System;

public partial class SongList : Control
{
	[Export] float spacing = 0f;
	[Export] int entryCount = 1;
	[Export] float scrollTickAmount = 10;
	[Export] float maxScrollSpeed = 100f;

	SongEntry songEntryTemplate;

	float entryHeight;
	float entrySpace;
	SongEntry[] entryPool;

	int startInd = 0;
	float bottomPos;
	float scroll = 0;
	float targetScroll = 0;
	int selectedInd = 0;

	float holdTimer = 0.4f;
	bool holdDown = false;
	bool holdUp = false;
	bool dragging = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		songEntryTemplate = GetNode<SongEntry>("SongEntry");
		songEntryTemplate.AnchorRight = 1f;
		songEntryTemplate.Position = new Vector2(30f, 0f);

		entryHeight = songEntryTemplate.Size.Y;
		entrySpace = entryHeight + spacing;

		entryPool = new SongEntry[(int)(Size.Y / entrySpace) + 2];
		entryPool[0] = songEntryTemplate;
		UpdateEntry(0);
		songEntryTemplate.Clicked += UpdateSelection;

		if (entryCount == 0) {
			songEntryTemplate.Visible = false;
		} else {
			songEntryTemplate.Selected();
		}

		for (int i = 1; i < entryPool.Length; i++) {
			SongEntry entry = (SongEntry)songEntryTemplate.Duplicate();
			entry.AnchorRight = 1f;
			entry.Position = new Vector2(entry.Position.X, i * entrySpace);
			
			if (i >= entryCount) {
				entry.Visible = false;
			}

			entryPool[i] = entry;
			
			AddChild(entry);

			UpdateEntry(i);
			entry.Clicked += UpdateSelection;
		}

		UpdateBottom();

		UpdateList();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (holdDown) {
			if (holdTimer > 0) {
				holdTimer -= (float)delta;
			} else {
				if (selectedInd < entryCount-1) {
					UpdateSelection(selectedInd+1);
				}
				holdTimer = 0.06f;
			}
		} else if (holdUp) {
			if (holdTimer > 0) {
				holdTimer -= (float)delta;
			} else {
				if (selectedInd > 0) {
					UpdateSelection(selectedInd-1);
				}
				holdTimer = 0.06f;
			}
		}

		// If you try scrolling above the first element
		if (targetScroll < 0)
		{
			// Lerp back down so the first element is at the top
			targetScroll = (float)Mathf.Lerp(targetScroll, 0, delta * 10);
		}
		else
		{
			// If the bottom of the final element is below the bottom bound of the scroll box
			if (bottomPos > Size.Y)
			{
				// If you try scrolling below the bottom element
				if (targetScroll + Size.Y > bottomPos)
				{
					// Lerp back up so the bottom element is at the bottom
					targetScroll = (float)Mathf.Lerp(targetScroll, bottomPos - Size.Y, delta * 10);
				}
			}
			else // If all the elements fit within the scroll box
			{
				if (targetScroll > 0) // If you try scrolling down
				{
					// Lerp back up so the first element is at the top
					targetScroll = (float)Mathf.Lerp(targetScroll, 0, delta * 10);
				}
			}
		}

		if (Math.Abs(targetScroll-scroll) > 1f) {
			InterpolateScroll(delta);
			UpdateList();
		}
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mb)
		{
			if (mb.ButtonIndex == MouseButton.WheelDown)
			{
				targetScroll += scrollTickAmount;
				UpdateList();
				AcceptEvent();
			}
			else if (mb.ButtonIndex == MouseButton.WheelUp)
			{
				targetScroll -= scrollTickAmount;
				UpdateList();
				AcceptEvent();
			} else if (mb.ButtonIndex == MouseButton.Left) {
				if (mb.Pressed) {
					dragging = true;
					AcceptEvent();
				} else {
					dragging = false;
				}
			}
		} else if (@event is InputEventMouseMotion mouseMotion && dragging) {
			targetScroll -= mouseMotion.Relative.Y * 2f;
			AcceptEvent();
		}
		
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_down")) {
			if (selectedInd < entryCount-1) {
				UpdateSelection(selectedInd+1);
			}
			holdDown = true;
			GetViewport().SetInputAsHandled();
		} else if (@event.IsActionReleased("ui_down")) {
			holdDown = false;
			holdTimer = 0.4f;
		}
		
		if (@event.IsActionPressed("ui_up")) {
			if (selectedInd > 0) {
				UpdateSelection(selectedInd-1);
			}
			holdUp = true;
			GetViewport().SetInputAsHandled();
		} else if (@event.IsActionReleased("ui_up")) {
			holdUp = false;
			holdTimer = 0.4f;
		}
    }

	void UpdateBottom() {
		bottomPos = entryCount * entrySpace - spacing;
	}

	void UpdatePositions() {
		for (int i = startInd; i < startInd + entryPool.Length; i++) {
			SongEntry entry = entryPool[i % entryPool.Length];
			entry.Position = new Vector2(entry.Position.X, (i * entrySpace) - scroll);
			if (i >= entryCount) entry.Visible = false;
		}
	}

	void UpdateList() {

		UpdatePositions();

		SongEntry firstEntry = entryPool[startInd % entryPool.Length];
		SongEntry lastEntry = entryPool[(startInd + entryPool.Length-1) % entryPool.Length];
		if (firstEntry.Position.Y > 0 && scroll >= 0) {
			do {
				lastEntry.Position = new Vector2(lastEntry.Position.X, ((startInd - 1) * entrySpace) - scroll);
				UpdateEntry(startInd-1);
				startInd--;
				firstEntry = lastEntry;
				lastEntry = entryPool[(startInd + entryPool.Length-1) % entryPool.Length];

			} while (firstEntry.Position.Y > 0);

		} else if (lastEntry.Position.Y < Size.Y - spacing - entryHeight && scroll + Size.Y <= bottomPos) {
			do {
				firstEntry.Position = new Vector2(firstEntry.Position.X, ((startInd + entryPool.Length) * entrySpace) - scroll);
				UpdateEntry(startInd + entryPool.Length);
				startInd++;
				lastEntry = firstEntry;
				firstEntry = entryPool[startInd % entryPool.Length];

			} while (lastEntry.Position.Y < Size.Y - spacing - entryHeight);
		}
	}

	void UpdateEntry(int listIndex) {
		SongEntry entry = entryPool[listIndex % entryPool.Length];
		entry.SetId(listIndex);
		if (listIndex == selectedInd) {
			entry.Selected(false);
		} else {
			entry.UnSelected(false);
		}
	}

	void UpdateSelection(int newSelectInd) {
		if (newSelectInd < 0 || newSelectInd >= entryCount) return;

		entryPool[selectedInd % entryPool.Length].UnSelected();
		selectedInd = newSelectInd;

		float selectedPos = selectedInd * entrySpace + entryHeight / 2f;
		targetScroll = selectedPos - Size.Y/2f;

		if (targetScroll < 0) targetScroll = 0;
		else if (targetScroll + Size.Y > bottomPos) targetScroll = bottomPos - Size.Y;

		UpdateList();

		if (selectedInd >= startInd && selectedInd < startInd + entryPool.Length) entryPool[selectedInd % entryPool.Length].Selected();
	}

	void InterpolateScroll(double delta) {
		scroll += Mathf.Clamp((targetScroll - scroll) * (float)delta * 10f, -maxScrollSpeed, maxScrollSpeed);
	}
}
