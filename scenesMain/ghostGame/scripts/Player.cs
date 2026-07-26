using Godot;

namespace GhostGame;

public partial class Player : PlayerNodeAbstract
{
	public AnimatedSprite2D _animatedSprite;
	public Area2D _area2D;
	public Marker2D _marker2D;
	private PackedScene _scoreTextFloat;

	public RandomNumberGenerator Rng;

	private bool _isCheckingHit;

	[Export]
	public int PlayerIndex;

	public override void _Ready()
	{
		base._Ready();

		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite.Play("idle");
		_animatedSprite.AnimationFinished += OnAnimationFinished;
		_area2D = GetNode<Area2D>("Area2D");
		_marker2D = GetNode<Marker2D>("Marker2D");
		_scoreTextFloat = GD.Load<PackedScene>("scenesGeneric/scenes/ScoreTextFloat.tscn");

		Rng = new RandomNumberGenerator();
	}

	public override void _Process(double delta)
	{
		if (!_isCheckingHit)
			return;

		var parent = GetParent();
		if (parent is null)
			return;

		var playerIndex = PlayerIndex;
		var overlappingAreas = _area2D.GetOverlappingAreas();
		foreach (var area in overlappingAreas)
		{
			if (area.Owner is not Entity entity)
				continue;

			if (entity.OwnerPlayerIndex != playerIndex)
				continue;

			if (!entity.CanBePointed)
				continue;

			ScoreTextFloat scoreTextFloat = _scoreTextFloat.Instantiate<ScoreTextFloat>();
			scoreTextFloat.Position = _marker2D.Position;
			scoreTextFloat.ScaleDisplay = 0.4f;
			scoreTextFloat.FadeSpeed = 1.5f;
			scoreTextFloat.Speed = 15f;

			entity.CanBePointed = false;

			if (entity.IsAngel)
			{
				scoreTextFloat.IsRed = true;
				scoreTextFloat.DisplayText = "-2";
				AddChild(scoreTextFloat);
				InGameScore -= 2;
			}
			else
			{
				scoreTextFloat.DisplayText = "+1";
				AddChild(scoreTextFloat);
				InGameScore++;
			}

			UpdateScoreboardScore(InGameScore);
			entity.QueueFree();
		}
	}

	public override void Action()
	{
		_animatedSprite.Play("action");
		_isCheckingHit = true;
	}

	public void OnAnimationFinished()
	{
		_animatedSprite.Play("idle");
		_isCheckingHit = false;
	}
}
