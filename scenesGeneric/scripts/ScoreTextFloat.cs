using Godot;

public partial class ScoreTextFloat : Node2D
{
	[Export]
	public float Speed = 25f;
	[Export]
	public float FadeSpeed = 1.0f;
	private float _alpha = 1.0f;
	[Export]
	public bool IsRed = false;
	public Color LabelColor = new(1, 1, 1);
	public string DisplayText = "";
	private Label _label;
	public float ScaleDisplay = 1.0f;

	public override void _Ready()
	{
		_label = GetNode<Label>("Label");

		_label.Text = DisplayText;
		if (IsRed)
			_label.Modulate = new Color(1, 0, 0, _alpha);
		else
			_label.Modulate = LabelColor;

		Scale = new Vector2(ScaleDisplay, ScaleDisplay);
	}

	public override void _Process(double delta)
	{
		Position = new Vector2(Position.X, Position.Y - (float)(Speed * delta));

		_alpha -= (float)(FadeSpeed * delta);

		_label.Modulate = new Color(_label.Modulate.R, _label.Modulate.G, _label.Modulate.B, _alpha);

		if (_alpha <= 0)
			QueueFree();
	}
}
