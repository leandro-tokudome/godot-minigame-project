using FlappyBird;
using Godot;

public partial class Ground : Node2D
{
	private Main _main;
	[Export]
	private float GroundSpeed = 50f;
	private VisibleOnScreenNotifier2D _visibleNotifier2D;

	public override void _Ready()
	{
		_main = GetParent() as Main;

		_visibleNotifier2D = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
		_visibleNotifier2D.ScreenExited += OnScreenExited;
	}


	public override void _PhysicsProcess(double delta)
	{
		Position += new Vector2(-GroundSpeed * (float)delta, 0);
	}

	private void OnScreenExited()
	{
		_main.GenerateNextGround();
		QueueFree();
	}
}
