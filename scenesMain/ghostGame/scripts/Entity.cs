using Godot;

namespace GhostGame;

public partial class Entity : Node2D
{
	public Area2D _area2D;
	public Sprite2D _sprite;

	public float EntityVelocity = 100f;
	public int OwnerPlayerIndex { get; set; }
	public bool IsAngel { get; set; }
	public bool CanBePointed { get; set; } = true;
	public bool IsFading { get; set; } = false;
	public float FadeHeight = 12.5f;
	private float _alpha = 1.0f;

	public override void _Ready()
	{
		_area2D = GetNode<Area2D>("Area2D");
		_sprite = GetNode<Sprite2D>("Sprite2D");
	}

	public override void _Process(double delta)
	{
		Position += new Vector2(0, EntityVelocity) * (float)delta;

		if (Position.Y > FadeHeight && !IsFading)
		{
			IsFading = true;
		}

		if (IsFading && _alpha > 0)
		{
			_alpha -= 0.0005f * EntityVelocity;
			_sprite.Modulate = new Color(_sprite.Modulate.R, _sprite.Modulate.G, _sprite.Modulate.B, _alpha);
		}

		if (IsFading && _alpha == 0)
			IsFading = false;
	}
}
