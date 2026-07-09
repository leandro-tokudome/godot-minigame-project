using Godot;

namespace MainMenu;

public partial class Main : Node2D
{
	private Button _playButton;
	private Button _exitButton;

	public override void _Ready()
	{
		_playButton = GetNode<Button>("Control/VBoxContainer/Play");
		_exitButton = GetNode<Button>("Control/VBoxContainer/Exit");

		_playButton.Pressed += OnPlayPressed;
		_exitButton.Pressed += OnExitPressed;
	}

	public void OnPlayPressed()
	{

	}

	public void OnExitPressed()
		=> GetTree().Quit();
}
