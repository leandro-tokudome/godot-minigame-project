using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class MainAbstract : Node2D
{
	[Export]
	private bool ShowTimer = false;
	[Export]
	private int GameTime = 0;
	[Export]
	public int Players = 2;
	public readonly List<IPlayer> PlayerProperties = [];
	public List<string> WinnerPlayers = [];

	private PackedScene _countdown;
	private PackedScene _finish;
	private PackedScene _timer;
	private Vector2 _screenSize;

	public override void _Ready()
	{
		GetTree().Paused = true;

		_countdown = GD.Load<PackedScene>("scenesGeneric/scenes/Countdown.tscn");
		_finish = GD.Load<PackedScene>("scenesGeneric/scenes/Finish.tscn");
		_timer = GD.Load<PackedScene>("scenesGeneric/scenes/GameTimer.tscn");

		_screenSize = GetViewport().GetVisibleRect().Size;

		int supportIndex = 4;
		for (int index = 1; index <= Players; index++)
		{
			PlayerProperties.Add((IPlayer)GetNode($"Player{index}"));

			if (supportIndex > Players)
			{
				GetNode($"Player{supportIndex}").QueueFree();
				if (HasNode($"Scoreboard{supportIndex}"))
					GetNode($"Scoreboard{supportIndex}").QueueFree();
				supportIndex--;
			}
		}

		if (ShowTimer) ShowGameTimer();
		ShowCountdown();
	}

	public void ShowCountdown()
	{
		Countdown countdown = _countdown.Instantiate<Countdown>();
		AddChild(countdown);
		countdown.CountdownFinished += OnCountdownFinish;

		countdown.GlobalPosition = _screenSize / 2.0f;
	}

	public void FinishGame()
	{
		Control finish = _finish.Instantiate<Control>();
		finish.GlobalPosition = _screenSize / 2.0f;
		AddChild(finish);

		GetTree().Paused = true;
		CountWin();
	}

	public void ShowGameTimer()
	{
		GameTimer timer = _timer.Instantiate<GameTimer>();
		timer.GameTime = GameTime;
		timer.GameTimerFinished += OnGameTimerFinish;
		AddChild(timer);

		timer.Position = new Vector2(_screenSize.X / 2.0f, 10);
	}

	public void CountWinMostPointed()
	{
		int greaterPoint = int.MinValue;
		greaterPoint = PlayerProperties
			.Select(x => x.InGameScore)
			.Max();

		foreach (var player in PlayerProperties)
			if (player.InGameScore == greaterPoint) WinnerPlayers.Add(player.PlayerName);
	}

	private void OnGameTimerFinish()
		=> FinishGame();

	private void OnCountdownFinish()
		=> GetTree().Paused = false;

	public virtual void CountWin()
		=> CountWinMostPointed();
}
