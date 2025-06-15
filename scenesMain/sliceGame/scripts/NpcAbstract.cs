using Godot;
using SliceGame;

public abstract partial class NpcAbstract : Node2D
{
    private Main _main;
    protected AnimatedSprite2D _AnimatedSprite;
    protected Timer DespawnTimer;
    protected Timer QueueFreeTimerAfterDeath;
    public bool IsDead = false;

    public override void _Ready()
    {
        _main = GetParent() as Main;
        _AnimatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        QueueFreeTimerAfterDeath = new Timer
        {
            WaitTime = 0.5,
            OneShot = true,
            Autostart = false
        };
        QueueFreeTimerAfterDeath.Timeout += RemoveNpc;
        AddChild(QueueFreeTimerAfterDeath);

        DespawnTimer = new Timer
        {
            WaitTime = 1.0,
            OneShot = true,
            Autostart = true
        };
        DespawnTimer.Timeout += OnDespawnTimeout;
        AddChild(DespawnTimer);

        _AnimatedSprite.Play("idle");
    }

    private void OnDespawnTimeout()
    {
        if (!IsDead)
        {
            _main.CurrentNpc = null;
            RemoveNpc();
        }
    }

    private void RemoveNpc()
    {
        _main.CanSpawn();
        IsDead = true;
        QueueFree();
    }

    public int Die()
    {
        if (IsDead) return 0;

        IsDead = true;
        _main.CurrentNpc = null;
        _AnimatedSprite.Play("dead");
        QueueFreeTimerAfterDeath.Start();
        return
            this is Princess ? -2 :
            this is Orc ? 1 :
            2;
    }
}
