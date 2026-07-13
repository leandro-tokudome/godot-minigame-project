using Godot;

public partial class Pipe : CharacterBody2D
{
	public float velocity = 50.0f;
	public int Direction = 1;

	public override void _PhysicsProcess(double delta)
	{
		if (IsOnCeiling() || IsOnFloor())
			Direction *= -1;
		Velocity = new Vector2(0, -velocity * Direction);
		MoveAndSlide();
	}
}
