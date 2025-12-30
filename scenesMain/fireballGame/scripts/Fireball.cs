using Godot;

namespace Fireball;

public partial class Fireball : Node2D
{
	private AnimatedSprite2D _animatedSprite;
	private Area2D _area2d;
	private RandomNumberGenerator Rng;

	private float Gravity;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite.Play("action");

		_area2d = GetNode<Area2D>("Area2D");
		_area2d.BodyEntered += OnBodyEntered;

		Rng = new();

		Gravity = Rng.RandfRange(20, 40);
		ZIndex = 2;
	}

	public override void _Process(double delta)
	{
		Position += new Vector2(0, Gravity * (float)delta);
	}

	public void OnBodyEntered(Node body)
	{
		if (body is Player player)
		{
			player.KillPlayer();
		}
	}
}
