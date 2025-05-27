using FlappyBird;
using Godot;

public partial class Checkpoint : Node2D
{
	private Main _main;
	private Area2D _area2d;

	public override void _Ready()
	{
		_main = GetParent() as Main;

		_area2d = GetNode<Area2D>("Area2D");
		_area2d.BodyEntered += OnBodyEntered;
	}

	public override void _PhysicsProcess(double delta)
		=> Position += new Vector2(-(Asteroid.AsteroidVelocityX * (float)delta), 0);

	private void OnBodyEntered(Node body)
	{
		if (body is Player player)
		{
			player.InGameScore++;

			if (_main.PlayersAlive.Count == 1)
			{
				_main.WinnerPlayers.Add(_main.PlayersAlive[0]);
				_main.FinishGame();
			}
		}
	}
}
