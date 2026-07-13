using Godot;

public partial class PlayerCharacterBodyAbstract : CharacterBody2D, IPlayer
{
	public string PlayerName { get; set; }
	public int InGameScore { get; set; } = 0;
	[Export]
	public string ActionButton { get; set; } = "action_p";
	[Export]
	public double ActionCooldown { get; set; } = 0;
	public bool IsAlive { get; set; } = true;
	[Export]
	public string RelativeScoreboard = null;

	[Export]
	public bool HasPhysic { get; set; } = false;
	[Export]
	public float GravityVelocity { get; set; } = 0f;
	public float VelocityX { get; set; } = 0f;

	private Scoreboard _scoreboard;
	private Timer CooldownTimer;

	public override void _Ready()
	{
		PlayerName = Name;

		if (!string.IsNullOrEmpty(RelativeScoreboard))
			_scoreboard = GetParent().GetNode<Scoreboard>(RelativeScoreboard);

		CooldownTimer = new Timer
		{
			WaitTime = ActionCooldown > 0 ? ActionCooldown : 0.01,
			OneShot = true,
			Autostart = false
		};

		AddChild(CooldownTimer);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (HasPhysic)
		{
			if (!IsOnFloor())
				Velocity += Vector2.Down * GravityVelocity * (float)delta;

			PhysicsProcessAction();
			Velocity = new Vector2(VelocityX, Velocity.Y);
			MoveAndSlide();
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (!string.IsNullOrEmpty(ActionButton) && @event.IsActionPressed(ActionButton) && !CooldownTimer.IsStopped())
			return;

		if (@event.IsActionPressed(ActionButton))
		{
			if (IsAlive)
				Action();
			if (ActionCooldown > 0)
				CooldownTimer.Start();
		}
	}

	public void VelocityYChange(float value)
		=> Velocity = new Vector2(Velocity.X, value);

	public void UpdateScoreboardScore(int newScore)
		=> _scoreboard.UpdateScore(newScore);

	public virtual void Action() { }

	public virtual void PhysicsProcessAction() { }
}
