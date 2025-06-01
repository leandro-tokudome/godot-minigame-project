using System.Collections.Generic;
using Godot;

namespace FlappyBird;

public partial class Main : MainAbstract
{
	public List<string> PlayersAlive = [];
	private string LastGround;
	private int LastGroundIndex = 1;
	private const float FirstGroundX = 90;
	private const float GroundHeight = 156f;
	private float AsteroidHeightGap = 0f;
	public Player LeaderPlayer;
	public int HighScore = 0;
	[Export]
	private float AsteroidWallGap = 13.5f;
	private RandomNumberGenerator Rng;
	private Timer _asteroidSpawnTimer;
	private PackedScene _ground;
	private PackedScene _asteroid;
	private PackedScene _checkpoint;

	public override void _Ready()
	{
		base._Ready();

		Rng = new();
		_ground = GD.Load<PackedScene>("scenesMain/flappyBird/scenes/Ground.tscn");
		_asteroid = GD.Load<PackedScene>("scenesMain/flappyBird/scenes/Asteroid.tscn");
		_checkpoint = GD.Load<PackedScene>("scenesMain/flappyBird/scenes/Checkpoint.tscn");
		_asteroidSpawnTimer = GetNode<Timer>("Timer");

		LeaderPlayer = GetNode<Player>("Player1");

		_asteroidSpawnTimer.Timeout += OnTimeout;

		for (var index = 1; index <= Players; index++)
			PlayersAlive.Add($"Player{index}");

		SpawnInitialGrounds();
	}

	private void SpawnInitialGrounds()
	{
		Ground FirstGround = _ground.Instantiate<Ground>();
		Ground SecondGround = _ground.Instantiate<Ground>();
		Ground ThirdGround = _ground.Instantiate<Ground>();

		FirstGround.GlobalPosition = new Vector2(FirstGroundX, GroundHeight);
		SecondGround.GlobalPosition = new Vector2(GetNextGroundPositionX(FirstGround), GroundHeight);
		ThirdGround.GlobalPosition = new Vector2(GetNextGroundPositionX(SecondGround), GroundHeight);

		ThirdGround.Name = $"LastGround{LastGroundIndex}";
		LastGround = ThirdGround.Name;
		LastGroundIndex++;

		AddChild(FirstGround);
		AddChild(SecondGround);
		AddChild(ThirdGround);
	}

	public void GenerateNextGround()
	{
		float NextGroundPositionX = GetNextGroundPositionX(GetNode<Ground>(LastGround));
		Ground NextGround = _ground.Instantiate<Ground>();
		NextGround.GlobalPosition = new Vector2(NextGroundPositionX, GroundHeight);

		NextGround.Name = $"LastGround{LastGroundIndex}";
		LastGround = NextGround.Name;
		LastGroundIndex++;

		AddChild(NextGround);
	}

	private float GetNextGroundPositionX(Ground ground)
		=> ground.GlobalPosition.X + 179f;

	public void RemovePlayer(string playerName)
	{
		PlayersAlive.Remove(playerName);
		if (PlayersAlive.Count == 0)
			FinishGame();

		for (var index = 1; index <= Players; index++)
		{
			Player player = GetNodeOrNull<Player>($"Player{index}");
			if (player is null)
				continue;

			if (player.IsAlive)
			{
				LeaderPlayer = player;
				break;
			}
		}

	}

	private void OnTimeout()
		=> GenerateAsteroidWall();

	private void GenerateAsteroidWall()
	{
		var randomNumber = Rng.RandfRange(10, 105);

		for (var index = 1; index <= 4; index++)
		{
			var asteroidTop = _asteroid.Instantiate<Asteroid>();
			var asteroidBottom = _asteroid.Instantiate<Asteroid>();

			asteroidTop.GlobalPosition = new Vector2(271, randomNumber - ((AsteroidWallGap / 2) + (32 * index)));
			asteroidBottom.GlobalPosition = new Vector2(271, randomNumber + (AsteroidWallGap / 2) + (32 * index));

			AddChild(asteroidTop);
			AddChild(asteroidBottom);
		}

		var checkpoint = _checkpoint.Instantiate<Checkpoint>();
		checkpoint.GlobalPosition = new Vector2(271 + 13f + (((float)_asteroidSpawnTimer.WaitTime + Asteroid.AsteroidVelocityX) / 2), GroundHeight - 20 - 13);

		AddChild(checkpoint);
	}

	public override void CountWin()
	{
		if (FinishedByTime)
			WinnerPlayers = PlayersAlive;
		else
			CountWinMostPointed();
	}
}
