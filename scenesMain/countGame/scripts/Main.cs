using System.Collections.Generic;
using Godot;

namespace CountGame;

public partial class Main : MainAbstract
{
	private Area2D _area2dRight;
	private Timer _timerStartGame;
	private Timer _timerButterflySpawn;
	private Timer _timerFinishGame;
	private PackedScene Butterfly;
	private List<Player> PlayersList = [];
	private RandomNumberGenerator Rng;
	private int ButterfliesNumber;
	private int SpawnedButterflies = 0;
	private int QueuedFreeButterflies = 0;

	public override void _Ready()
	{
		base._Ready();

		_area2dRight = GetNode<Area2D>("Area2DRight");
		_area2dRight.AreaEntered += OnAreaEntered;

		_timerStartGame = GetNode<Timer>("TimerStartGame");
		_timerStartGame.Timeout += OnTimeoutStartGame;
		_timerButterflySpawn = GetNode<Timer>("TimerButterflySpawn");
		_timerButterflySpawn.Timeout += OnTimeoutButterflySpawn;
		_timerFinishGame = GetNode<Timer>("TimerFinishGame");
		_timerFinishGame.Timeout += FinishGame;

		Butterfly = GD.Load<PackedScene>("scenesMain/countGame/scenes/Butterfly.tscn");
		Rng = new();

		for (var index = 1; index <= Players; index++)
		{
			Player player = GetNode<Player>($"Player{index}");
			PlayersList.Add(player);
		}
	}

	private void SpawnButterfly()
	{
		Vector2 butterflyInitialPosition = new(-10, Rng.RandfRange(20, 75));

		Butterfly butterfly = Butterfly.Instantiate<Butterfly>();
		float randomXVelocity = Rng.RandfRange(15, 35);
		butterfly.XVelocity = randomXVelocity;
		bool hasZigZag = Rng.Randf() <= 0.45;
		butterfly.HasZigZag = hasZigZag;
		butterfly.Position = butterflyInitialPosition;

		AddChild(butterfly);
	}

	public void OnAreaEntered(Node2D body)
	{
		if (body is not null)
		{
			body.GetParent().QueueFree();
			QueuedFreeButterflies++;
		}

		if (QueuedFreeButterflies == ButterfliesNumber)
			ShowWinner();
	}

	private void SortButterlyNumber()
		=> ButterfliesNumber = Rng.RandiRange(50, 90);

	private void OnTimeoutStartGame()
	{
		SortButterlyNumber();
		OnTimeoutButterflySpawn();
	}

	private void OnTimeoutButterflySpawn()
	{
		if (SpawnedButterflies != ButterfliesNumber)
		{
			SpawnButterfly();
			SpawnedButterflies++;
			_timerButterflySpawn.Start();
		}
	}

	public int GetShorterCount()
	{
		int shorterCount = 99999;

		foreach (Player player in PlayersList)
		{
			int variationCount = ButterfliesNumber - player.TimesClicked;
			int treatedVariationCount = variationCount < 0 ? variationCount * -1 : variationCount;

			player.VariationCount = treatedVariationCount;

			if (treatedVariationCount < shorterCount)
				shorterCount = treatedVariationCount;
		}

		return shorterCount;
	}

	public void ShowWinner()
	{
		int shorterCount = GetShorterCount();

		var countDisplay = GetNode<Sprite2D>("CountDisplay");
		countDisplay.Visible = true;
		var label = GetNode<Label>("Label");
		label.Text = ButterfliesNumber.ToString();

		for (var i = 1; i <= Players; i++)
		{
			var scoreboard = GetNode<Scoreboard>($"Scoreboard{i}");
			var player = GetNode<Player>($"Player{i}");
			player.CanClick = false;
			scoreboard.UpdateScore(player.TimesClicked);

			if (player.VariationCount == shorterCount)
				scoreboard._label.LabelSettings.FontColor = new Color(1, 0, 0);

			scoreboard.Visible = true;

		}

		_timerFinishGame.Start();
	}

	public override void CountWin()
	{
		int shorterCount = GetShorterCount();

		foreach (Player player in PlayersList)
		{
			if (player.VariationCount == shorterCount)
				WinnerPlayers.Add(player.Name);
		}
	}
}
