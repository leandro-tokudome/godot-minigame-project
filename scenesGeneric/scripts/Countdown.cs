using Godot;

public partial class Countdown : Control
{
	private Sprite2D _sprite2d;
	private Sprite2D _background;
	private Timer _timer;
	private string[] _sprites = { "countdown2", "countdown1", "countdownGo" };
	private int _currentIndex = 0;

	[Signal]
	public delegate void CountdownFinishedEventHandler();

	public override void _Ready()
	{
		ZIndex = 99;
		ProcessMode = ProcessModeEnum.Always;

		_sprite2d = GetNode<Sprite2D>("Sprite2D");
		_background = GetNode<Sprite2D>("Background");
		_timer = GetNode<Timer>("Timer");

		_sprite2d.Texture = GD.Load<Texture2D>($"scenesGeneric/sprites/countdown3.png");
		_background.Texture = GD.Load<Texture2D>($"scenesGeneric/sprites/countdownBackground.png");
		_background.ZIndex = -1;
		_timer.Timeout += OnTimeout;
	}

	public void OnTimeout()
	{
		if (_currentIndex < _sprites.Length)
		{
			_sprite2d.Texture = GD.Load<Texture2D>($"scenesGeneric/sprites/{_sprites[_currentIndex]}.png");
		}
		else
		{
			_timer.Stop();
			EmitSignal(SignalName.CountdownFinished);
			QueueFree();
		}

		_currentIndex++;
	}
}
