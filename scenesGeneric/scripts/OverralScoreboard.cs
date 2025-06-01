using System.Collections.Generic;
using Godot;

public partial class OverralScoreboard : Node2D
{
	[Export]
	private int Players = 2;
	private Dictionary<string, string> ScoreboardDic;

	public override void _Ready()
	{
		int supportIndex = 4;
		for (int index = 1; index <= Players; index++)
		{
			ScoreboardDic.Add($"Player{index}", $"Scoreboard{index}");

			if (supportIndex > Players)
			{
				Scoreboard scoreboard = GetNode<Scoreboard>($"Scoreboard{supportIndex}");
				scoreboard.QueueFree();
				supportIndex--;
			}
		}
	}

	public void UpdateScores()
	{
		MainAbstract main = GetNode<MainAbstract>("Main");
		foreach (string player in main.WinnerPlayers)
		{
			Scoreboard scoreboard = GetNode<Scoreboard>(ScoreboardDic[player]);
			scoreboard.UpdateScoreAddOne();
		}
	}
}
