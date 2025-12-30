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
	public bool HasPhysic { get; set; } = false;
	[Export]
	public float GravityVelocity { get; set; } = 0f;
	public float VelocityY { get; set; } = 0f;
	public float VelocityX { get; set; } = 0f;

	private Timer CooldownTimer;

	public override void _Ready()
	{
		PlayerName = Name;

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
			VelocityY += GravityVelocity * (float)delta;
			Position += new Vector2(VelocityX * (float)delta, VelocityY * (float)delta);
			if (IsOnFloor())
				VelocityY = 0;
		}

		PhysicsProcessAction();

		MoveAndSlide();
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

	public virtual void Action() { }

	public virtual void PhysicsProcessAction() { }
}
