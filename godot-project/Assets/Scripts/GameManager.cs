using Godot;
using System;
using System.Collections.Generic;

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
	[ExportCategory("Node References")]
	[Export] private EnemyPool _pool;
	[Export] private Camera3D _camera;
	[Export] private ShapeDrawing _draw;
	[Export] private PlayerBrain _player;
	[Export] private AudioStreamPlayer2D _uiClick;
	public PlayerBrain Player => _player;
	private Vector3 _playerStartPos;
	
	public override void _PhysicsProcess(double delta)
	{
		CameraLerpProcess(delta);
		
		if (_gameState == GameState.GameActive)
		{
			_player._PlayerPhysicsProcess(delta);
			_pool._PoolPhysicsProcess(delta);
			_draw._DrawingPhysicsProcess(delta);
			
			SuriveScoreProcess(delta);
		}
	}
	
	
	#region GameState
	
	[Signal] public delegate void OnGameStateChangeEventHandler(GameState newState, GameState oldState);
	[Signal] public delegate void OnGameResetEventHandler();

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
					ResetGame();
					break;
			}
		} 
	}

	public void GameEnd()
	{
		GameState = GameState.GameOver;
	}
	
	public void ResetGame()
	{
		EmitSignalOnGameReset();
		_player.Position = _playerStartPos;
		_score = 0;
		_timeSinceLastSurviveScore = 0f;
	}

	public void PlayUiClickSound()
	{
		_uiClick.Play();
	}

	#endregion

	

	#region Score

	private int _score = 0;
	public int Score => _score;

	private int _timeBetweenSurviveScore = 1;
	private float _timeSinceLastSurviveScore = 0f;

	private void SuriveScoreProcess(double delta)
	{
		if (_timeSinceLastSurviveScore > _timeBetweenSurviveScore)
		{
			_score += (int)_timeSinceLastSurviveScore * 10;
			_timeSinceLastSurviveScore = 0f;
		}
		_timeSinceLastSurviveScore += (float)delta;
	}

	public void AddScore(int add)
	{
		_score += add;
	}

	#endregion

	

	# region CameraLerp
	
	[ExportCategory("Camera Lerp Settings")]
	[Export] private float _cameraMenuHeight = 160f;
	[Export] private float _cameraGameHeight = 80f;

	private bool _cameraInPosition = false;

	private void _setCameraInPositionToFalseOnStateChange(GameState newState, GameState oldState)
	{
		_cameraInPosition = false;
	}

	private void CameraLerpProcess(double delta)
	{
		if (_cameraInPosition) return;
		
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
			else _cameraInPosition = true;
		} else if (_gameState is GameState.GameActive or GameState.GamePaused or GameState.GamePausedSettings)
		{
			if (Math.Abs(_camera.Position.Y - _cameraGameHeight) > 0.5f)
			{
				Vector3 targetPos = new Vector3(
					_camera.Position.X,
					_cameraGameHeight,
					_camera.Position.Z
				);
				_camera.Position = _camera.Position.Lerp(targetPos, (float)delta);
			}
			else _cameraInPosition = true;
		}
	}
	
	#endregion
	
	

	#region EnterAndExitTree

	public override void _EnterTree()
	{
		if (_player == null || _pool == null || _camera == null || _draw == null)
		{
			throw new Exception("GameManagerNode is missing node references!");
		}
		
		Global.Instance.SetGameManager(this);
		_playerStartPos = _player.Position;

		this.OnGameStateChange += _setCameraInPositionToFalseOnStateChange;
		
		_draw.OnDrawingStart += _onDrawingStart;
		_draw.OnCast += _onDrawingEnd;
	}
	
	public override void _ExitTree()
	{
		this.OnGameStateChange -= _setCameraInPositionToFalseOnStateChange;
		
		_draw.OnDrawingStart -= _onDrawingStart;
		_draw.OnCast -= _onDrawingEnd;
	}

	public override void _Ready()
	{
		GameState = GameState.MainMenu;
	}

	#endregion

	
	
	#region ShapeDrawing

	public void _onDrawingStart()
	{
		_player.StartCasting();
	}

	public void _onDrawingEnd(AttackShape cast)
	{
		foreach (EnemyBrain pawn in _pool.Pawns)
		{
			Vector2 onScreen = _camera.UnprojectPosition(pawn.GlobalPosition);
			if (Geometry2D.IsPointInPolygon(onScreen, cast.Points) && pawn.Shape == cast.Shape)
			{
				pawn.Health.TakeDamage(1);
			}
		}

		_player.StopCasting();
	}

	#endregion
}
