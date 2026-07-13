using Godot;

public partial class Scoreboard : Node2D
{
	[Export]
	private string PlayerSprite = "player";
	private Sprite2D _sprite2d;
	public Label _label;
	public string DisplayText = "0";
	[Export]
	public float ScaleDisplay = 1.0f;
	[Export]
	public bool IsTransparent = false;
	[Export]
	public string LabelColor = "black";

	public override void _Ready()
	{
		var labelColor = Colors.Black;
		switch (LabelColor.ToLower())
		{
			case "red":
				labelColor = Colors.Red;
				break;
			case "green":
				labelColor = Colors.Green;
				break;
			case "blue":
				labelColor = Colors.Blue;
				break;
			case "yellow":
				labelColor = Colors.Yellow;
				break;
		}

		_sprite2d = GetNode<Sprite2D>("Sprite2D");
		_sprite2d.Texture = GD.Load<Texture2D>($"scenesGeneric/sprites/scoreboard-{PlayerSprite}.png");
		if (IsTransparent)
			_sprite2d.Visible = false;

		_label = GetNode<Label>("Label");
		_label.Text = DisplayText;

		if (_label.LabelSettings != null)
		{
			_label.LabelSettings = _label.LabelSettings.Duplicate() as LabelSettings;
			_label.LabelSettings.FontColor = labelColor;
		}

		Scale = new Vector2(ScaleDisplay, ScaleDisplay);
		ZIndex = 1000;
	}

	public void UpdateScore(int newScore)
		=> _label.Text = newScore.ToString();

	public void UpdateScoreAddOne()
		=> _label.Text = (int.Parse(_label.Text) + 1).ToString();
}
