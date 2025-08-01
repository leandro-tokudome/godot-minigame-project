using Godot;

namespace CountGame;

public partial class Player : PlayerNodeAbstract
{
	private AnimatedSprite2D _animatedSprite;
	public int TimesClicked = 0;
	public int VariationCount = 0;
	public bool CanClick = true;

	public override void _Ready()
	{
		base._Ready();

		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite.Play("idle");
		_animatedSprite.AnimationFinished += OnAnimationFinished;
	}

	public override void Action()
	{
		if (CanClick)
		{
			TimesClicked++;
			_animatedSprite.Play("action");
		}
	}

	public void OnAnimationFinished()
		=> _animatedSprite.Play("idle");
}
