using System;
using System.Collections.Generic;
using Godot;

namespace CupShuffle;

public partial class Main : MainAbstract
{
	private List<Player> PlayersList = [];
	private float FramesVelocity = 1.0f;
	public int Shuffles = 0;
	private int Round = 1;
	public bool IsShuffling = false;
	public bool IsPlayerChoice = false;
	private int ShufflesRound1 = 20;
	private int ShufflesRound2 = 45;
	private int ShufflesRound3 = 80;
	private int ShuuflesRound4 = 100;

	private Node2D _cups;
	public AnimatedSprite2D CupsAnimatedSprite;
	private PackedScene PlayerBall;
	private PackedScene ScoreTextFloat;
	private List<string> BallsPosition = [];
	private RandomNumberGenerator Rng;

	private Vector2 PositionA;
	private Vector2 PositionB;
	private Vector2 PositionC;
	private Vector2 PositionD;
	private Vector2 ArrowPositionPlayer1;
	private Vector2 ArrowPositionPlayer2;
	private Vector2 ArrowPositionPlayer3;
	private Vector2 ArrowPositionPlayer4;

	private Dictionary<int, Action> ShuffleMethods;

	private Timer ShowdownBallsTimer;
	private Timer LowerCupsTimer;
	[Export]
	private int PlayerChoiceTimer = 5;

	public override void _Ready()
	{
		base._Ready();

		ShuffleMethods = new Dictionary<int, Action>
		{
			{ 1, AtoB },
			{ 2, AtoC },
			{ 3, AtoD },
			{ 4, BtoC },
			{ 5, BtoD },
			{ 6, CtoD }
		};

		_cups = GetNode<Node2D>("Cups");

		ShowdownBallsTimer = new Timer { WaitTime = 2.0, OneShot = true };
		LowerCupsTimer = new Timer { WaitTime = 1.5, OneShot = true };

		ShowdownBallsTimer.Timeout += HideBalls;
		LowerCupsTimer.Timeout += StartShuffle;

		AddChild(ShowdownBallsTimer);
		AddChild(LowerCupsTimer);

		PlayerBall = GD.Load<PackedScene>("scenesMain/cupShuffle/scenes/PlayerBall.tscn");
		ScoreTextFloat = GD.Load<PackedScene>("scenesGeneric/scenes/ScoreTextFloat.tscn");

		CupsAnimatedSprite = _cups.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		CupsAnimatedSprite.Play("idle-lift");

		Rng = new();
		Rng.Randomize();

		for (var index = 1; index <= Players; index++)
			PlayersList.Add(GetNode<Player>($"Player{index}"));

		InitializePlayersBalls();
		SetPlayersBall();

		StartGame();
	}

	public void StartGame()
		=> ShowdownBallsTimer.Start();

	private void InitializePlayersBalls()
	{
		List<Marker2D> playerBallsMarker = [];
		List<Marker2D> playerArrowMarker = [];

		for (var index = 1; index <= 4; index++)
		{
			playerBallsMarker.Add(GetNode<Marker2D>($"Player{index}BallMarker"));
			playerArrowMarker.Add(GetNode<Marker2D>($"Player{index}ArrowMarker"));

			if (index > Players)
			{
				BallsPosition.Add(null);
				continue;
			}

			BallsPosition.Add($"Player{index}");
		}

		PositionA = playerBallsMarker[0].GlobalPosition;
		PositionB = playerBallsMarker[1].GlobalPosition;
		PositionC = playerBallsMarker[2].GlobalPosition;
		PositionD = playerBallsMarker[3].GlobalPosition;

		ArrowPositionPlayer1 = playerArrowMarker[0].GlobalPosition;
		ArrowPositionPlayer2 = playerArrowMarker[1].GlobalPosition;
		ArrowPositionPlayer3 = playerArrowMarker[2].GlobalPosition;
		ArrowPositionPlayer4 = playerArrowMarker[3].GlobalPosition;
	}

	private void SetPlayersBall()
	{
		for (var index = 1; index <= 4; index++)
		{
			if (BallsPosition[index - 1] is null)
				continue;

			var playerBall = PlayerBall.Instantiate<Node2D>();
			playerBall.Name = $"{BallsPosition[index - 1]}Ball";
			playerBall.ZIndex = -1;

			var nameToLower = BallsPosition[index - 1].ToString().ToLower();
			Sprite2D playerBallSprite = playerBall.GetNode<Sprite2D>("Sprite2D");
			playerBallSprite.Texture = GD.Load<Texture2D>($"scenesMain/cupShuffle/sprites/{nameToLower}/{nameToLower}-ball.png");

			playerBall.Position =
				index == 1 ? PositionA :
				index == 2 ? PositionB :
				index == 3 ? PositionC :
				index == 4 ? PositionD :
				throw new Exception("Index position not in context");

			AddChild(playerBall);
		}
	}

