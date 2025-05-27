using Godot;

public partial class Scoreboard : Node2D
{
	[Export]
	private string PlayerSprite = "player";
	private Sprite2D _sprite2d;
	private Label _label;
	public string DisplayText = "0";
	public float ScaleDisplay = 1.0f;

	public override void _Ready()
	{
		_sprite2d = GetNode<Sprite2D>("Sprite2D");
		_sprite2d.Texture = GD.Load<Texture2D>($"scenesGeneric/sprites/scoreboard-{PlayerSprite}.png");

		_label = GetNode<Label>("Label");
		_label.Modulate = new Color(0, 0, 0);
		_label.Text = DisplayText;

		Scale = new Vector2(ScaleDisplay, ScaleDisplay);
	}

	public void UpdateScore(int newScore)
		=> _label.Text = newScore.ToString();

	public void UpdateScoreAddOne()
		=> _label.Text = (int.Parse(_label.Text) + 1).ToString();
}
