using System.Collections.Generic;
using Godot;

namespace TimeGame;

public partial class Main : MainAbstract
{
	private Label GameTimerLabel;
	private RandomNumberGenerator Rng;
	private Timer StartPlayerTimer;
	private Timer FinishRoundTimer;
	private List<Player> PlayersList = [];
	private int Round = 1;
	private int SortedTime = 0;
	public int ButtonsPressed = 0;
	private float ElapsedTime = 0f;
	public bool IsRunningTime = false;

	public override void _Ready()
	{
		base._Ready();

		StartPlayerTimer = new Timer { WaitTime = 5, Autostart = false, OneShot = true };
		FinishRoundTimer = new Timer { WaitTime = 5, Autostart = false, OneShot = true };
		StartPlayerTimer.Timeout += OnStartPlayerTimeout;
		FinishRoundTimer.Timeout += OnFinishRoundTimeout;
		AddChild(StartPlayerTimer);
		AddChild(FinishRoundTimer);


		GameTimerLabel = GetNode<Label>("Label");
		GameTimerLabel.Modulate = new Color(0, 0, 0);
		Rng = new();

		for (var index = 1; index <= Players; index++)
		{
			Player player = GetNode<Player>($"Player{index}");
			PlayersList.Add(player);
		}

		StartGame();
	}

	private void StartGame()
	{
		DisplaySortedTime();
		StartPlayerTimer.Start();
	}

	public override void _Process(double delta)
	{
		if (!IsRunningTime)
			return;

		ElapsedTime += (float)delta;

		foreach (Player player in PlayersList)
			player.UpdateTimer(delta, ElapsedTime);

		if (ElapsedTime == 30.0f || ElapsedTime > 30.0f)
		{
			IsRunningTime = false;

			foreach (Player player in PlayersList)
			{
				if (player.CanPressButton)
					player.StopTime = 30;

				player.CanPressButton = false;
			}

			FinishRound();
		}
	}

	private void DisplaySortedTime()
	{
		SortTime();
		string secondsText = SortedTime < 10 ? $"0{SortedTime}" : SortedTime.ToString();
		GameTimerLabel.Text = $"{secondsText}:00";
		GameTimerLabel.Modulate = new Color(0, 0, 0);
	}

	public void FinishRound()
	{
		IsRunningTime = false;
		ShowPlayersTime();
		FinishRoundTimer.Start();
		GameTimerLabel.Modulate = new Color(1, 0, 0);

		CountRoundWin();
		ButtonsPressed = 0;
	}

	public void CheckButtonsPressed()
	{
		if (ButtonsPressed == Players)
			FinishRound();
	}

	private void ShowPlayersTime()
	{
		foreach (Player player in PlayersList)
			player.ShowTimer();
	}

	private void SortTime()
		=> SortedTime = Rng.RandiRange(8, 20);

	private void OnStartPlayerTimeout()
	{
		ButtonsPressed = 0;
		ElapsedTime = 0;

		foreach (Player player in PlayersList)
		{
			player.Seconds = 0;
			player.Centiseconds = 0;
			player.StopTime = 0;
			player.CanPressButton = true;
		}

		IsRunningTime = true;
	}

	private void OnFinishRoundTimeout()
	{
		if (Round == 3)
			FinishGame();

		Round++;

		foreach (Player player in PlayersList)
		{
			player._label.Modulate = new Color(0, 0, 0);
			player._animatedSprite.Play("idle");
			player.ResetTimer();
		}

		DisplaySortedTime();
		StartPlayerTimer.Start();
	}

	private void CountRoundWin()
	{
		int shorterTime = 99999;

		foreach (Player player in PlayersList)
		{
			int variationTime = SortedTime * 100 - player.StopTime;
			int treatedVariationTime = variationTime < 0 ? variationTime * -1 : variationTime;

			player.VariationTime = treatedVariationTime;

			if (treatedVariationTime < shorterTime)
				shorterTime = treatedVariationTime;
		}

		foreach (Player player in PlayersList)
		{
			if (player.VariationTime == shorterTime)
			{
				player.InGameScore++;
				player.UpdateScoreboardScore(player.InGameScore);
				player._label.Modulate = new Color(1, 0, 0);
			}
		}
	}
}