	private void RemovePlayersBall()
	{
		for (var index = 1; index <= Players; index++)
		{
			var playerBall = GetNode<Node2D>($"Player{index}Ball");
			playerBall.QueueFree();
		}
	}

	private void StartShuffle()
	{
		RemovePlayersBall();
		IsShuffling = true;

		FramesVelocity =
			Round == 1 ? 1.0f :
			Round == 2 ? 1.7f :
			Round == 3 ? 2.4f :
			Round == 4 ? 2.95f :
			1.0f;
		ChangeAnimationVelocity(FramesVelocity);

		RandomShuffle();
	}

	public void RandomShuffle()
	{
		if (Round == 1 && Shuffles == ShufflesRound1)
			FinishShuffling();
		else if (Round == 2 && Shuffles == ShufflesRound2)
			FinishShuffling();
		else if (Round == 3 && Shuffles == ShufflesRound3)
			FinishShuffling();
		else if (Round == 4 && Shuffles == ShuuflesRound4)
			FinishShuffling();

		if (IsShuffling)
		{
			var randomNumber = Rng.RandiRange(1, 6);
			ShuffleMethods[randomNumber]();
		}
	}

	private void AtoB()
	{
		SwapPositions(BallsPosition, 0, 1);
		PlayCupAnimation("a-to-b", false);
	}

	private void AtoC()
	{
		SwapPositions(BallsPosition, 0, 2);
		PlayCupAnimation("a-to-c", false);
	}

	private void AtoD()
	{
		SwapPositions(BallsPosition, 0, 3);
		PlayCupAnimation("a-to-d", false);
	}

	private void BtoC()
	{
		SwapPositions(BallsPosition, 1, 2);
		PlayCupAnimation("b-to-c", false);
	}

	private void BtoD()
	{
		SwapPositions(BallsPosition, 1, 3);
		PlayCupAnimation("a-to-c", true);
	}

	private void CtoD()
	{
		SwapPositions(BallsPosition, 2, 3);
		PlayCupAnimation("a-to-b", true);
	}

	private void SwapPositions(List<string> list, int firstIndex, int secondIndex)
		=> (list[secondIndex], list[firstIndex]) = (list[firstIndex], list[secondIndex]);

	private void FinishShuffling()
	{
		IsShuffling = false;
		ShowGameTimer(PlayerChoiceTimer);

		for (var index = 1; index <= Players; index++)
			GetNode<Player>($"Player{index}").SetPlayerArrow();

		IsPlayerChoice = true;
	}

	public override void OnGameTimerFinish()
	{
		IsPlayerChoice = false;

		GameTimer timer = GetNode<GameTimer>("GameTimer");
		timer.QueueFree();

		CountPoints();
		ShowdownBalls();
		FinishRound();

		ShowdownBallsTimer.Start();
	}

	private void CountPoints()
	{
		foreach (var player in PlayersList)
		{
			if (player.Name == BallsPosition[player.PlayerChoice - 1])
			{
				var pointGained =
					Round == 1 ? 1 :
					Round == 2 ? 2 :
					Round == 3 ? 3 :
					Round == 4 ? 4 :
					0;

				player.InGameScore += pointGained;

				var floatScore = GetNode<Marker2D>($"{player.Name}FloatScore");
				var scoreTextFloat = ScoreTextFloat.Instantiate<ScoreTextFloat>();
				scoreTextFloat.DisplayText = $"+{pointGained}";
				scoreTextFloat.Position = floatScore.GlobalPosition;

				scoreTextFloat.ScaleDisplay = 0.75f;
				AddChild(scoreTextFloat);

				player.UpdateScoreboardScore(player.InGameScore);
			}
		}
	}

	private void FinishRound()
	{
		Shuffles = 0;
		Round++;
	}

	private void ShowdownBalls()
	{
		for (var index = 1; index <= Players; index++)
			GetNode<Node2D>($"Player{index}Arrow").QueueFree();

		ChangeAnimationVelocity(1.0f);
		SetPlayersBall();
		LiftCups();
	}

	private void HideBalls()
	{
		if (Round == 5)
			FinishGame();

		LowerCups();
		LowerCupsTimer.Start();
	}

	private void ChangeAnimationVelocity(float animationVelocity)
		=> CupsAnimatedSprite.SpeedScale = animationVelocity;

	private void PlayCupAnimation(string animationName, bool isMirrored)
	{
		var randomNumber = Rng.RandiRange(1, 2);

		if (!isMirrored)
		{
			if (randomNumber == 1)
				CupsAnimatedSprite.Play(animationName);
			else
				CupsAnimatedSprite.PlayBackwards(animationName);
		}
		else
		{
			CupsAnimatedSprite.FlipH = true;
			if (randomNumber == 1)
				CupsAnimatedSprite.Play(animationName);
			else
				CupsAnimatedSprite.PlayBackwards(animationName);
		}
	}

	private void LowerCups()
		=> CupsAnimatedSprite.PlayBackwards("lift");

	private void LiftCups()
		=> CupsAnimatedSprite.Play("lift");
}
