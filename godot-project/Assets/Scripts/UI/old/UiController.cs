using Godot;
using System;
using Tater.Scripts;

public partial class UiController : Control
{
	[ExportCategory("Nodes")]
	[Export] private Control _pauseMenu;
	[Export] private Control _startMenu;
	[Export] private GameManager _gameManager;
	[Export] private Control _gameUi;
	
	[ExportCategory("Settings")]
	[Export] private float _hideDistance = 5000;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_hideAllChildren();
		// _gameManager.OnGameStateChange += _stateChanged;
	}

	private void _stateChanged(GameState state)
	{
		_hideAllChildren();
		
		switch (state)
		{
			case GameState.GameActive:
				_gameUi.Position = new Vector2(
					_gameUi.Position.X - _hideDistance,
					_gameUi.Position.Y - _hideDistance
				);
				break;
			case GameState.MainMenu:
				_startMenu.Position = new Vector2(
					_startMenu.Position.X - _hideDistance,
					_startMenu.Position.Y - _hideDistance
				);
				break;
			case GameState.GamePaused:
				_gameUi.Position = new Vector2(
					_gameUi.Position.X - _hideDistance,
					_gameUi.Position.Y - _hideDistance
				);
				_pauseMenu.Position = new Vector2(
					_pauseMenu.Position.X - _hideDistance,
					_pauseMenu.Position.Y - _hideDistance
				);
				break;
		}
	}

	

	private void _hideAllChildren()
	{
		_gameUi.Position = new Vector2(
			_gameUi.Position.X + _hideDistance,
			_gameUi.Position.Y + _hideDistance
		);
		_startMenu.Position = new Vector2(
			_startMenu.Position.X + _hideDistance,
			_startMenu.Position.Y + _hideDistance
		);
		_pauseMenu.Position = new Vector2(
			_pauseMenu.Position.X + _hideDistance,
			_pauseMenu.Position.Y + _hideDistance
		);
	}
}
