using Godot;

public partial class Asteroid : CharacterBody2D
{
	public const float AsteroidVelocityX = 60f;
	private Sprite2D _sprite2d;
	private VisibleOnScreenNotifier2D _visibleNotifier2D;

	public override void _Ready()
	{
		_sprite2d = GetNode<Sprite2D>("Sprite2D");
		_visibleNotifier2D = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");

		_visibleNotifier2D.ScreenExited += OnScreenExited;
		ZIndex = -1;
	}

	public override void _Process(double delta)
		=> _sprite2d.Rotate(0.005f);


	public override void _PhysicsProcess(double delta)
		=> Position += new Vector2(-AsteroidVelocityX * (float)delta, 0);

	private void OnScreenExited()
		=> QueueFree();
}
