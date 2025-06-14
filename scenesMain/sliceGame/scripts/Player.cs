using Godot;

namespace SliceGame;

public partial class Player : PlayerNodeAbstract
{
	private Main _main;
	private AnimatedSprite2D _animatedSprite;
	private Marker2D _marker2d;
	private PackedScene _scoreTextFloat;

	public override void _Ready()
	{
		_main = GetParent() as Main;
		base._Ready();

		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite.Play("idle");
		_animatedSprite.AnimationFinished += OnAnimationFinished;

		_marker2d = GetNode<Marker2D>("Marker2D");

		_scoreTextFloat = GD.Load<PackedScene>("scenesGeneric/scenes/ScoreTextFloat.tscn");
	}

	public override void Action()
	{
		_animatedSprite.Play("action");

		NpcAbstract npc = _main.CurrentNpc as NpcAbstract;

		int score = 0;
		if (npc != null)
		{
			score = npc.Die();

			ScoreTextFloat scoreTextFloat = _scoreTextFloat.Instantiate<ScoreTextFloat>();
			scoreTextFloat.Position = _marker2d.Position;
			if (score < 0)
			{
				scoreTextFloat.DisplayText = "-2";
				scoreTextFloat.IsRed = true;
			}
			else
			{
				scoreTextFloat.DisplayText = "+1";
			}

			scoreTextFloat.ScaleDisplay = 0.5f;
			AddChild(scoreTextFloat);

			InGameScore += score;
			UpdateScoreboardScore(InGameScore);
		}

	}

	public void OnAnimationFinished()
	{
		if (_animatedSprite.Animation == "action")
			_animatedSprite.Play("idle");
	}
}
