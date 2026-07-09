using Godot;

namespace Boxing;

public partial class Block : Node2D
{
	private AnimatedSprite2D _animatedSprite2d;
	private int _blockNumber;

	public int BlockNumber
	{
		get => _blockNumber;
		set
		{
			_blockNumber = value;
			_animatedSprite2d?.Play($"idle-block{_blockNumber}");
		}
	}

	public override void _Ready()
	{
		_animatedSprite2d = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite2d.Play($"idle-block{_blockNumber}");
		_animatedSprite2d.AnimationFinished += OnAnimationFinished;
	}

	public void DestroyBlock()
		=> _animatedSprite2d.Play($"destroy-block{BlockNumber}");

	private void OnAnimationFinished()
	{
		if (_animatedSprite2d.Animation.ToString().StartsWith("destroy-block"))
			QueueFree();
	}
}
