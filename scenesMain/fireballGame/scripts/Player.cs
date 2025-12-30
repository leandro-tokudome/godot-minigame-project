using Godot;

namespace Fireball;

public partial class Player : PlayerCharacterBodyAbstract
{
	private Main _main;
	private AnimatedSprite2D _animatedSprite;
	private CollisionShape2D _collisionShape;
	private Timer _queueFreeTimer;

	public override void _Ready()
	{
		base._Ready();

		_main = GetParent() as Main;

		VelocityX = 37.5f;

		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite.Play("action");

		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

		_queueFreeTimer = GetNode<Timer>("QueueFreeTimer");
		_queueFreeTimer.Timeout += QueueFree;

		if (Name == "Player2" || Name == "Player4")
		{
			_animatedSprite.FlipH = _animatedSprite.FlipH == true ? false : true;
			VelocityX *= -1;
		}
	}

	public override void Action()
	{
		if (IsAlive)
		{
			_animatedSprite.FlipH = _animatedSprite.FlipH == true ? false : true;
			VelocityX *= -1;
		}
	}

	public void KillPlayer()
	{
		IsAlive = false;
		_animatedSprite.Play("death");
		_collisionShape.SetDeferred("disabled", true);
		VelocityX = 0;
		ZIndex = 998;

		InGameScore = _main.WinningPoints;
		_main.WinningPoints++;

		if (_main.WinningPoints == _main.Players)
			_main.FinishGame();

		_queueFreeTimer.Start();
	}
}
