using CupShuffle;
using Godot;

public partial class Cups : Node2D
{
	private Main _main;
	private AnimatedSprite2D _animatedSprite;

	public override void _Ready()
	{
		_main = GetParent() as Main;
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite.AnimationFinished += OnAnimationFinished;
	}

	private void OnAnimationFinished()
	{
		if (_main.IsShuffling)
		{
			_main.Shuffles++;
			_main.CupsAnimatedSprite.FlipH = false;
			_main.RandomShuffle();
		}
	}
}
