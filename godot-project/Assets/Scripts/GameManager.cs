using Godot;
using System;

namespace Tater.Scripts;

public enum GameState
{
	Initializing,
	MainMenu,
	MainMenuSettings,
	GameActive,
	GamePaused,
	GamePausedSettings,
	GameOver,
}

public partial class GameManager : Node
{
	[Signal] public delegate void OnGameStateChangeEventHandler(GameState newState, GameState oldState);
	
	[ExportCategory("Node References")]
	[Export] private PlayerBrain _player;
	public PlayerBrain Player => _player;
	[Export] private EnemyPool _pool;
	[Export] private Camera3D _camera;
	[Export] private ShapeDrawing _draw;
	
	[ExportCategory("Settings")]
	[Export] private float _cameraMenuHeight = 160f;
	[Export] private float _cameraGameHeight = 80f;

	private GameState _gameState = GameState.Initializing;
	public GameState GameState
	{
		get => _gameState;
		set
		{
			GD.Print("GameManager: State switching from " + _gameState + " to " + value + "!");
			EmitSignalOnGameStateChange(value, _gameState);
			_gameState = value;
			switch (value)
			{
				case GameState.MainMenu:
					_goToMainMenu();
					break;
			}
		} 
	}

	private void _goToMainMenu()
	{
		ResetGame();
	}

	private int _score = 0;
	public int Score => _score;

	private int _scoreIncrement = 1;
	private float _timeSinceLastScoreIncrement = 0f;

	public override void _Ready()
	{
		if (_player == null || _pool == null || _camera == null || _draw == null)
		{
			throw new Exception("GameManagerNode is missing node references!");
		}

		GameState = GameState.MainMenu;
		ResetGame();
	}

	public override void _EnterTree()
	{
		Global.Instance.SetGameManager(this);
		
		_draw.OnDrawingStart += OnDrawingStart;
		_draw.OnCast += OnCast;
		_pool.OnPawnKill += value => _score += value;
		_player.Health.OnLethalDamage += EndGame;
	}
	
	public override void _ExitTree()
	{
		_draw.OnDrawingStart -= OnDrawingStart;
		_draw.OnCast -= OnCast;
	}

	private void EndGame()
	{
		GD.Print("GAME ENDED!!!!! " + _score);
	}

	public void ResetGame()
	{
		_score = 0;
		_player.Visible = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_gameState == GameState.GameActive)
		{
			_player._BrainPhysicsProcess(delta);
			_pool._BrainProcess(delta);
			_pool._BrainPhysicsProcess(delta);
			_draw._ShapeDrawingProcess(delta);

			if (_timeSinceLastScoreIncrement > _scoreIncrement)
			{
				_score += (int)_timeSinceLastScoreIncrement * 10;
				_timeSinceLastScoreIncrement = 0f;
			}

			_timeSinceLastScoreIncrement += (float)delta;
			
			if (Math.Abs(_camera.Position.Y - _cameraGameHeight) > 0.5f)
			{
				Vector3 targetPos = new Vector3(
					_camera.Position.X,
					_cameraGameHeight,
					_camera.Position.Z
				);
				_camera.Position = _camera.Position.Lerp(targetPos, (float)delta);
			}
		}

		if (_gameState == GameState.MainMenu)
		{
			if (Math.Abs(_camera.Position.Y - _cameraMenuHeight) > 0.5f)
			{
				Vector3 targetPos = new Vector3(
					_camera.Position.X,
					_cameraMenuHeight,
					_camera.Position.Z
				);
				_camera.Position = _camera.Position.Lerp(targetPos, (float)delta);
			}
		}
	}

	public void OnDrawingStart()
	{
		_player.StartCasting();
	}

	public void OnCast(AttackShape cast)
	{
		foreach (EnemyBrain pawn in _pool.Pawns)
		{
			Vector2 onScreen = _camera.UnprojectPosition(pawn.GlobalPosition);
			if (Geometry2D.IsPointInPolygon(onScreen, cast.Points) && pawn.Shape == cast.Shape)
			{
				_pool.DamagePawn(pawn, 1);
			}
		}

		_player.StopCasting();
	}
}
