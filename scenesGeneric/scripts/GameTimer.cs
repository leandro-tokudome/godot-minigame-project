using Godot;

public partial class GameTimer : Node2D
{
	public int GameTime = 1;
	private Timer _timer;
	private Label _label;

	[Signal]
	public delegate void GameTimerFinishedEventHandler();

	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");
		_timer.Timeout += UpdateTime;

		_label = GetNode<Label>("Label");
		_label.Text = GameTime.ToString();

		ZIndex = 999;
	}

	public void UpdateTime()
	{
		GameTime -= 1;
		if (GameTime == 0)
			EmitSignal(SignalName.GameTimerFinished);

		_label.Text = GameTime.ToString();

		if (GameTime <= 10)
			_label.Modulate = new Color(1, 0, 0);
	}
}
