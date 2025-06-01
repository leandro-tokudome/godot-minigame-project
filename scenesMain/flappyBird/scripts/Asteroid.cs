using Godot;

public partial class Asteroid : CharacterBody2D
{
	public const float AsteroidVelocityX = 60f;
	private int RotationType;
	private float RotationVelocity;
	private RandomNumberGenerator Rng;
	private Sprite2D _sprite2d;
	private VisibleOnScreenNotifier2D _visibleNotifier2D;

	public override void _Ready()
	{
		Rng = new();
		RotationType = Rng.RandiRange(1, 2);
		RotationVelocity = RotationType == 1 ? 0.5f : -0.5f;

		_sprite2d = GetNode<Sprite2D>("Sprite2D");
		_visibleNotifier2D = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");

		_visibleNotifier2D.ScreenExited += OnScreenExited;
		ZIndex = -1;
	}

	public override void _Process(double delta)
		=> _sprite2d.Rotate(RotationVelocity * (float)delta);


	public override void _PhysicsProcess(double delta)
		=> Position += new Vector2(-AsteroidVelocityX * (float)delta, 0);

	private void OnScreenExited()
		=> QueueFree();
}
