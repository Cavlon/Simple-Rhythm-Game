using Godot;
using System;

public partial class Receptor : Node2D
{
	Sprite2D sprite;

    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("Sprite2D");
    }

	public void SetTexture(Texture2D texture) {
		sprite.Texture = texture;
	}
}
