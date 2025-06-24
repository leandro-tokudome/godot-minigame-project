using System.Collections.Generic;
using Godot;

namespace MeterGame;

public partial class Player : PlayerNodeAbstract
{
	private RandomNumberGenerator Rng;

	[Export]
	private string RelativeStatue = null;
	private Sprite2D Statue;
	private PackedScene _cursorPackedScene;
	private AnimatedSprite2D _animatedSprite2d;
	private Node2D Cursor;
	private Marker2D CursorMarker2d;
	private PackedScene _salmonStripe;
	private PackedScene _orangeStripe;
	private PackedScene _redStripe;
	private PackedScene _whiteStripe;
	private PackedScene _scoreTextFloat;

	private Texture2D StatueDefault;
	private Texture2D Statue1;
	private Texture2D Statue2;
	private Texture2D Statue3;
	private Texture2D Statue4;
	private Texture2D Statue5;

	private readonly List<Node2D> StripesToBeRemoved = [];
	private int SalmonSize = 16;
	private int OrangeSize = 12;
	private int RedSize = 8;
	private int WhiteSize = 4;
	private float InitialSalmonPosition;
	private float InitialOrangePosition;
	private float InitialRedPosition;
	private float InitialWhitePosition;
	private float CursorVelocity = 54.0f;
	private float CursorVelocityAddition = 3.5f;
	private float MaxCursorVelocity = 180f;
	private bool CanClick = false;

	private Vector2 CursorInitialPosition = new(-69, -3);

	public override void _Ready()
	{
		base._Ready();

		Rng = new();
		Rng.Randomize();

		_cursorPackedScene = GD.Load<PackedScene>("scenesMain/meterGame/scenes/Cursor.tscn");
		_salmonStripe = GD.Load<PackedScene>("scenesMain/meterGame/scenes/SalmonStripe.tscn");
		_orangeStripe = GD.Load<PackedScene>("scenesMain/meterGame/scenes/OrangeStripe.tscn");
		_redStripe = GD.Load<PackedScene>("scenesMain/meterGame/scenes/RedStripe.tscn");
		_whiteStripe = GD.Load<PackedScene>("scenesMain/meterGame/scenes/WhiteStripe.tscn");
		_scoreTextFloat = GD.Load<PackedScene>("scenesGeneric/scenes/ScoreTextFloat.tscn");
		_animatedSprite2d = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		Statue = GetParent().GetNode<Sprite2D>(RelativeStatue);

		StatueDefault = GD.Load<Texture2D>("scenesMain/meterGame/sprites/misc/statue-1.png");
		Statue1 = GD.Load<Texture2D>("scenesMain/meterGame/sprites/misc/statue-2.png");
		Statue2 = GD.Load<Texture2D>("scenesMain/meterGame/sprites/misc/statue-3.png");
		Statue3 = GD.Load<Texture2D>("scenesMain/meterGame/sprites/misc/statue-4.png");
		Statue4 = GD.Load<Texture2D>("scenesMain/meterGame/sprites/misc/statue-5.png");
		Statue5 = GD.Load<Texture2D>("scenesMain/meterGame/sprites/misc/statue-6.png");

		_animatedSprite2d.Play("idle");
		_animatedSprite2d.AnimationFinished += OnAnimationFinished;

		InitializeCursor();
		GenerateNewBar();
	}

	public override void _Process(double delta)
	{
		Cursor.Position += new Vector2(CursorVelocity * (float)delta, 0);
		if (Cursor.Position.X >= 69.0f)
		{
			StripesToBeRemoved.ForEach(x => x.QueueFree());
			StripesToBeRemoved.Clear();
			GenerateNewBar();
		}
	}

	public override void Action()
	{
		if (CanClick)
		{
			_animatedSprite2d.Play("dust");

			var cursorPositionX = Cursor.Position.X;

			ScoreTextFloat scoreTextFloat = _scoreTextFloat.Instantiate<ScoreTextFloat>();
			scoreTextFloat.GlobalPosition = Cursor.Position + CursorMarker2d.Position;
			scoreTextFloat.ScaleDisplay = 0.4f;
			scoreTextFloat.FadeSpeed = 1.5f;
			scoreTextFloat.Speed = 15f;

			if (cursorPositionX >= InitialSalmonPosition && cursorPositionX < InitialOrangePosition)
			{
				Statue.Texture = Statue2;
				InGameScore += 1;
				scoreTextFloat.DisplayText = "+1";
				scoreTextFloat.LabelColor = new Color(255 / 255f, 187 / 255f, 186 / 255f);
			}
			else if (cursorPositionX >= InitialOrangePosition && cursorPositionX < InitialRedPosition)
			{
				Statue.Texture = Statue3;
				InGameScore += 2;
				scoreTextFloat.DisplayText = "+2";
				scoreTextFloat.LabelColor = new Color(244 / 255f, 90 / 255f, 0);
			}
			else if (cursorPositionX >= InitialRedPosition && cursorPositionX < InitialWhitePosition)
			{
				Statue.Texture = Statue4;
				InGameScore += 3;
				scoreTextFloat.DisplayText = "+3";
				scoreTextFloat.LabelColor = new Color(208 / 255f, 0, 0);
			}
			else if (cursorPositionX >= InitialWhitePosition && cursorPositionX < InitialWhitePosition + WhiteSize)
			{
				Statue.Texture = Statue5;
				InGameScore += 5;
				scoreTextFloat.DisplayText = "+5";
				scoreTextFloat.LabelColor = new Color(1, 1, 1);
			}
			else
			{
				Statue.Texture = Statue1;
				scoreTextFloat.DisplayText = "MISS";
				scoreTextFloat.LabelColor = new Color(161 / 255f, 161 / 255f, 161 / 255f);
			}

			AddChild(scoreTextFloat);
			UpdateScoreboardScore(InGameScore);
		}

		CanClick = false;
	}


	private void InitializeCursor()
	{
		Cursor = _cursorPackedScene.Instantiate<Node2D>();
		CursorMarker2d = Cursor.GetNode<Marker2D>("Marker2D");
		Cursor.ZIndex = 2;
		AddChild(Cursor);
	}

	private void GenerateNewBar()
	{
		Statue.Texture = StatueDefault;

		var initialBarPosition = Rng.RandfRange(
			-69.0f,
			69.0f - SalmonSize - OrangeSize - RedSize - WhiteSize
		);
		InitialSalmonPosition = initialBarPosition;
		InitialOrangePosition = InitialSalmonPosition + SalmonSize;
		InitialRedPosition = InitialOrangePosition + OrangeSize;
		InitialWhitePosition = InitialRedPosition + RedSize;

		CreateStripe(_salmonStripe, SalmonSize, InitialSalmonPosition);
		CreateStripe(_orangeStripe, OrangeSize, InitialOrangePosition);
		CreateStripe(_redStripe, RedSize, InitialRedPosition);
		CreateStripe(_whiteStripe, WhiteSize, InitialWhitePosition);

		RepositionCursor();
	}

	private void CreateStripe(PackedScene stripePrefab, int size, float initialX)
	{
		for (var index = 0; index < size; index++)
		{
			var stripe = stripePrefab.Instantiate<Node2D>();
			stripe.Position = new Vector2(initialX + index, 0);
			StripesToBeRemoved.Add(stripe);
			AddChild(stripe);
		}
	}

	private void RepositionCursor()
	{
		CanClick = true;
		if (CursorVelocity < MaxCursorVelocity)
			CursorVelocity += CursorVelocityAddition;
		Cursor.Position = CursorInitialPosition;
	}

	private void OnAnimationFinished()
		=> _animatedSprite2d.Play("idle");
}
