using Godot;

public partial class QueueFreeWall : Area2D
{
	public override void _Ready()
		=> BodyEntered += OnBodyEntered;

	public void OnBodyEntered(Node body)
		=> body.QueueFree();
}
