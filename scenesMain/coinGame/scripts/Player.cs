using Godot;

namespace CoinGame;

public partial class Player : PlayerCharacterBodyAbstract
{
	private AnimatedSprite2D _animatedSprite;
	private Timer _turnOverTimer;
	private Timer _stunCooldownTimer;

	[Export]
	private float JumpVelocity = 110f;
	[Export]
	private int Direction = 1;
	private bool IsStunned = false;

	public override void _Ready()
	{
		base._Ready();

		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite.AnimationFinished += OnAnimationFinished;
		if (Direction == -1)
			_animatedSprite.FlipH = true;

		_turnOverTimer = GetNode<Timer>("TurnOverCooldown");
		_stunCooldownTimer = GetNode<Timer>("StunCooldown");
		_stunCooldownTimer.Timeout += OnStunCooldownTimeout;

		_animatedSprite.Play("idle");
		ZIndex = 999;
	}

	public override void Action()
	{
		if (IsStunned)
			return;

		_animatedSprite.Play("action");
		VelocityYChange(-JumpVelocity);
	}

	public override void _Process(double delta)
	{
		if (IsStunned)
			return;

		if (IsOnWall() && _turnOverTimer.IsStopped())
		{
			_turnOverTimer.Start();
			Direction *= -1;
			_animatedSprite.FlipH = Direction != 1;
		}
	}

	public override void PhysicsProcessAction()
	{
		if (IsStunned)
		{
			VelocityX = 0f;
			return;
		}

		VelocityX = 50f * Direction;
	}

	public void StunPlayer()
	{
		if (IsStunned)
			return;
		IsStunned = true;
		_stunCooldownTimer.Start();
		_animatedSprite.Play("stun");
	}

	public void OnAnimationFinished()
		=> _animatedSprite.Play("idle");

	public void OnStunCooldownTimeout()
	{
		IsStunned = false;
		_animatedSprite.Play("idle");
	}
}
