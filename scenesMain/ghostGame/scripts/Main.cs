using Godot;
using System;

namespace GhostGame;

public partial class Main : MainAbstract
{
	public Timer _spawnTimer;
	public Marker2D _player1SpawnMarker;
	public Marker2D _player2SpawnMarker;
	public Marker2D _player3SpawnMarker;
	public Marker2D _player4SpawnMarker;
	public Area2D _area2d;
	public PackedScene _ghost;
	public PackedScene _angel;

	public RandomNumberGenerator Rng;

	public float SpawnChance = 0.7f;
	public float GhostSpawnChance = 0.6f;
	public float EntityVelocity = 100f;
	public bool AccelerateEntities = false;

	public override void _Ready()
	{
		base._Ready();

		_spawnTimer = GetNode<Timer>("SpawnTimer");
		_spawnTimer.Timeout += OnSpawnTimerTimeout;
		_player1SpawnMarker = GetNode<Marker2D>("Player1SpawnMarker2D");
		_player2SpawnMarker = GetNode<Marker2D>("Player2SpawnMarker2D");
		_player3SpawnMarker = GetNode<Marker2D>("Player3SpawnMarker2D");
		_player4SpawnMarker = GetNode<Marker2D>("Player4SpawnMarker2D");
		_area2d = GetNode<Area2D>("Area2D");
		_area2d.AreaEntered += OnAreaEntered;
		_ghost = GD.Load<PackedScene>("res://scenesMain/ghostGame/scenes/Ghost.tscn");
		_angel = GD.Load<PackedScene>("res://scenesMain/ghostGame/scenes/Angel.tscn");

		Rng = new RandomNumberGenerator();
	}

	public void OnSpawnTimerTimeout()
	{
		if (Rng.Randf() < 1 - SpawnChance)
			return;

		var entityToSpawn = Rng.Randf() < GhostSpawnChance ? _ghost : _angel;

		for (int i = 1; i <= Players; i++)
		{
			var spawnMarker = GetNode<Marker2D>($"Player{i}SpawnMarker2D");
			var entityInstance = entityToSpawn.Instantiate<Node2D>();
			entityInstance.Position = spawnMarker.Position;
			AddChild(entityInstance);

			var entityScript = entityInstance as Entity;
			if (entityScript is null)
				continue;

			entityScript.OwnerPlayerIndex = i;
			entityScript.IsAngel = entityToSpawn == _angel;
			entityScript.EntityVelocity = EntityVelocity;
		}

		// Accelerate entities over time
		if (_spawnTimer.WaitTime > 0.5f && AccelerateEntities)
		{
			_spawnTimer.WaitTime -= 0.01f;
			EntityVelocity += 5f;
		}
	}

	public void OnAreaEntered(Node entity)
		=> entity.QueueFree();
}
