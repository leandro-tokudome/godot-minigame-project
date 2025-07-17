using Godot;

namespace TimeGame;

public partial class Player : PlayerNodeAbstract
{
	private Main _main;
	public Label _label;
	public AnimatedSprite2D _animatedSprite;
	private RandomNumberGenerator Rng;
	public int Seconds = 0;
	public int Centiseconds = 0;
	public int StopTime = 0;
	public int VariationTime = 0;
	public bool CanPressButton = false;
	private int SortedTime;

	private float Alpha = 1.0f;

	public override void _Ready()
	{
		base._Ready();

		_main = GetParent() as Main;
		Rng = new();

		_label = GetNode<Label>("Label");
		_label.Modulate = new Color(0, 0, 0);
		_label.Text = "00:00";

		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite.Play("idle");
	}

	public override void Action()
	{
		if (CanPressButton)
		{
			CanPressButton = false;
			_animatedSprite.Play("action");

			StopTime = Centiseconds < 10 ? int.Parse($"{Seconds}0{Centiseconds}") : int.Parse($"{Seconds}{Centiseconds}");

			_main.ButtonsPressed++;
			_main.CheckButtonsPressed();
		}
	}

	public void UpdateTimer(double delta, float elapsedTime)
	{
		if (CanPressButton)
		{
			Seconds = (int)elapsedTime;
			Centiseconds = (int)((elapsedTime - Seconds) * 100);

			Alpha -= (float)(0.25f * delta);
			_label.Modulate = new Color(_label.Modulate.R, _label.Modulate.G, _label.Modulate.B, Alpha);

			string secondsText = Seconds < 10 ? $"0{Seconds}" : Seconds.ToString();
			string centisecondsText = Centiseconds < 10 ? $"0{Centiseconds}" : Centiseconds.ToString();

			_label.Text = $"{secondsText}:{centisecondsText}";
		}
	}

	public void ResetTimer()
	{
		Seconds = 0;
		Centiseconds = 0;
		StopTime = 0;

		Alpha = 1.0f;
		_label.Modulate = new Color(_label.Modulate.R, _label.Modulate.G, _label.Modulate.B, Alpha);

		string secondsText = Seconds < 10 ? $"0{Seconds}" : Seconds.ToString();
		string centisecondsText = Centiseconds < 10 ? $"0{Centiseconds}" : Centiseconds.ToString();

		_label.Text = $"{secondsText}:{centisecondsText}";
	}

	public void ShowTimer()
		=> _label.Modulate = new Color(_label.Modulate.R, _label.Modulate.G, _label.Modulate.B);
}
