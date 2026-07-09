using System.Collections.Generic;
using Godot;

namespace Boxing;

public partial class Player : PlayerNodeAbstract
{
	private Main _main;
	private PackedScene PlayerArrowPackedScene;
	private Node2D _arrow;
	private PackedScene BlockPackedScene;
	private AnimatedSprite2D _animatedSprite2d;
	private Marker2D _blockOneMarker2d;
	private Marker2D _firstQueue;
	private Marker2D _lastQueue;
	private Sprite2D _firstEnergyBar;
	private Timer _timerEnergyUp;
	private Block _firstBlock;
	private Block _secondBlock;
	private Block _thirdBlock;

	private Vector2 _arrowBlockOnePosition;
	public List<int> BlockSequence;
	public int BlockSequenceIndex = 0;
	private Vector2 _firstQueuePosition;
	private Vector2 _lastQueuPosition;
	private int PlayerChoice = 1;
	private int Energy = 10;
	private bool IsLeftHand = true;
	private bool IsPunching = false;
	private bool IsBlockChanging = false;

	public override void _Ready()
	{
		base._Ready();

		_main = GetParent() as Main;
		BlockSequence = _main.BlockSequence;

		_animatedSprite2d = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_timerEnergyUp = GetNode<Timer>("TimerEnergyUp");
		_blockOneMarker2d = GetNode<Marker2D>("BlockOneMarker");
		_firstQueue = GetNode<Marker2D>("FirstQueue");
		_lastQueue = GetNode<Marker2D>("LastQueue");

		_animatedSprite2d.AnimationFinished += OnAnimationFinished;
		_timerEnergyUp.Timeout += OnTimerEnergyUpTimeout;
		_firstQueuePosition = _firstQueue.Position;
		_lastQueuPosition = _lastQueue.Position;
		_arrowBlockOnePosition = _blockOneMarker2d.Position;

		PlayerArrowPackedScene = GD.Load<PackedScene>("scenesMain/boxingGame/scenes/Arrow.tscn");
		_arrow = PlayerArrowPackedScene.Instantiate<Node2D>();
		var nameToLower = Name.ToString().ToLower();
		_arrow.GetNode<Sprite2D>("Sprite2D").Texture = GD.Load<Texture2D>($"scenesMain/boxingGame/sprites/{nameToLower}/{nameToLower}-arrow.png");
		_arrow.Position = _arrowBlockOnePosition;
		AddChild(_arrow);

		BlockPackedScene = GD.Load<PackedScene>("scenesMain/boxingGame/scenes/Block.tscn");

		CallDeferred(nameof(GenerateFirstBlocks));
		CallDeferred(nameof(GenerateFirstEnergyBar));
	}

	public override void _Process(double delta)
	{
		if (_firstBlock.BlockNumber == PlayerChoice && !IsPunching && Energy > 0)
		{
			IsPunching = true;

			if (IsLeftHand)
				_animatedSprite2d.Play("action-left-hand");
			else
				_animatedSprite2d.Play("action-right-hand");

			IsLeftHand = !IsLeftHand;
		}

		if (_animatedSprite2d.Frame == 10 && !IsBlockChanging)
		{
			IsBlockChanging = true;
			InGameScore++;
			UpdateScoreboardScore(InGameScore);
			_firstBlock.DestroyBlock();
			GenerateNextBlock();
		}
	}

	public override void Action()
	{
		if (Energy > 0)
		{
			Energy--;
			_firstEnergyBar.QueueFree();
			_firstEnergyBar = GetNodeOrNull<Sprite2D>($"EnergyBar{Energy - 1}");
		}

		PlayerChoice++;

		if (PlayerChoice > 3)
		{
			PlayerChoice = 1;
			_arrow.Position = _arrowBlockOnePosition;
		}
		else
			_arrow.Position += new Vector2(0, 16f);
	}

	private void GenerateFirstBlocks()
	{
		if (BlockSequence is null || BlockSequence.Count < 3)
		{
			CallDeferred(nameof(GenerateFirstBlocks));
			return;
		}

		for (var index = 0; index < 3; index++)
		{
			var block = GenerateBlockByNumber(BlockSequence[index]);
			block.Position = new Vector2(_firstQueuePosition.X + (index * 18f), _firstQueuePosition.Y);
			AddChild(block);

			if (index == 0)
				_firstBlock = block;
			else if (index == 1)
				_secondBlock = block;
			else if (index == 2)
				_thirdBlock = block;
		}

		BlockSequenceIndex = 3;
	}

	private void GenerateFirstEnergyBar()
	{
		for (var index = 0; index < Energy; index++)
		{
			GenerateEnergyBar(index);
		}
	}

	private static Block GenerateBlockByNumber(int blockNumber)
	{
		var blockScene = GD.Load<PackedScene>("scenesMain/boxingGame/scenes/Block.tscn");
		var block = blockScene.Instantiate<Block>();
		block.BlockNumber = blockNumber;
		return block;
	}

	private void GenerateNextBlock()
	{
		var block = GenerateBlockByNumber(BlockSequence[BlockSequenceIndex]);
		block.Position = new Vector2(_lastQueuPosition.X + 18f, _lastQueuPosition.Y);
		AddChild(block);

		var tween = CreateTween();
		tween.SetParallel();
		tween.SetTrans(Tween.TransitionType.Sine);
		tween.SetEase(Tween.EaseType.Out);

		tween.TweenProperty(_secondBlock, "position", new Vector2(_firstQueuePosition.X, _secondBlock.Position.Y), 0.20f);
		tween.TweenProperty(_thirdBlock, "position", new Vector2(_firstQueuePosition.X + 18f, _thirdBlock.Position.Y), 0.20f);
		tween.TweenProperty(block, "position", _lastQueuPosition, 0.20f);

		_firstBlock = _secondBlock;
		_secondBlock = _thirdBlock;
		_thirdBlock = block;

		IncreaseBlockSequenceIndex();
	}

	private void GenerateEnergyBar(int energyIndex = 0)
	{
		var energyBar = new Sprite2D
		{
			Texture = GD.Load<Texture2D>("scenesMain/boxingGame/sprites/misc/energy-bar.png"),
			Position = new Vector2(-15f + (energyIndex * 3f), -15f),
			Name = $"EnergyBar{energyIndex}"
		};

		AddChild(energyBar);
		_firstEnergyBar = energyBar;
	}

	private void IncreaseBlockSequenceIndex()
		=> BlockSequenceIndex++;

	private void OnAnimationFinished()
	{
		IsPunching = false;
		IsBlockChanging = false;
		_animatedSprite2d.Play("idle");
	}

	private void OnTimerEnergyUpTimeout()
	{
		if (Energy < 10)
		{
			GenerateEnergyBar(Energy);
			Energy++;
		}
	}
}
