using Godot;

namespace MeterGame;

public partial class Main : MainAbstract
{
	public override void _Ready()
	{
		base._Ready();

		for (var index = 1; index <= 4; index++)
		{
			if (index <= Players) continue;

			var statue = GetNode<Sprite2D>($"Statue{index}");
			statue.QueueFree();
		}
	}
}
