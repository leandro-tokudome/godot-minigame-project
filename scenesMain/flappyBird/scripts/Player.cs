using Godot;

namespace FlappyBird;

public partial class Player : PlayerCharacterBodyAbstract
{
	private Main _main;

	[Export]
	private float JumpVelocity = 0f;

	private AnimatedSprite2D _animatedSprite;
	private CollisionShape2D _collisionShape;

	public override void _Ready()
	{
		base._Ready();

		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite.Play("idle");
		_animatedSprite.AnimationFinished += OnAnimationFinished;

		_main = GetParent() as Main;

		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
	}

	public override void Action()
	{
		_animatedSprite.Play("action");

		VelocityYChange(-JumpVelocity);
	}

	public override void PhysicsProcessAction()
	{
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);
			Node collider = (Node)collision.GetCollider();

			if (collider.Name != "Ceiling")
			{
				IsAlive = false;
				ZIndex = 999;
				GravityVelocity = 0f;
				VelocityX = -50f;
				VelocityYChange(0f);
				_collisionShape.Disabled = true;

				_animatedSprite.Play("player_death");

				_main.RemovePlayer(Name);
			}
		}
	}

	public void OnAnimationFinished()
	{
		if (_animatedSprite.Animation == "action")
			_animatedSprite.Play("idle");
		if (_animatedSprite.Animation == "player_death")
			QueueFree();
	}
}
