using Godot;
using System;

namespace CoinGame;

public partial class Point : Node2D
{
	public AnimatedSprite2D _animatedSprite;
	public Area2D _area2D;

	public int PointValue;
	private bool _signalsConnected;
	private bool _initialized;

	public override void _Ready()
	{
		_animatedSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		_area2D = GetNodeOrNull<Area2D>("Area2D");

		if (_animatedSprite is null || _area2D is null)
			return;

		if (!_signalsConnected)
		{
			_animatedSprite.AnimationFinished += OnAnimationFinished;
			_area2D.BodyEntered += OnBodyEntered;
			_signalsConnected = true;
		}

		_initialized = true;
	}

	public void DefineValue(bool IsCoin)
	{
		if (_animatedSprite is null || _area2D is null)
		{
			CallDeferred(nameof(DefineValue), IsCoin);
			return;
		}

		if (IsCoin)
		{
			PointValue = 1;
			_animatedSprite.Play("coin");
		}
		else
		{
			PointValue = 3;
			_animatedSprite.Play("diamond");
		}
	}

	public void OnAnimationFinished()
		=> QueueFree();

	public void OnBodyEntered(Node body)
	{
		if (body is Player player)
		{
			player.InGameScore += PointValue;
			player.UpdateScoreboardScore(player.InGameScore);
			_animatedSprite.Play("fading");
			_area2D.SetDeferred("monitoring", false);
		}
	}
}
