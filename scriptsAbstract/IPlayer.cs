using Godot;

public interface IPlayer
{
    string PlayerName { get; set; }
    int InGameScore { get; set; }
    string ActionButton { get; set; }
    double ActionCooldown { get; set; }
    bool IsAlive { get; set; }
    bool HasPhysic { get; set; }
    float GravityVelocity { get; set; }
    float VelocityY { get; set; }
    float VelocityX { get; set; }
}