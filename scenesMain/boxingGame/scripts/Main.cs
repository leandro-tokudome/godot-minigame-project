using Godot;
using System.Collections.Generic;

namespace Boxing;

public partial class Main : MainAbstract
{
	private RandomNumberGenerator Rng;

	public List<int> BlockSequence = [];

	public override void _Ready()
	{
		base._Ready();

		Rng = new();
		int randomBlockNumber;

		for (var index = 0; index < 200; index++)
		{
			randomBlockNumber = Rng.RandiRange(1, 3);
			BlockSequence.Add(randomBlockNumber);
		}
	}

	public override void _Process(double delta)
	{
	}
}
