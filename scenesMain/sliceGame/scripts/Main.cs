using System;
using Godot;

namespace SliceGame;

public partial class Main : MainAbstract
{
	private RandomNumberGenerator Rng;
	private Timer CanSpawnTimer;
	private Timer TrySpawnTimer;
	private Timer ForceSpawnTimer;
	private Marker2D _marker2d;
	private PackedScene _orc;
	private PackedScene _chiefOrc;
	private PackedScene _princess;
	public Node2D CurrentNpc;

	public override void _Ready()
	{
		base._Ready();

		_marker2d = GetNode<Marker2D>("SpawnPoint");
		_orc = GD.Load<PackedScene>("scenesMain/sliceGame/scenes/Orc.tscn");
		_chiefOrc = GD.Load<PackedScene>("scenesMain/sliceGame/scenes/ChiefOrc.tscn");
		_princess = GD.Load<PackedScene>("scenesMain/sliceGame/scenes/Princess.tscn");

		CanSpawnTimer = new Timer { WaitTime = 0.7, OneShot = true };
		TrySpawnTimer = new Timer { WaitTime = 0.015, Autostart = true };
		ForceSpawnTimer = new Timer { WaitTime = 3.0, OneShot = true };

		CanSpawnTimer.Timeout += StartSpawnTimer;
		TrySpawnTimer.Timeout += TrySpawn;
		ForceSpawnTimer.Timeout += ForceSpawn;

		Rng = new();
		Rng.Randomize();

		AddChild(CanSpawnTimer);
		AddChild(TrySpawnTimer);
		AddChild(ForceSpawnTimer);
		ForceSpawnTimer.Start();
	}

	public void TrySpawn()
	{
		if (Rng.Randf() <= 0.02)
			SpawnNpc();
	}

	public void ForceSpawn()
		=> SpawnNpc();

	public void SpawnNpc()
	{
		var randomNumber = Rng.Randf();
		PackedScene chosenNpc =
			randomNumber <= 0.30 ? _princess :
			randomNumber > 0.30 && randomNumber <= 0.85 ? _orc :
			_chiefOrc;
		CurrentNpc = chosenNpc.Instantiate<Node2D>();
		CurrentNpc.Position = _marker2d.GlobalPosition;

		AddChild(CurrentNpc);
		TrySpawnTimer.Stop();
		ForceSpawnTimer.Stop();
	}

	public void CanSpawn()
		=> CanSpawnTimer.Start();

	public void StartSpawnTimer()
	{
		TrySpawnTimer.Start();
		ForceSpawnTimer.Start();
	}
}
