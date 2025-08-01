using System.Collections.Generic;
using Godot;

public partial class Butterfly : Node2D
{
	private RandomNumberGenerator Rng;
	private AnimatedSprite2D _animatedSprite;
	private List<string> AnimationsList = ["black", "blue", "brown", "green", "orange", "pink", "purple", "red", "yellow"];
	public float XVelocity;
	private float YVelocity;
	private int YDirectionIndex;
	public bool HasZigZag;
	private float _time;

	public override void _Ready()
	{
		Rng = new();
		YDirectionIndex = Rng.Randf() <= 0.5 ? -1 : 1;

		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		float randomScale = Rng.RandfRange(0.5f, 0.7f);
		_animatedSprite.Scale = new Vector2(randomScale, randomScale);
		int randomAnimationIndex = Rng.RandiRange(0, 8);
		_animatedSprite.Play(AnimationsList[randomAnimationIndex]);
	}

	public override void _Process(double delta)
	{
		Position += new Vector2(XVelocity * (float)delta, 0);

		if (HasZigZag)
		{
			_time += (float)delta;
			YVelocity = Mathf.Sin(_time) * (YDirectionIndex * 10);
			Position += new Vector2(0, YVelocity * (float)delta);
		}
	}
}
