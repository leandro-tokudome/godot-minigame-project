using System.Runtime.CompilerServices;
using Godot;

namespace Fireball;

public partial class Main : MainAbstract
{
	private PackedScene _fireball;
	private Timer _fireballSummonTimer;

	private RandomNumberGenerator Rng;
	public int WinningPoints = 1;

	public override void _Ready()
	{
		base._Ready();

		_fireball = GD.Load<PackedScene>("scenesMain/fireballGame/scenes/Fireball.tscn");

		_fireballSummonTimer = new Timer
		{
			WaitTime = 0.225,
			Autostart = true,
			OneShot = false
		};

		_fireballSummonTimer.Timeout += SummonFireball;
		AddChild(_fireballSummonTimer);

		Rng = new();
	}

	private void SummonFireball()
	{
		var fireball = _fireball.Instantiate<Fireball>();
		fireball.Scale = new Vector2(0.65f, 0.65f);
		fireball.Position = new Vector2(Rng.RandfRange(0, 256), -20);
		AddChild(fireball);
	}

	public override void CountWin()
	{
		CountWinMostPointed();
	}
}
