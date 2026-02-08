using Godot;
using System;
using Tater.Scripts;

public partial class UiController : Control
{
	[ExportCategory("Nodes")]
	[Export] private Control _pauseMenu;
	[Export] private Control _startMenu;
	[Export] private GameManager _gameManager;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_hideAllChildren();
		_gameManager.OnGameStateChange += _stateChanged;
	}

	private void _stateChanged(GameState state)
	{
		_hideAllChildren();
		
		switch (state)
		{
			case GameState.MainMenu:
				_startMenu.Visible = true;
				break;
			case GameState.GamePaused:
				_pauseMenu.Visible = true;
				break;
		}
	}

	

	private void _hideAllChildren()
	{
		// _hideChild(_pauseMenu);
		_startMenu.Visible = false;
		_pauseMenu.Visible = false;
	}

	private void _hideChild(Control child)
	{
		foreach (Control kid in child.GetChildren())
		{
			kid.Visible = false;
		}
	}

	private void _showChild(Control child)
	{
		foreach (Control kid in child.GetChildren())
		{
			kid.Visible = true;
		}
	}
}
