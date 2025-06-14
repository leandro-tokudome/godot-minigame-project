using Godot;

namespace CupShuffle;

public partial class Player : PlayerNodeAbstract
{
	private Main _main;
	private PackedScene PlayerArrowPackedScene;
	private Marker2D _arrowMarker2d;
	private Vector2 _arrowStartPosition;
	private Node2D PlayerArrow;
	public int PlayerChoice = 1;

	public override void _Ready()
	{
		base._Ready();

		_main = GetParent() as Main;
		_arrowMarker2d = _main.GetNode<Marker2D>($"{Name}ArrowMarker");
		_arrowStartPosition = _arrowMarker2d.GlobalPosition;

		PlayerArrowPackedScene = GD.Load<PackedScene>("scenesMain/cupShuffle/scenes/Arrow.tscn");
	}

	public override void Action()
	{
		if (_main.IsPlayerChoice)
		{
			PlayerChoice++;

			if (PlayerChoice > 4)
			{
				PlayerChoice = 1;
				PlayerArrow.Position = _arrowStartPosition;
			}
			else
				PlayerArrow.Position += new Vector2(31f, 0);
		}
	}

	public void SetPlayerArrow()
	{
		PlayerChoice = 1;

		var playerArrow = PlayerArrowPackedScene.Instantiate<Node2D>();
		playerArrow.Name = $"{Name}Arrow";

		Sprite2D playerArrowSprite = playerArrow.GetNode<Sprite2D>("Sprite2D");
		var nameToLower = Name.ToString().ToLower();
		playerArrowSprite.Texture = GD.Load<Texture2D>($"scenesMain/cupShuffle/sprites/{nameToLower}/{nameToLower}-arrow.png");

		playerArrow.Position = _arrowStartPosition;

		PlayerArrow = playerArrow;
		_main.AddChild(playerArrow);
	}
}
