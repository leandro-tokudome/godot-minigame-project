using System.Collections.Generic;
using Godot;

namespace MeterGame;

public partial class Main : MainAbstract
{
	private RandomNumberGenerator Rng = new();

	public int SalmonSize = 16;
	public int OrangeSize = 12;
	public int RedSize = 8;
	public int WhiteSize = 4;
	public float InitialSalmonPosition;
	public float InitialOrangePosition;
	public float InitialRedPosition;
	public float InitialWhitePosition;
	public int PLayersFinishedBar = 0;

	public List<Player> PlayersList = [];

	public override void _Ready()
	{
		base._Ready();

		for (var index = 1; index <= 4; index++)
		{
			if (index <= Players) continue;

			var statue = GetNode<Sprite2D>($"Statue{index}");
			statue.QueueFree();
		}

		for (var index = 1; index <= Players; index++)
			PlayersList.Add(GetNode<Player>($"Player{index}"));

		ActivateGenerationPlayersNewBar();
	}

	public void GenerateNewBarPosition()
	{
		var initialBarPosition = Rng.RandfRange(
			-69.0f,
			69.0f - SalmonSize - OrangeSize - RedSize - WhiteSize
		);
		InitialSalmonPosition = initialBarPosition;
		InitialOrangePosition = InitialSalmonPosition + SalmonSize;
		InitialRedPosition = InitialOrangePosition + OrangeSize;
		InitialWhitePosition = InitialRedPosition + RedSize;
	}

	public void ActivateGenerationPlayersNewBar()
	{
		GenerateNewBarPosition();

		foreach (var player in PlayersList)
			player.GenerateNewBar();
	}
}
