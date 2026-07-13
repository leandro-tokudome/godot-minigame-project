using Godot;
using System;

namespace CoinGame;

public partial class Missile : Node2D
{
	public AnimatedSprite2D _animatedSprite;
	public Area2D _area2D;

	public RandomNumberGenerator Rng = new();
	public float MissileVelocity = 25f;
	public int Direction = 1;
	public bool IsActive = true;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite.Play("missile");
		_area2D = GetNode<Area2D>("Area2D");

		_animatedSprite.AnimationFinished += OnAnimationFinished;
		_area2D.BodyEntered += OnBodyEntered;
	}

	public override void _Process(double delta)
	{
		if (!IsActive)
			return;

		Position += new Vector2(MissileVelocity * Direction * (float)delta, 0);
	}

	public void Prepare()
	{
		if (Direction == 1)
			_animatedSprite.FlipH = true;

		MissileVelocity = Rng.RandfRange(25f, 50f);
	}

	public void OnAnimationFinished()
	{
		if (_animatedSprite.Animation == "explosion")
			QueueFree();
	}

	public void OnBodyEntered(Node body)
	{
		if (body is Player player)
		{
			IsActive = false;
			_animatedSprite.Play("explosion");
			player.StunPlayer();
			_area2D.SetDeferred("monitoring", false);
		}
	}
}
