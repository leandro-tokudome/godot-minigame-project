using Godot;

namespace CoinGame;

public partial class Main : MainAbstract
{
	public Marker2D _pointSpawn;
	public Timer _pointSpawnTimer;
	public Timer _missileSpawnTimer;
	public PackedScene _point;
	public PackedScene _missile;

	public RandomNumberGenerator Rng;

	public float DiamondSpawnChance = 0.3f;
	public float MissileSpawnChance = 0.3f;

	public override void _Ready()
	{
		base._Ready();

		Rng = new RandomNumberGenerator();

		_pointSpawn = GetNode<Marker2D>("PointSpawn");
		_pointSpawnTimer = GetNode<Timer>("PointSpawnTimer");
		_missileSpawnTimer = GetNode<Timer>("MissileSpawnTimer");
		_point = GD.Load<PackedScene>("scenesMain/coinGame/scenes/Point.tscn");
		_missile = GD.Load<PackedScene>("scenesMain/coinGame/scenes/Missile.tscn");

		_pointSpawnTimer.Timeout += OnPointSpawnTimerTimeout;
		_missileSpawnTimer.Timeout += OnMissileSpawnTimerTimeout;
	}

	public void OnPointSpawnTimerTimeout()
	{
		var pointInstance = _point.Instantiate<Point>();

		var spawnPosition = GetValidPointSpawnPosition();
		_pointSpawn.Position = spawnPosition;
		pointInstance.Position = spawnPosition;
		AddChild(pointInstance);
		pointInstance.CallDeferred(nameof(Point.DefineValue), Rng.Randf() < 1f - DiamondSpawnChance);
	}

	public Vector2 GetValidPointSpawnPosition()
	{
		const float minDistanceFromPlayers = 30f;
		const int maxAttempts = 100;

		for (int attempt = 0; attempt < maxAttempts; attempt++)
		{
			var randomIndexX = Rng.RandiRange(0, 26);
			var randomIndexY = Rng.RandiRange(0, 13);
			var candidatePosition = new Vector2(11f + (9 * randomIndexX), 13.5f + (9 * randomIndexY));

			if (randomIndexX != 13
			&& randomIndexY is not 6 and not 7
			&& IsSpawnPositionValid(candidatePosition, minDistanceFromPlayers))
				return candidatePosition;
		}

		return _pointSpawn.Position;
	}

	public bool IsSpawnPositionValid(Vector2 candidatePosition, float minDistanceFromPlayers)
	{
		foreach (var child in GetChildren())
		{
			if (child is Point existingPoint)
			{
				if (existingPoint.Position.DistanceSquaredTo(candidatePosition) < 1f)
					return false;
			}
			else if (child is Player player)
			{
				if (player.Position.DistanceSquaredTo(candidatePosition) < minDistanceFromPlayers * minDistanceFromPlayers)
					return false;
			}
		}

		return true;
	}

	public void OnMissileSpawnTimerTimeout()
	{
		if (Rng.Randf() < MissileSpawnChance)
			SpawnMissile();
	}

	public void SpawnMissile()
	{
		var missileInstance = _missile.Instantiate<Missile>();
		var missileDirection = Rng.RandiRange(0, 1) == 0 ? -1 : 1;

		var spawnPosition = new Vector2(missileDirection == 1 ? -20f : 276f, Rng.RandfRange(20f, 124f));
		missileInstance.Position = spawnPosition;
		missileInstance.Direction = missileDirection;
		AddChild(missileInstance);
		missileInstance.CallDeferred(nameof(Missile.Prepare));
	}
}
